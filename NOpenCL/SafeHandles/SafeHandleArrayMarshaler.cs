// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL.SafeHandles
{
    using System;
    using System.Runtime.InteropServices;

    internal sealed class SafeHandleArrayMarshaler : ICustomMarshaler
    {
        private static readonly SafeHandleArrayMarshaler Instance = new SafeHandleArrayMarshaler();

        private SafeHandleArrayMarshaler()
        {
        }

        public static ICustomMarshaler GetInstance(string cookie)
        {
            return Instance;
        }

        public void CleanUpManagedData(object ManagedObj)
        {
            throw new NotSupportedException();
        }

        public void CleanUpNativeData(IntPtr pNativeData)
        {
            if (pNativeData == IntPtr.Zero)
                return;

            var managedHandle = GCHandle.FromIntPtr(Marshal.ReadIntPtr(pNativeData, -IntPtr.Size));
            var array = (SafeHandle[])managedHandle.Target;
            managedHandle.Free();

            for (int i = 0; i < array.Length; i++)
            {
                SafeHandle current = array[i];
                if (current == null)
                    continue;

                if (Marshal.ReadIntPtr(pNativeData, i * IntPtr.Size) != IntPtr.Zero)
                    array[i].DangerousRelease();
            }

            Marshal.FreeHGlobal(pNativeData - IntPtr.Size);
        }

        public int GetNativeDataSize()
        {
            return IntPtr.Size;
        }

        public IntPtr MarshalManagedToNative(object ManagedObj)
        {
            if (ManagedObj == null)
                return IntPtr.Zero;

            var array = (SafeHandle[])ManagedObj;
            int i = 0;
            bool success = false;
            try
            {
                for (i = 0; i < array.Length; success = false, i++)
                {
                    SafeHandle current = array[i];
                    if (current != null && !current.IsClosed && !current.IsInvalid)
                        current.DangerousAddRef(ref success);
                }

                IntPtr result = Marshal.AllocHGlobal((array.Length + 1) * IntPtr.Size);
                Marshal.WriteIntPtr(result, 0, GCHandle.ToIntPtr(GCHandle.Alloc(array, GCHandleType.Normal)));
                for (int j = 0; j < array.Length; j++)
                {
                    SafeHandle current = array[j];
                    if (current == null || current.IsClosed || current.IsInvalid)
                    {
                        // the memory for this element was initialized to null by AllocHGlobal
                        continue;
                    }

                    Marshal.WriteIntPtr(result, (j + 1) * IntPtr.Size, current.DangerousGetHandle());
                }

                return result + IntPtr.Size;
            }
            catch
            {
                int total = success ? i + 1 : i;
                for (int j = 0; j < total; j++)
                {
                    SafeHandle current = array[j];
                    if (current != null)
                        current.DangerousRelease();
                }

                throw;
            }
        }

        public object MarshalNativeToManaged(IntPtr pNativeData)
        {
            throw new NotSupportedException();
        }
    }
}
