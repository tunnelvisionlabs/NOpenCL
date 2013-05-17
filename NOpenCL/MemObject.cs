namespace NOpenCL
{
    using System;

    public abstract class MemObject : IDisposable
    {
        internal MemObject()
        {
        }

        internal abstract MemObjectSafeHandle BaseHandle
        {
            get;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
