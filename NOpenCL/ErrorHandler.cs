namespace NOpenCL
{
    using System;
    using ErrorCode = NOpenCL.UnsafeNativeMethods.ErrorCode;

    internal static class ErrorHandler
    {
        internal static void ThrowOnFailure(ErrorCode errorCode)
        {
            if (errorCode >= 0)
                return;

            switch (errorCode)
            {
            case ErrorCode.OutOfHostMemory:
                throw new OutOfMemoryException("There is insufficient memory to satisfy the request.");

            case ErrorCode.InvalidValue:
                throw new ArgumentException();

            default:
                throw new Exception(string.Format("Unknown error code: {0}", errorCode));
            }
        }
    }
}
