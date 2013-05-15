namespace NOpenCL
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class SamplerSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public SamplerSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseSampler(handle);
            return result == ErrorCode.Success;
        }
    }
}
