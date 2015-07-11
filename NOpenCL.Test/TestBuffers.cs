// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using Buffer = NOpenCL.Buffer;

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
                using (Buffer buffer = context.CreateBuffer(MemoryFlags.AllocateHostPointer, 1024))
                {
                    buffer.Destroyed += (sender, e) => destroyed = true;
                    Assert.IsFalse(destroyed);
                }
            }

            Assert.IsTrue(destroyed);
        }
    }
}
