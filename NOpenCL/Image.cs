// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using NOpenCL.SafeHandles;

    public sealed class Image : MemObject<ImageSafeHandle>
    {
        internal Image(Context context, ImageSafeHandle handle)
            : base(context, handle)
        {
        }

        public ImageFormat Format
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public UIntPtr ElementSize
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.ElementSize);
            }
        }

        public UIntPtr RowPitch
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.RowPitch);
            }
        }

        public UIntPtr SlicePitch
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.SlicePitch);
            }
        }

        public UIntPtr Width
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.Width);
            }
        }

        public UIntPtr Height
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.Height);
            }
        }

        public UIntPtr Depth
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.Depth);
            }
        }

        public UIntPtr ArraySize
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.ArraySize);
            }
        }

        public Buffer Buffer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public uint NumMipLevels
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.NumMipLevels);
            }
        }

        public uint NumSamples
        {
            get
            {
                return UnsafeNativeMethods.GetImageInfo(Handle, UnsafeNativeMethods.ImageInfo.NumSamples);
            }
        }
    }
}
