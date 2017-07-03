// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Extensions
{
    using System;
    using System.Linq;

    public static class KhrD3D10Sharing
    {
        public static readonly string ExtensionName = "cl_khr_d3d10_sharing";

        /// <summary>
        /// If the <c>cl_khr_d3d10_sharing</c> extension is enabled, returns true if
        /// Direct3D 10 resources created as shared by setting MiscFlags to include
        /// D3D10_RESOURCE_MISC_SHARED will perform faster when shared with OpenCL,
        /// compared with resources which have not set this flag. Otherwise returns false.
        /// </summary>
        private static readonly UnsafeNativeMethods.ContextParameterInfo<bool> D3D10PreferSharedResourcesKhr =
            (UnsafeNativeMethods.ContextParameterInfo<bool>)new UnsafeNativeMethods.ParameterInfoBoolean(0x402C);

        public static bool IsSupported(Platform platform)
        {
            if (platform == null)
                throw new ArgumentNullException(nameof(platform));

            return platform.Extensions.Contains(ExtensionName);
        }

        public static bool IsSupported(Device device)
        {
            if (device == null)
                throw new ArgumentNullException(nameof(device));

            return device.Extensions.Contains(ExtensionName);
        }

        public static bool GetD3D10PreferSharedResourcesKhr(this Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return UnsafeNativeMethods.GetContextInfo(context.Handle, D3D10PreferSharedResourcesKhr);
        }
    }
}
