/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL.Test
{
    using System;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class TestPlatform
    {
        [TestMethod]
        public void TestGetPlatforms()
        {
            Platform[] platforms = Platform.GetPlatforms();
            Assert.IsNotNull(platforms);
            if (platforms.Length == 0)
            {
                Assert.Inconclusive("No OpenCL platforms found.");
                return;
            }

            foreach (Platform platform in platforms)
            {
                TestPlatformProperties(platform);
            }
        }

        private static void TestPlatformProperties(Platform platform)
        {
            Assert.IsNotNull(platform);

            Console.WriteLine(platform.ToString());

            string profile = platform.Profile;
            Assert.IsNotNull(profile);
            switch (profile)
            {
            case "FULL_PROFILE":
            case "EMBEDDED_PROFILE":
                break;

            default:
                Assert.Inconclusive("Unknown platform profile: " + profile);
                break;
            }

            string version = platform.Version;
            StringAssert.Matches(version, new Regex("^OpenCL (1.[12]|2.[01]) .*$")); 

            string name = platform.Name;
            Assert.IsNotNull(name);
            Assert.AreNotEqual(string.Empty, name);

            string vendor = platform.Vendor;
            Assert.IsNotNull(vendor);
            Assert.AreNotEqual(string.Empty, vendor);

            IReadOnlyList<string> extensions = platform.Extensions;
            Assert.IsNotNull(extensions);
            foreach (string extension in extensions)
            {
                Assert.IsNotNull(extension);
                Assert.AreNotEqual(string.Empty, extension);
            }
        }
    }
}
