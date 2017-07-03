// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;

    public sealed class KernelArgument
    {
        private readonly Kernel _kernel;
        private readonly int _index;

        internal KernelArgument(Kernel kernel, int index)
        {
            if (kernel == null)
                throw new ArgumentNullException(nameof(kernel));
            if (index < 0)
                throw new ArgumentOutOfRangeException(nameof(index));

            _kernel = kernel;
            _index = index;
        }

        public Kernel Kernel
        {
            get
            {
                return _kernel;
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public AddressQualifier AddressQualifier
        {
            get
            {
                return (AddressQualifier)UnsafeNativeMethods.GetKernelArgInfo(_kernel.Handle, _index, UnsafeNativeMethods.KernelArgInfo.AddressQualifier);
            }
        }

        public AccessQualifier AccessQualifier
        {
            get
            {
                return (AccessQualifier)UnsafeNativeMethods.GetKernelArgInfo(_kernel.Handle, _index, UnsafeNativeMethods.KernelArgInfo.AccessQualifier);
            }
        }

        public string TypeName
        {
            get
            {
                return UnsafeNativeMethods.GetKernelArgInfo(_kernel.Handle, _index, UnsafeNativeMethods.KernelArgInfo.TypeName);
            }
        }

        public TypeQualifiers TypeQualifiers
        {
            get
            {
                return (TypeQualifiers)UnsafeNativeMethods.GetKernelArgInfo(_kernel.Handle, _index, UnsafeNativeMethods.KernelArgInfo.TypeQualifier);
            }
        }

        public string Name
        {
            get
            {
                return UnsafeNativeMethods.GetKernelArgInfo(_kernel.Handle, _index, UnsafeNativeMethods.KernelArgInfo.Name);
            }
        }

        public void SetValue(Buffer buffer)
        {
            if (buffer == null)
                throw new ArgumentNullException(nameof(buffer));

            unsafe
            {
                IntPtr handle = buffer.Handle.DangerousGetHandle();
                SetValue((UIntPtr)IntPtr.Size, (IntPtr)(&handle));
            }
        }

        public void SetValue(Image image)
        {
            if (image == null)
                throw new ArgumentNullException(nameof(image));

            unsafe
            {
                IntPtr handle = image.Handle.DangerousGetHandle();
                SetValue((UIntPtr)IntPtr.Size, (IntPtr)(&handle));
            }
        }

        public void SetValue(int value)
        {
            unsafe
            {
                SetValue((UIntPtr)sizeof(int), (IntPtr)(&value));
            }
        }

        public void SetValue(UIntPtr size, IntPtr value)
        {
            UnsafeNativeMethods.SetKernelArg(_kernel.Handle, _index, size, value);
        }
    }
}
