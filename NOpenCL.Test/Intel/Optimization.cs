/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.Test.Intel
{
    using System;
    using System.Diagnostics;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = NOpenCL.Buffer;

    [TestClass]
    public class Optimization
    {
        private readonly Random Random = new Random();

        [TestMethod]
        public void TestOptimization()
        {
            int taskSize = 1 << 24;
            int localWorkSize = 32;
            bool warming = true;
            bool gather4 = false;
            bool useRelaxedMath = false;
            bool useHostPointer = true;
            bool autoGroupSize = false;
            bool enableProfiling = true;
            bool runOnGpu = true;
            TestOptimization(taskSize, localWorkSize, warming, gather4, useRelaxedMath, useHostPointer, autoGroupSize, enableProfiling, runOnGpu);
        }

        public void TestOptimization(int taskSize, int localWorkSize, bool warming, bool gather4, bool useRelaxedMath, bool useHostPointer, bool autoGroupSize, bool enableProfiling, bool runOnGpu)
        {
            int globalWorkSize = taskSize;
            if (!autoGroupSize)
                Assert.AreEqual(0, globalWorkSize % localWorkSize, "Global or local work size is incorrect.");

            Context context;
            Device device;
            CommandQueue commandQueue;
            Kernel kernel;
            Kernel kernel4;
            using (SetupOpenCL(out context, out device, out commandQueue, out kernel, out kernel4, runOnGpu, useHostPointer, enableProfiling))
            {
                float[] input = new float[taskSize];
                float[] regularOutput = new float[taskSize];
                float[] openCLOutput = new float[taskSize];

                // set input array to random legal values
                double minimum = -255.0;
                double maximum = 255.0;
                for (int i = 0; i < input.Length; i++)
                    input[i] = (float)(Random.NextDouble() * (maximum - minimum) + minimum);

                // do simple math
                TimeSpan stopwatchTime;
                TimeSpan profiledTime;
                TimeSpan readTime;
                ExecuteKernel(context, device, commandQueue, gather4 ? kernel4 : kernel, input, openCLOutput, gather4 ? globalWorkSize / 4 : globalWorkSize, localWorkSize, warming, useHostPointer, autoGroupSize, enableProfiling, out stopwatchTime, out profiledTime, out readTime);

                Console.WriteLine("Executing reference...");
                ExecuteNative(input, regularOutput);
                Console.WriteLine("Done");
                Console.WriteLine();

                Console.WriteLine("NDRange perf counter time {0} ms.", stopwatchTime.TotalMilliseconds);
                if (enableProfiling)
                    Console.WriteLine("NDRange event profiling time {0} ms.", profiledTime.TotalMilliseconds);

                Console.WriteLine("Read buffer performance counter time {0} ms.", readTime.TotalMilliseconds);

                // do verification
                Console.WriteLine("Performing verification...");
                for (int i = 0; i < taskSize; i++)
                {
                    Assert.AreEqual(regularOutput[i], openCLOutput[i], 0.01);
                }
            }
        }

        private unsafe void ExecuteKernel(Context context, Device device, CommandQueue commandQueue, Kernel kernel, float[] input, float[] output, int globalWorkSize, int localWorkSize, bool warming, bool useHostPointer, bool autoGroupSize, bool enableProfiling,
            out TimeSpan stopwatchTime, out TimeSpan profiledTime, out TimeSpan readTime)
        {
            MemoryFlags inFlags = (useHostPointer ? MemoryFlags.UseHostPointer : MemoryFlags.CopyHostPointer) | MemoryFlags.ReadOnly;
            MemoryFlags outFlags = (useHostPointer ? MemoryFlags.UseHostPointer : MemoryFlags.CopyHostPointer) | MemoryFlags.ReadWrite;

            int taskSize = input.Length;
            // allocate buffers
            fixed (float* pinput = input, poutput = output)
            {
                using (Buffer inputBuffer = context.CreateBuffer(inFlags, sizeof(float) * taskSize, (IntPtr)pinput),
                    outputBuffer = context.CreateBuffer(outFlags, sizeof(float) * taskSize, (IntPtr)poutput))
                {
                    kernel.Arguments[0].SetValue(inputBuffer);
                    kernel.Arguments[1].SetValue(outputBuffer);

                    Console.WriteLine("Original global work size {0}", globalWorkSize);
                    Console.WriteLine("Original local work size {0}", localWorkSize);
                    if (autoGroupSize)
                    {
                        Console.WriteLine("Run-time determines optimal workgroup size");
                    }

                    IntPtr workGroupSizeMaximum = kernel.GetWorkGroupSize(device);
                    Console.WriteLine("Maximum workgroup size for this kernel  {0}", workGroupSizeMaximum.ToInt64());

                    if (warming)
                    {
                        Console.Write("Warming up OpenCL execution...");
                        using (commandQueue.EnqueueNDRangeKernel(kernel, new[] { (IntPtr)globalWorkSize }, autoGroupSize ? null : new[] { (IntPtr)localWorkSize }))
                        {
                        }

                        commandQueue.Finish();
                        Console.WriteLine("Done");
                    }

                    Console.Write("Executing OpenCL kernel...");
                    Stopwatch timer = Stopwatch.StartNew();
                    // execute kernel, pls notice autoGroupSize
                    using (Event perfEvent = commandQueue.EnqueueNDRangeKernel(kernel, new[] { (IntPtr)globalWorkSize }, autoGroupSize ? null : new[] { (IntPtr)localWorkSize }))
                    {
                        Event.WaitAll(perfEvent);
                        stopwatchTime = timer.Elapsed;

                        Console.WriteLine("Done");

                        if (enableProfiling)
                        {
                            ulong start = perfEvent.CommandStartTime;
                            ulong end = perfEvent.CommandEndTime;
                            // a tick is 100ns
                            profiledTime = TimeSpan.FromTicks((long)(end - start) / 100);
                        }
                        else
                        {
                            profiledTime = TimeSpan.Zero;
                        }
                    }

                    timer.Restart();
                    if (useHostPointer)
                    {
                        IntPtr tmpPtr;
                        using (commandQueue.EnqueueMapBuffer(outputBuffer, true, MapFlags.Read, 0, sizeof(float) * taskSize, out tmpPtr))
                        {
                        }

                        Assert.AreEqual((IntPtr)poutput, tmpPtr, "EnqueueMapBuffer failed to return original pointer");
                        using (commandQueue.EnqueueUnmapMemObject(outputBuffer, tmpPtr))
                        {
                        }
                    }
                    else
                    {
                        using (commandQueue.EnqueueReadBuffer(outputBuffer, true, 0, sizeof(float) * taskSize, (IntPtr)poutput))
                        {
                        }
                    }

                    commandQueue.Finish();
                    readTime = timer.Elapsed;
                }
            }
        }

        private void ExecuteNative(float[] input, float[] output)
        {
            Stopwatch timer = Stopwatch.StartNew();
            int taskSize = input.Length;
            for (int i = 0; i < taskSize; i++)
                output[i] = 1.0f / (float)Math.Sqrt(Math.Abs(input[i]));

            TimeSpan elapsed = timer.Elapsed;
            Console.WriteLine("Native perf counter time {0} ms.", elapsed.TotalMilliseconds);
        }

        private IDisposable SetupOpenCL(out Context context, out Device device, out CommandQueue commandQueue, out Kernel kernel, out Kernel kernel4, bool runOnGPU, bool useRelaxedMath, bool enableProfiling)
        {
            DisposableCollection<IDisposable> disposables = new DisposableCollection<IDisposable>(true);
            try
            {
                string buildOptions = "-cl-fast-relaxed-math";

                if (runOnGPU)
                    Console.WriteLine("Trying to run on a processor graphics");
                else
                    Console.WriteLine("Trying to run on a CPU");

                Platform platform = GetIntelOpenCLPlatform();
                Assert.IsNotNull(platform, "Failed to find Intel OpenCL platform.");

                // create the OpenCL context
                if (runOnGPU)
                {
                    Device[] devices = platform.GetDevices(DeviceType.Gpu);
                    device = devices[0];
                    context = Context.Create(device);
                    disposables.Add(context);
                }
                else
                {
                    Device[] devices = platform.GetDevices(DeviceType.Cpu);
                    device = devices[0];
                    context = Context.Create(device);
                }

                commandQueue = context.CreateCommandQueue(device, enableProfiling ? CommandQueueProperties.ProfilingEnable : 0);
                disposables.Add(commandQueue);

                string source = @"
__kernel void
SimpleKernel( const __global float *input, __global float *output)
{
    size_t index = get_global_id(0);
    output[index] = rsqrt(fabs(input[index]));
}

__kernel /*__attribute__((vec_type_hint(float4))) */ void
SimpleKernel4( const __global float4 *input, __global float4 *output)
{
    size_t index = get_global_id(0);
    output[index] = rsqrt(fabs(input[index]));
}
";
                Program program = context.CreateProgramWithSource(source);
                disposables.Add(program);

                program.Build(useRelaxedMath ? buildOptions : null);
                kernel = program.CreateKernel("SimpleKernel");
                disposables.Add(kernel);

                kernel4 = program.CreateKernel("SimpleKernel4");
                disposables.Add(kernel4);

                Console.WriteLine("Using device {0}...", device.Name);
                Console.WriteLine("Using {0} compute units...", device.MaxComputeUnits);
                Console.WriteLine("Buffer alignment required for zero-copying is {0} bytes", device.MemoryBaseAddressAlignment);
            }
            catch
            {
                disposables.Dispose();
                throw;
            }

            return disposables;
        }

        private Platform GetIntelOpenCLPlatform()
        {
            Platform[] platforms = Platform.GetPlatforms();
            foreach (Platform platform in platforms)
            {
                Console.WriteLine("Found platform: {0}", platform.Name);
                if ("Intel(R) OpenCL".Equals(platform))
                    return platform;
            }

            if (platforms.Length == 0)
                return null;

            return platforms[0];
        }
    }
}
