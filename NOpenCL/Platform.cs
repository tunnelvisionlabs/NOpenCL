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
                return UnsafeNativeMethods.GetPlatformInfo(_platform, UnsafeNativeMethods.PlatformInfo.Extensions).Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        public Device[] GetDevices(DeviceType deviceType)
        {
            UnsafeNativeMethods.ClDeviceID[] devices = UnsafeNativeMethods.GetDeviceIDs(ID, deviceType);
            return Array.ConvertAll(devices, device => new Device(device));
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
