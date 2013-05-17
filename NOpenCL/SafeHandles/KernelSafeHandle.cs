namespace NOpenCL.SafeHandles
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class KernelSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public KernelSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseKernel(handle);
            return result == ErrorCode.Success;
        }
    }
}
