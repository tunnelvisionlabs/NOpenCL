// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.Extensions
{
    using System;
    using System.Linq;

    public static class KhrD3D11Sharing
    {
        public static readonly string ExtensionName = "cl_khr_d3d11_sharing";

        /// <summary>
        /// If the <c>cl_khr_d3d11_sharing</c> extension is supported, Returns true if
        /// Direct3D 11 resources created as shared by setting MiscFlags to include
        /// D3D11_RESOURCE_MISC_SHARED will perform faster when shared with OpenCL,
        /// compared with resources which have not set this flag. Otherwise returns false.
        /// </summary>
        private static readonly UnsafeNativeMethods.ContextParameterInfo<bool> D3D11PreferSharedResourcesKhr =
            (UnsafeNativeMethods.ContextParameterInfo<bool>)new UnsafeNativeMethods.ParameterInfoBoolean(0x402D);

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

        public static bool GetD3D11PreferSharedResourcesKhr(this Context context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            return UnsafeNativeMethods.GetContextInfo(context.Handle, D3D11PreferSharedResourcesKhr);
        }
    }
}
