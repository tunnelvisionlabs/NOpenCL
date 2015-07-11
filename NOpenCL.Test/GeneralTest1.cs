using System;
using System.Linq;
using NOpenCL;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace NOpenCL.Test
{
    //This test is based on an example by Derek Gerstmann.
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class GeneralTest1
    {
        [TestMethod]
        public void GeneralTest()
        {
            // Lets say we have N integers and we want to square their values.
            int N = 10000;
            Random r = new Random();
            int[] inputArray = Enumerable.Range(0, N).Select(n => r.Next(100)).ToArray();

            // Lets choose a platform (AMD, NVidia, Intel, etc..)
            Platform platform = null;
            foreach (var p in Platform.GetPlatforms())
                if (Regex.IsMatch(p.Version, "^OpenCL (1.[12]|2.[01]).*$"))
                {
                    platform = p;
                    break;
                }
            if (platform == null)
                throw new PlatformNotSupportedException("No OpenCL platform found.");

            // Lets pick a device to use from that platform. We can filter by CPU, GPU, etc...
            Device device = platform.GetDevices(DeviceType.Gpu)[0];

            // Setup Context and CommandQueue
            Context context = Context.Create(device);
            CommandQueue commandQueue = context.CreateCommandQueue(device);

            // Create and build a program from our OpenCL-C sourcePtr code
            string source =
@"__kernel void square(
 __global int* input, __global int* output)
{
    size_t i = get_global_id( 0);
    output[i] = input[i] * input[i];
}";
            Program program = context.CreateProgramWithSource(source);
            program.Build();

            // Create a kernel from our program
            Kernel kernel = program.CreateKernel("square");

            // Allocate input and output buffers, and fill the input with data
            Mem input = context.CreateBuffer(MemoryFlags.ReadOnly, N * sizeof(int));
            Mem output = context.CreateBuffer(MemoryFlags.WriteOnly, N * sizeof(int));

            // Copy our host buffer of random values to the input device buffer
            commandQueue.EnqueueWriteBuffer(input, true, inputArray);

            // Get the maximum number of work items supported for this kernel on this device
            ulong local = kernel.GetWorkGroupSize(device);

            // Set the arguments to our kernel, and enqueue it for execution
            kernel.Arguments[0].SetValue(input);
            kernel.Arguments[1].SetValue(output);
            commandQueue.EnqueueNDRangeKernel(kernel, N, 256);

            // Force the command queue to get processed, wait until all commands are complete
            commandQueue.Finish();

            // Read back the results
            int[] results = new int[N];
            IntPtr dest = Marshal.AllocHGlobal(N * 4);
            commandQueue.EnqueueReadBufferAndWait(output, dest, N * 4);
            Marshal.Copy(dest, results, 0, results.Length);

            // Validate our results
            int correct = 0;
            for (int i = 0; i < N; i++)
                correct += (results[i] == inputArray[i] * inputArray[i]) ? 1 : 0;

            Assert.AreEqual(correct, N);
        }
    }
}
