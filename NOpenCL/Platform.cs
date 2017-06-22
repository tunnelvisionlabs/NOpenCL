// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;

    public sealed class Platform : IEquatable<Platform>
    {
        private readonly UnsafeNativeMethods.ClPlatformID _platform;

        private Platform(UnsafeNativeMethods.ClPlatformID platform)
        {
            _platform = platform;
        }

        public string Profile
        {
            get
            {
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Profile);
            }
        }

        public string Version
        {
            get
            {
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Version);
            }
        }

        public string Name
        {
            get
            {
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Name);
            }
        }

        public string Vendor
        {
            get
            {
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Vendor);
            }
        }

        public IReadOnlyList<string> Extensions
        {
            get
            {
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Extensions).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal UnsafeNativeMethods.ClPlatformID ID
        {
            get
            {
                return _platform;
            }
        }

        public static Platform[] GetPlatforms()
        {
            UnsafeNativeMethods.ClPlatformID[] platforms = UnsafeNativeMethods.GetPlatformIDs();
            return Array.ConvertAll(platforms, platform => new Platform(platform));
        }

        public Device[] GetDevices()
        {
            return GetDevices(DeviceType.All);
        }

        public Device[] GetDevices(DeviceType deviceType)
        {
            UnsafeNativeMethods.ClDeviceID[] devices = UnsafeNativeMethods.GetDeviceIDs(ID, deviceType);
            return Array.ConvertAll(devices, device => new Device(device));
        }

#pragma warning disable CS1574 // XML comment has cref attribute that could not be resolved
        /// <summary>
        /// Allows the implementation to release the resources allocated by the OpenCL compiler for this platform.
        /// </summary>
        /// <remarks>
        /// This is a hint from the application and does not guarantee that the compiler will not be used
        /// in the future or that the compiler will actually be unloaded by the implementation. Calls to
        /// <see cref="Program.Build()" autoUpgrade="true"/>, <see cref="Program.Compile"/>, or
        /// <see cref="Context.LinkProgram"/> after <see cref="UnloadCompiler"/> will reload the compiler, if necessary,
        /// to build the appropriate program executable.
        /// </remarks>
        public void UnloadCompiler()
#pragma warning restore CS1574 // XML comment has cref attribute that could not be resolved
        {
            UnsafeNativeMethods.UnloadPlatformCompiler(ID);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Platform);
        }

        public bool Equals(Platform other)
        {
            if (other == this)
                return true;
            else if (other == null)
                return false;

            return object.Equals(_platform, other._platform);
        }

        public override int GetHashCode()
        {
            return _platform.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format("{0} {1}: {2}, {3}, Extensions: {{{4}}}", Vendor, Name, Profile, Version, string.Join(", ", Extensions));
        }
    }
}
