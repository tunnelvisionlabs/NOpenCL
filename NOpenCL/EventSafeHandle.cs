namespace NOpenCL
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class EventSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public EventSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseEvent(handle);
            return result == ErrorCode.Success;
        }
    }
}
