/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

namespace NOpenCL
{
    /// <summary>
    /// Specifies the type of global memory cache supported.
    /// </summary>
    public enum CacheType
    {
        None = 0,
        ReadOnly = 1,
        ReadWrite = 2,
    }
}
