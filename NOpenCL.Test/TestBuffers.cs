/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = NOpenCL.Mem;

    [TestClass]
    public class TestBuffers
    {
        [TestMethod]
        public void TestBufferDestroyedEvent()
        {
            bool destroyed = false;

            Platform platform = Platform.GetPlatforms()[0];
            using (Context context = Context.Create(platform.GetDevices()))
            {
                using (Mem buffer = context.CreateBuffer(MemoryFlags.AllocateHostPointer, 1024))
                {
                    buffer.Destroyed += (sender, e) => destroyed = true;
                    Assert.IsFalse(destroyed);
                }
            }

            Assert.IsTrue(destroyed);
        }
    }
}
