namespace NOpenCL
{
    using Microsoft.Win32.SafeHandles;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    public sealed class ProgramSafeHandle : SafeHandleZeroOrMinusOneIsInvalid
    {
        public ProgramSafeHandle()
            : base(true)
        {
        }

        protected override bool ReleaseHandle()
        {
            ErrorCode result = UnsafeNativeMethods.clReleaseProgram(handle);
            return result == ErrorCode.Success;
        }
    }
}
