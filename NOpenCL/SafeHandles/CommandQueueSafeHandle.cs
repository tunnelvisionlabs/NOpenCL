namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class CommandQueueSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public CommandQueueSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseCommandQueue(handle);
            return result == ErrorCode.Success;
        }
    }
}
