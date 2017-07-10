// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Test.NVidia
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = NOpenCL.Buffer;

    [TestClass]
    public class Bandwidth
    {
        private const int DefaultSize = 32 * (1 << 20); // 32 M
        private const int DefaultIncrement = 1 << 22; // 4 M
        private const int MemoryCopyIterations = 100;

        [TestMethod]
        public void TestBandwidth()
        {
            int start = DefaultSize;
            int end = DefaultSize;
            int increment = DefaultIncrement;
            PrintMode printMode = PrintMode.UserReadable;
            MemoryMode memoryMode = MemoryMode.Pageable;
            AccessMode accessMode = AccessMode.Direct;

            // Get OpenCL platform ID for NVIDIA if available, otherwise default
            Platform platform = OclUtils.GetPlatform();

            // Find out how many devices there are
            Device[] devices = platform.GetDevices(DeviceType.Gpu);
            if (devices.Length == 0)
            {
                Console.WriteLine("No GPU devices found. Falling back to CPU for test...");
                devices = platform.GetDevices(DeviceType.Cpu);
                Assert.AreNotEqual(0, devices.Length, "There are no devices supporting OpenCL");
            }

            int startDevice = 0;
            int endDevice = 0;

            // Get and log the device info
            Console.WriteLine("Running on...");
            Console.WriteLine();
            for (int i = startDevice; i <= endDevice; i++)
                Console.WriteLine(devices[i].Name);

            Console.WriteLine();

            // Mode
            Console.WriteLine("Quick Mode");
            Console.WriteLine();
            TestMode testMode = TestMode.Quick;

            bool hostToDevice = true;
            bool deviceToHost = true;
            bool deviceToDevice = true;

            if (testMode == TestMode.Range)
                throw new NotImplementedException();

            using (var context = Context.Create(devices))
            {
                if (hostToDevice)
                    TestBandwidth(context, devices, start, end, increment, testMode, MemoryCopyKind.HostToDevice, printMode, accessMode, memoryMode, startDevice, endDevice);

                if (deviceToHost)
                    TestBandwidth(context, devices, start, end, increment, testMode, MemoryCopyKind.DeviceToHost, printMode, accessMode, memoryMode, startDevice, endDevice);

                if (deviceToDevice)
                    TestBandwidth(context, devices, start, end, increment, testMode, MemoryCopyKind.DeviceToDevice, printMode, accessMode, memoryMode, startDevice, endDevice);
            }
        }

        private CommandQueue CreateQueue(Context context, Device device)
        {
            return context.CreateCommandQueue(device, CommandQueueProperties.ProfilingEnable);
        }

        private void TestBandwidth(Context context, Device[] devices, int start, int end, int increment, TestMode testMode, MemoryCopyKind memoryCopyKind, PrintMode printMode, AccessMode accessMode, MemoryMode memoryMode, int startDevice, int endDevice)
        {
            switch (testMode)
            {
            case TestMode.Quick:
                TestBandwidthQuick(context, devices, DefaultSize, memoryCopyKind, printMode, accessMode, memoryMode, startDevice, endDevice);
                break;

            case TestMode.Range:
                TestBandwidthRange(context, devices, start, end, increment, memoryCopyKind, printMode, accessMode, memoryMode, startDevice, endDevice);
                break;

            case TestMode.Shmoo:
                TestBandwidthShmoo(context, devices, memoryCopyKind, printMode, accessMode, memoryMode, startDevice, endDevice);
                break;

            default:
                break;
            }
        }

        private void TestBandwidthQuick(Context context, Device[] devices, int size, MemoryCopyKind memoryCopyKind, PrintMode printMode, AccessMode accessMode, MemoryMode memoryMode, int startDevice, int endDevice)
        {
            TestBandwidthRange(context, devices, size, size, DefaultIncrement, memoryCopyKind, printMode, accessMode, memoryMode, startDevice, endDevice);
        }

        private void TestBandwidthRange(Context context, Device[] devices, int start, int end, int increment, MemoryCopyKind memoryCopyKind, PrintMode printMode, AccessMode accessMode, MemoryMode memoryMode, int startDevice, int endDevice)
        {
            // count the number of copies we're going to run
            int count = 1 + ((end - start) / increment);
            int[] memSizes = new int[count];
            double[] bandwidths = new double[count];

            // Before calculating the cumulative bandwidth, initialize bandwidths array to NULL
            for (int i = 0; i < count; i++)
                bandwidths[i] = 0.0;

            // Use the device asked by the user
            for (int currentDevice = startDevice; currentDevice <= endDevice; currentDevice++)
            {
                // Allocate command queue for the device (dealloc first if already allocated)
                using (CommandQueue queue = CreateQueue(context, devices[currentDevice]))
                {
                    // run each of the copies
                    for (int i = 0; i < count; i++)
                    {
                        memSizes[i] = start + (i * increment);
                        switch (memoryCopyKind)
                        {
                        case MemoryCopyKind.DeviceToHost:
                            bandwidths[i] += TestDeviceToHostTransfer(context, queue, memSizes[i], accessMode, memoryMode);
                            break;

                        case MemoryCopyKind.HostToDevice:
                            bandwidths[i] += TestHostToDeviceTransfer(context, queue, memSizes[i], accessMode, memoryMode);
                            break;

                        case MemoryCopyKind.DeviceToDevice:
                            bandwidths[i] += TestDeviceToDeviceTransfer(context, queue, memSizes[i]);
                            break;
                        }
                    }
                }
            } // Complete the bandwidth computation on all the devices

            // print results
            if (printMode == PrintMode.Csv)
            {
                PrintResultsCsv(memSizes, bandwidths, count, memoryCopyKind, accessMode, memoryMode, 1 + endDevice - startDevice);
            }
            else
            {
                PrintResultsReadable(memSizes, bandwidths, count, memoryCopyKind, accessMode, memoryMode, 1 + endDevice - startDevice);
            }
        }

        private void TestBandwidthShmoo(Context context, Device[] devices, MemoryCopyKind memoryCopyKind, PrintMode printMode, AccessMode accessMode, MemoryMode memoryMode, int startDevice, int endDevice)
        {
            throw new NotImplementedException();
        }

        private double TestDeviceToHostTransfer(Context context, CommandQueue commandQueue, int memSize, AccessMode accessMode, MemoryMode memoryMode)
        {
            if (memoryMode == MemoryMode.Pinned)
                return TestDeviceToHostTransferPinned(context, commandQueue, memSize, accessMode);
            else
                return TestDeviceToHostTransferPaged(context, commandQueue, memSize, accessMode);
        }

        private double TestDeviceToHostTransferPinned(Context context, CommandQueue commandQueue, int memSize, AccessMode accessMode)
        {
            // create a host buffer
            using (Buffer pinnedData = context.CreateBuffer(MemoryFlags.ReadWrite | MemoryFlags.AllocateHostPointer, memSize))
            {
                // get a mapped pointer
                IntPtr h_data;
                commandQueue.EnqueueMapBuffer(pinnedData, true, MapFlags.Write, 0, memSize, out h_data);

                // initialize
                for (int i = 0; i < memSize; i++)
                    Marshal.WriteByte(h_data, i, (byte)i);

                // unmap and make data in the host buffer valid
                commandQueue.EnqueueUnmapMemObject(pinnedData, h_data);

                // allocate device memory
                using (Buffer deviceData = context.CreateBuffer(MemoryFlags.ReadWrite, memSize))
                {
                    // initialize device memory
                    commandQueue.EnqueueMapBuffer(pinnedData, true, MapFlags.Write, 0, memSize, out h_data);
                    commandQueue.EnqueueWriteBuffer(deviceData, false, 0, memSize, h_data);

                    // sync queue to host
                    commandQueue.Finish();
                    var timer = Stopwatch.StartNew();
                    if (accessMode == AccessMode.Direct)
                    {
                        // DIRECT: API access to device buffer
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            commandQueue.EnqueueReadBuffer(deviceData, false, 0, memSize, h_data);
                        }

                        commandQueue.Finish();
                    }
                    else
                    {
                        // MAPPED: mapped pointers to device buffer for conventional pointer access
                        IntPtr dm_idata;
                        commandQueue.EnqueueMapBuffer(deviceData, true, MapFlags.Read, 0, memSize, out dm_idata);
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            CopyMemory(h_data, dm_idata, (UIntPtr)memSize);
                        }

                        commandQueue.EnqueueUnmapMemObject(deviceData, dm_idata);
                    }

                    // get the elapsed time in seconds
                    double elapsedTimeInSeconds = timer.Elapsed.TotalSeconds;

                    // Calculate bandwidth in MB/s
                    //      This is for kernels that read and write GMEM simultaneously
                    //      Obtained Throughput for unidirectional block copies will be 1/2 of this #
                    double bandwidthInMBs = 2.0 * ((double)memSize * (double)MemoryCopyIterations) / (elapsedTimeInSeconds * (double)(1 << 20));

                    return bandwidthInMBs;
                }
            }
        }

        [DllImport("kernel32.dll")]
        ////[SuppressUnmanagedCodeSecurity]
        private static extern void CopyMemory(IntPtr destination, IntPtr source, UIntPtr length);

        private unsafe double TestDeviceToHostTransferPaged(Context context, CommandQueue commandQueue, int memSize, AccessMode accessMode)
        {
            // standard host allocation
            byte[] data = new byte[memSize];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            fixed (byte* pdata = data)
            {
                // allocate device memory
                using (Buffer deviceData = context.CreateBuffer(MemoryFlags.ReadWrite, memSize))
                {
                    // initialize device memory
                    commandQueue.EnqueueWriteBuffer(deviceData, false, 0, memSize, (IntPtr)pdata);

                    // sync queue to host
                    commandQueue.Finish();
                    var timer = Stopwatch.StartNew();
                    if (accessMode == AccessMode.Direct)
                    {
                        // DIRECT: API access to device buffer
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            commandQueue.EnqueueReadBuffer(deviceData, false, 0, memSize, (IntPtr)pdata);
                        }

                        commandQueue.Finish();
                    }
                    else
                    {
                        // MAPPED: mapped pointers to device buffer for conventional pointer access
                        IntPtr dm_idata;
                        commandQueue.EnqueueMapBuffer(deviceData, true, MapFlags.Read, 0, memSize, out dm_idata);
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            CopyMemory((IntPtr)pdata, dm_idata, (UIntPtr)memSize);
                        }

                        commandQueue.EnqueueUnmapMemObject(deviceData, dm_idata);
                    }

                    // get the elapsed time in seconds
                    double elapsedTimeInSeconds = timer.Elapsed.TotalSeconds;

                    // Calculate bandwidth in MB/s
                    //      This is for kernels that read and write GMEM simultaneously
                    //      Obtained Throughput for unidirectional block copies will be 1/2 of this #
                    double bandwidthInMBs = 2.0 * ((double)memSize * (double)MemoryCopyIterations) / (elapsedTimeInSeconds * (double)(1 << 20));

                    return bandwidthInMBs;
                }
            }
        }

        private double TestHostToDeviceTransfer(Context context, CommandQueue commandQueue, int memorySize, AccessMode accessMode, MemoryMode memoryMode)
        {
            if (memoryMode == MemoryMode.Pinned)
                return TestHostToDeviceTransferPinned(context, commandQueue, memorySize, accessMode);
            else
                return TestHostToDeviceTransferPaged(context, commandQueue, memorySize, accessMode);
        }

        private double TestHostToDeviceTransferPinned(Context context, CommandQueue commandQueue, int memSize, AccessMode accessMode)
        {
            // Create a host buffer
            using (Buffer pinnedData = context.CreateBuffer(MemoryFlags.ReadWrite | MemoryFlags.AllocateHostPointer, memSize))
            {
                // get a mapped pointer
                IntPtr h_data;
                commandQueue.EnqueueMapBuffer(pinnedData, true, MapFlags.Write, 0, memSize, out h_data);

                // initialize
                for (int i = 0; i < memSize; i++)
                    Marshal.WriteByte(h_data, i, (byte)i);

                // unmap and make data in the host buffer valid
                commandQueue.EnqueueUnmapMemObject(pinnedData, h_data);

                // allocate device memory
                using (Buffer deviceData = context.CreateBuffer(MemoryFlags.ReadWrite, memSize))
                {
                    // sync queue to host
                    commandQueue.Finish();
                    var timer = Stopwatch.StartNew();
                    if (accessMode == AccessMode.Direct)
                    {
                        commandQueue.EnqueueMapBuffer(pinnedData, true, MapFlags.Read, 0, memSize, out h_data);

                        // DIRECT: API access to device buffer
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            commandQueue.EnqueueWriteBuffer(deviceData, false, 0, memSize, h_data);
                        }

                        commandQueue.Finish();
                    }
                    else
                    {
                        // MAPPED: mapped pointers to device buffer for conventional pointer access
                        IntPtr dm_idata;
                        commandQueue.EnqueueMapBuffer(deviceData, true, MapFlags.Write, 0, memSize, out dm_idata);
                        commandQueue.EnqueueMapBuffer(pinnedData, true, MapFlags.Read, 0, memSize, out h_data);

                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            CopyMemory(dm_idata, h_data, (UIntPtr)memSize);
                        }

                        commandQueue.EnqueueUnmapMemObject(deviceData, dm_idata);
                    }

                    // get the elapsed time in seconds
                    double elapsedTimeInSeconds = timer.Elapsed.TotalSeconds;

                    // Calculate bandwidth in MB/s
                    //      This is for kernels that read and write GMEM simultaneously
                    //      Obtained Throughput for unidirectional block copies will be 1/2 of this #
                    double bandwidthInMBs = 2.0 * ((double)memSize * (double)MemoryCopyIterations) / (elapsedTimeInSeconds * (double)(1 << 20));

                    return bandwidthInMBs;
                }
            }
        }

        private unsafe double TestHostToDeviceTransferPaged(Context context, CommandQueue commandQueue, int memSize, AccessMode accessMode)
        {
            // standard host allocation
            byte[] data = new byte[memSize];
            for (int i = 0; i < data.Length; i++)
                data[i] = (byte)i;

            fixed (byte* pdata = data)
            {
                // allocate device memory
                using (Buffer deviceData = context.CreateBuffer(MemoryFlags.ReadWrite, memSize))
                {
                    // sync queue to host
                    commandQueue.Finish();
                    var timer = Stopwatch.StartNew();
                    if (accessMode == AccessMode.Direct)
                    {
                        // DIRECT: API access to device buffer
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            commandQueue.EnqueueWriteBuffer(deviceData, false, 0, memSize, (IntPtr)pdata);
                        }

                        commandQueue.Finish();
                    }
                    else
                    {
                        // MAPPED: mapped pointers to device buffer for conventional pointer access
                        IntPtr dm_idata;
                        commandQueue.EnqueueMapBuffer(deviceData, true, MapFlags.Write, 0, memSize, out dm_idata);
                        for (int i = 0; i < MemoryCopyIterations; i++)
                        {
                            CopyMemory(dm_idata, (IntPtr)pdata, (UIntPtr)memSize);
                        }

                        commandQueue.EnqueueUnmapMemObject(deviceData, dm_idata);
                    }

                    // get the elapsed time in seconds
                    double elapsedTimeInSeconds = timer.Elapsed.TotalSeconds;

                    // Calculate bandwidth in MB/s
                    //      This is for kernels that read and write GMEM simultaneously
                    //      Obtained Throughput for unidirectional block copies will be 1/2 of this #
                    double bandwidthInMBs = 2.0 * ((double)memSize * (double)MemoryCopyIterations) / (elapsedTimeInSeconds * (double)(1 << 20));

                    return bandwidthInMBs;
                }
            }
        }

        private double TestDeviceToDeviceTransfer(Context context, CommandQueue commandQueue, int memorySize)
        {
            if (context == null)
                throw new ArgumentNullException("context");

            double elapsedTimeInSeconds = 0.0;
            double bandwidthInMBs = 0.0;

            // allocate host memory
            byte[] data = new byte[memorySize];

            // initialize the memory
            for (int i = 0; i < memorySize; i++)
                data[i] = 0xFF;

            // allocate device input and output memory and initialize the device input memory
            using (Buffer d_idata = context.CreateBuffer(MemoryFlags.ReadOnly, memorySize),
                d_odata = context.CreateBuffer(MemoryFlags.WriteOnly, memorySize))
            {
                unsafe
                {
                    fixed (byte* rawData = data)
                    {
                        using (commandQueue.EnqueueWriteBuffer(d_idata, true, 0, memorySize, (IntPtr)rawData))
                        {
                        }
                    }
                }

                // sync queue to host, start timer 0, and copy data from one GPU buffer to another GPU buffer
                commandQueue.Finish();
                var timer = Stopwatch.StartNew();
                for (int i = 0; i < MemoryCopyIterations; i++)
                {
                    using (commandQueue.EnqueueCopyBuffer(d_idata, d_odata, 0, 0, memorySize))
                    {
                    }
                }

                // sync with GPU
                commandQueue.Finish();

                // get the elapsed time in seconds
                elapsedTimeInSeconds = timer.Elapsed.TotalSeconds;

                // Calculate bandwidth in MB/s
                //      This is for kernels that read and write GMEM simultaneously
                //      Obtained Throughput for unidirectional block copies will be 1/2 of this #
                bandwidthInMBs = 2.0 * ((double)memorySize * (double)MemoryCopyIterations) / (elapsedTimeInSeconds * (double)(1 << 20));
            }

            return bandwidthInMBs;
        }

        private void PrintResultsReadable(int[] memSizes, double[] bandwidths, int count, MemoryCopyKind memoryCopyKind, AccessMode accessMode, MemoryMode memoryMode, int p)
        {
            if (memoryCopyKind == MemoryCopyKind.DeviceToDevice)
            {
                Console.WriteLine("Device to device bandwidth: {0} devices(s)", p);
            }
            else
            {
                if (memoryCopyKind == MemoryCopyKind.DeviceToHost)
                {
                    Console.Write("Device to host bandwidth: {0} devices(s), ", p);
                }
                else if (memoryCopyKind == MemoryCopyKind.HostToDevice)
                {
                    Console.Write("Host to device bandwidth: {0} devices(s), ", p);
                }

                if (memoryMode == MemoryMode.Pageable)
                {
                    Console.Write("Paged memory");
                }
                else if (memoryMode == MemoryMode.Pinned)
                {
                    Console.Write("Pinned memory");
                }

                if (accessMode == AccessMode.Direct)
                {
                    Console.WriteLine(", direct access");
                }
                else if (accessMode == AccessMode.Mapped)
                {
                    Console.WriteLine(", mapped access");
                }
            }

            Console.WriteLine("   Transfer size (bytes)\tBandwidth (MB/s)");
            for (int i = 0; i < count; i++)
            {
                Console.WriteLine("   {0}\t\t\t{1}{2}", memSizes[i], memSizes[i] < 10000 ? "\t" : string.Empty, bandwidths[i]);
            }

            Console.WriteLine();
        }

        private void PrintResultsCsv(int[] memSizes, double[] bandwidths, int count, MemoryCopyKind memoryCopyKind, AccessMode accessMode, MemoryMode memoryMode, int p)
        {
            throw new NotImplementedException();
        }

        public enum TestMode
        {
            Quick,
            Range,
            Shmoo,
        }

        public enum MemoryCopyKind
        {
            DeviceToHost,
            HostToDevice,
            DeviceToDevice,
        }

        public enum PrintMode
        {
            UserReadable,
            Csv,
        }

        public enum MemoryMode
        {
            Pageable,
            Pinned,
        }

        public enum AccessMode
        {
            Mapped,
            Direct,
        }
    }
}
