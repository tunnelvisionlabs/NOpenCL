namespace NOpenCL
{
    using System;
    using System.Collections.ObjectModel;

    public sealed class DisposableCollection<T> : Collection<T>, IDisposable
        where T : IDisposable
    {
        private bool _disposed;

        public void Dispose()
        {
            foreach (T item in this)
            {
                if (item != null)
                    item.Dispose();
            }

            Clear();
            _disposed = true;
            GC.SuppressFinalize(this);
        }

        protected override void ClearItems()
        {
            ThrowIfDisposed();
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            ThrowIfDisposed();
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            ThrowIfDisposed();
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            ThrowIfDisposed();
            base.SetItem(index, item);
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
