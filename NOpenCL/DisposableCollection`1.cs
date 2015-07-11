// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    public sealed class DisposableCollection<T> : Collection<T>, IDisposable
        where T : IDisposable
    {
        private readonly bool _reverseOrder;
        private bool _disposed;

        public DisposableCollection()
            : this(true)
        {
        }

        public DisposableCollection(bool reverseOrder)
        {
            _reverseOrder = reverseOrder;
        }

        public void Dispose()
        {
            IEnumerable<T> enumerable = _reverseOrder ? this.Reverse() : this;
            foreach (T item in enumerable)
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
