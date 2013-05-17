namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class ContextSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ContextSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseContext(handle);
            return result == ErrorCode.Success;
        }
    }
}
