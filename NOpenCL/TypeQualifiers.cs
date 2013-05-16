namespace NOpenCL
{
    using System;

    [Flags]
    public enum TypeQualifiers : ulong
    {
        None = 0,
        Const = (1 << 0),
        Restrict = (1 << 1),
        Volatile = (1 << 2),
    }
}
