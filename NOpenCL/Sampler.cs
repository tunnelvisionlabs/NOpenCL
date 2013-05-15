namespace NOpenCL
{
    using System;

    /// <summary>
    /// To create an instance of <see cref="Sampler"/>, call <see cref="Context.CreateSampler"/>.
    /// </summary>
    public sealed class Sampler : IDisposable
    {
        private readonly SamplerSafeHandle _handle;
        private bool _disposed;

        internal Sampler(SamplerSafeHandle handle)
        {
            if (handle == null)
                throw new ArgumentNullException("handle");

            _handle = handle;
        }

        public uint ReferenceCount
        {
            get
            {
                return UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.ReferenceCount);
            }
        }

        public Context Context
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool NormalizedCoordinates
        {
            get
            {
                return UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.NormalizedCoordinates);
            }
        }

        public AddressingMode AddressingMode
        {
            get
            {
                return (AddressingMode)UnsafeNativeMethods.GetSamplerInfo(Handle, UnsafeNativeMethods.SamplerInfo.AddressingMode);
            }
        }

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
