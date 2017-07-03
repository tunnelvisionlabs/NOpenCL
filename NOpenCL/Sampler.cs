// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.ComponentModel;
    using NOpenCL.SafeHandles;

    /// <summary>
    /// To create an instance of <see cref="Sampler"/>, call <see cref="NOpenCL.Context.CreateSampler"/>.
    /// For more information, see the documentation for
    /// <a href="http://www.khronos.org/registry/cl/sdk/1.2/docs/man/xhtml/sampler_t.html"><c>sampler_t</c></a>.
    /// </summary>
    public sealed class Sampler : IDisposable
    {
        private readonly SamplerSafeHandle _handle;
        private readonly Context _context;
        private bool _disposed;

        internal Sampler(SamplerSafeHandle handle, Context context)
        {
            if (handle == null)
                throw new ArgumentNullException(nameof(handle));
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            _handle = handle;
            _context = context;
        }

        /// <summary>
        /// Get the sampler reference count. The reference count returned should be
        /// considered immediately stale. It is unsuitable for general use in applications.
        /// This feature is provided for identifying memory leaks.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.ReferenceCount);
            }
        }

        /// <summary>
        /// Get the <see cref="Context"/> specified when this sampler was created.
        /// </summary>
        public Context Context
        {
            get
            {
                ThrowIfDisposed();
                return _context;
            }
        }

        /// <summary>
        /// Get the normalized coordinates value associated with this sampler. This value
        /// specifies whether the <em>x</em>, <em>y</em> and <em>z</em> coordinates are
        /// passed in as normalized or unnormalized values.
        /// </summary>
        public bool NormalizedCoordinates
        {
            get
            {
                return UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.NormalizedCoordinates);
            }
        }

        /// <summary>
        /// Get the addressing mode value associated with this sampler.
        /// </summary>
        public AddressingMode AddressingMode
        {
            get
            {
                return (AddressingMode)UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.AddressingMode);
            }
        }

        /// <summary>
        /// Get the filter mode value associated with this sampler.
        /// </summary>
        public FilterMode FilterMode
        {
            get
            {
                return (FilterMode)UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.FilterMode);
            }
        }

        internal SamplerSafeHandle Handle
        {
            get
            {
                ThrowIfDisposed();
                return _handle;
            }
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
