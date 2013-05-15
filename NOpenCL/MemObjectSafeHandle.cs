namespace NOpenCL
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class MemObjectSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public MemObjectSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseMemObject(handle);
            return result == ErrorCode.Success;
        }
    }
}
