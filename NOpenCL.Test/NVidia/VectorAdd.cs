namespace NOpenCL.Test.NVidia
{
    using System;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = NOpenCL.Buffer;

    [TestClass]
    public class VectorAdd
    {
        /// <summary>
        /// Length of float arrays to process (odd # for illustration)
        /// </summary>
        private const int NumElements = 11444777;

        private const float MinEpsilonError = 1e-3f;

        [TestMethod]
        public unsafe void TestVectorAdd()
        {
            Console.WriteLine("TestVectorAdd Starting...");
            Console.WriteLine();
            Console.WriteLine("# of float elements per array \t= {0}", NumElements);

            // set global and local work size dimensions
            int localWorkSize = 256;
            int globalWorkSize = RoundUp(localWorkSize, NumElements);
            Console.WriteLine("Global work size \t\t= {0}", globalWorkSize);
            Console.WriteLine("Local work size \t\t= {0}", localWorkSize);
            Console.WriteLine("Number of work groups \t\t= {0}", globalWorkSize % localWorkSize + globalWorkSize / localWorkSize);
            Console.WriteLine();

            // allocate and initialize host arrays
            Console.WriteLine("Allocate and initialize host memory...");
            float[] srcA = new float[globalWorkSize];
            float[] srcB = new float[globalWorkSize];
            float[] dst = new float[globalWorkSize];
            float[] golden = new float[NumElements];
            FillArray(srcA, NumElements);
            FillArray(srcB, NumElements);

            // get an OpenCL platform
            Console.WriteLine("Get platform...");
            Platform platform = OclUtils.GetPlatform();

            // get the devices
            Console.WriteLine("Get GPU devices...");
            Device[] devices = platform.GetDevices(DeviceType.Gpu);

            // create the context
            Console.WriteLine("Get context...");
            using (Context context = Context.Create(devices))
            {
                // create a command queue
                Console.WriteLine("Get command queue...");
                using (CommandQueue commandQueue = CommandQueue.Create(context, devices[0], CommandQueueProperties.None))
                {
                    Console.WriteLine("Create buffers...");
                    using (Buffer deviceSrcA = context.CreateBuffer(MemoryFlags.ReadOnly, globalWorkSize * sizeof(float)),
                        deviceSrcB = context.CreateBuffer(MemoryFlags.ReadOnly, globalWorkSize * sizeof(float)),
                        deviceDst = context.CreateBuffer(MemoryFlags.WriteOnly, globalWorkSize * sizeof(float)))
                    {
                        string source =
                            @"// OpenCL Kernel Function for element by element vector addition
__kernel void VectorAdd(__global const float* a, __global const float* b, __global float* c, int iNumElements)
{
    // get index into global data array
    int iGID = get_global_id(0);

    // bound check (equivalent to the limit on a 'for' loop for standard/serial C code
    if (iGID >= iNumElements)
    {   
        return; 
    }

    // add the vector elements
    c[iGID] = a[iGID] + b[iGID];
}
";

                        // create the program
                        Console.WriteLine("Create program with source...");
                        using (Program program = context.CreateProgramWithSource(source))
                        {
                            // build the program
                            string options;
                            if (false)
                                options = "-cl-fast-relaxed-math -DMAC";
                            else
                                options = "-cl-fast-relaxed-math";

                            program.Build(options);

                            // create the kernel
                            using (Kernel kernel = program.CreateKernel("VectorAdd"))
                            {
                                kernel.Arguments[0].SetValue(deviceSrcA);
                                kernel.Arguments[1].SetValue(deviceSrcB);
                                kernel.Arguments[2].SetValue(deviceDst);
                                kernel.Arguments[3].SetValue(NumElements);

                                // Start core sequence... copy input data to GPU, compute, copy results back
                                fixed (float* psrcA = srcA, psrcB = srcB, pdst = dst)
                                {
                                    // asynchronous write of data to GPU device
                                    using (commandQueue.EnqueueWriteBuffer(deviceSrcA, false, 0, sizeof(float) * globalWorkSize, (IntPtr)psrcA))
                                    using (commandQueue.EnqueueWriteBuffer(deviceSrcB, false, 0, sizeof(float) * globalWorkSize, (IntPtr)psrcB))
                                    {
                                    }

                                    // launch kernel
                                    using (commandQueue.EnqueueNDRangeKernel(kernel, (IntPtr)globalWorkSize, (IntPtr)localWorkSize))
                                    {
                                    }

                                    // synchronous/blocking read of results, and check accumulated errors
                                    using (commandQueue.EnqueueReadBuffer(deviceDst, true, 0, sizeof(float) * globalWorkSize, (IntPtr)pdst))
                                    {
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // compute and compare results for golden-host and report errors and pass/fail
            Console.WriteLine("Comparing against host computation...");
            Console.WriteLine();
            VectorAddHost(srcA, srcB, golden, NumElements);
            bool match = Comparefet(golden, dst, NumElements, 0.0f, 0);
            Assert.IsTrue(match);
        }

        private void VectorAddHost(float[] source1, float[] source2, float[] result, int length)
        {
            for (int i = 0; i < length; i++)
                result[i] = source1[i] + source2[i];
        }

        private static bool Comparefet(float[] reference, float[] data, int length, float epsilon, float threshold)
        {
            return CompareDataAsFloatThreshold(reference, data, length, epsilon, threshold);
        }

        private static bool CompareDataAsFloatThreshold(float[] reference, float[] data, int length, float epsilon, float threshold)
        {
            if (reference == null)
                throw new ArgumentNullException("reference");
            if (data == null)
                throw new ArgumentNullException("data");
            if (epsilon < 0)
                throw new ArgumentOutOfRangeException("epsilon");

            // if we set epsilon to be 0, let's set a minimum threshold
            float maxError = Math.Max(epsilon, MinEpsilonError);
            int errorCount = 0;
            bool result = true;

            for (int i = 0; i < length; i++)
            {
                float diff = Math.Abs(reference[i] - data[i]);
                bool comp = diff < maxError;
                result &= comp;

                if (!comp)
                {
                    errorCount++;
#if DEBUG
                    if (errorCount < 50)
                    {
                        Console.WriteLine();
                        Console.WriteLine("    ERROR(epsilon={0}), i={1}, (ref){2} / (data){3} / (diff){4}", maxError, i, reference[i], data[i], diff);
                    }
#endif
                }
            }

            if (threshold == 0.0f)
            {
                if (errorCount > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("    Total # of errors = {0}", errorCount);
                }

                return errorCount == 0;
            }
            else
            {
                if (errorCount > 0)
                {
                    Console.WriteLine();
                    Console.WriteLine("    {0}(%) of bytes mismatched (count={1})", (float)errorCount * 100 / (float)length, errorCount);
                }

                return length * threshold > errorCount;
            }
        }

        private static readonly Random Random = new Random();

        private void FillArray(float[] data, int length)
        {
            for (int i = 0; i < length; i++)
                data[i] = (float)Random.NextDouble();
        }

        private static int RoundUp(int groupSize, int globalSize)
        {
            int r = globalSize % groupSize;
            if (r == 0)
                return globalSize;

            return globalSize + groupSize - r;
        }
    }
}
