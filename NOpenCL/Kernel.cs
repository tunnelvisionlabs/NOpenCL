/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

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
                throw new ArgumentNullException("handle");
            if (program == null)
                throw new ArgumentNullException("program");

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
                return UnsafeNativeMethods.GetKernelInfo(Handle, UnsafeNativeMethods.KernelInfo.Attributes).Split(new [] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
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

        public ulong[] GetGlobalWorkSize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.GlobalWorkSize);
        }

        public ulong GetWorkGroupSize(Device device) 
        {
            return (ulong)UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.WorkGroupSize)[0];
        }

        public ulong[] GetCompileWorkGroupSize(Device device)
        {
            return UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.CompileWorkGroupSize);
        }

        public ulong GetLocalMemorySize(Device device)
        {
            return (ulong)UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.LocalMemorySize)[0];
        }

        public ulong GetPreferredWorkGroupSizeMultiple(Device device)
        {
            return (ulong)UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.PreferredWorkGroupSizeMultiple)[0];
        }

        public ulong GetPrivateMemorySize(Device device)
        {
            return (ulong)UnsafeNativeMethods.GetKernelWorkGroupInfo(Handle, device.ID, UnsafeNativeMethods.KernelWorkGroupInfo.PrivateMemorySize)[0];
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
