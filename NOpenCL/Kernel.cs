// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;

    public sealed class Kernel : IDisposable
    {
        private readonly KernelSafeHandle _handle;
        private readonly Program _program;

        private ReadOnlyCollection<KernelArgument> _arguments;
        private bool _disposed;

        internal Kernel(KernelSafeHandle handle, Program program)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (program == null)
                throw new ArgumentNullException(nameof(program));

            _handle = handle;
            _program = program;
        }

        public ReadOnlyCollection<KernelArgument> Arguments
        {
            get
            {
                if (_arguments == null)
                {
                    uint argumentCount = UnsafeNativeMethods.GetKernelInfo(Handle, UnsafeNativeMethods.KernelInfo.NumArgs);
                    KernelArgument[] arguments = new KernelArgument[argumentCount];
                    for (int i = 0; i < arguments.Length; i++)
                        arguments[i] = new KernelArgument(this, i);

                    _arguments = new ReadOnlyCollection<KernelArgument>(arguments);
                }

                return _arguments;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetKernelInfo(Handle, UnsafeNativeMethods.KernelInfo.ReferenceCount);
            }
        }

        public string FunctionName
        {
            get
            {
                return UnsafeNativeMethods.GetKernelInfo(Handle, UnsafeNativeMethods.KernelInfo.FunctionName);
            }
        }

        public Context Context
        {
            get
            {
                ThrowIfDisposed();
                return _program.Context;
            }
        }

        public Program Program
        {
            get
            {
                ThrowIfDisposed();
                return _program;
            }
        }

        public IReadOnlyList<string> Attributes
        {
            get
            {
                return UnsafeNativeMethods.GetKernelInfo(Handle, UnsafeNativeMethods.KernelInfo.Attributes).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        internal KernelSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
        }

        public IReadOnlyList<IntPtr> GetGlobalWorkSize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.GlobalWorkSize);
        }

        public IntPtr GetWorkGroupSize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.WorkGroupSize);
        }

        public IReadOnlyList<IntPtr> GetCompileWorkGroupSize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.CompileWorkGroupSize);
        }

        public ulong GetLocalMemorySize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.LocalMemorySize);
        }

        public IntPtr GetPreferredWorkGroupSizeMultiple(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.PreferredWorkGroupSizeMultiple);
        }

        public ulong GetPrivateMemorySize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.PrivateMemorySize);
        }

        public void Dispose()
        {
            _handle.Dispose();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
