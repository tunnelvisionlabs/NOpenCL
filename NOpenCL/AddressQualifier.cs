// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// The address space qualifier used in a variable declaration.
    /// </summary>
    public enum AddressQualifier
    {
        /// <summary>
        /// The <c>__global</c> or <c>global</c> address space name is used to refer to
        /// memory objects (buffer or image objects) allocated from the global memory pool.
        /// </summary>
        Global = 0x119B,

        /// <summary>
        /// The <c>__local</c> or <c>local</c> address space name is used to describe
        /// variables that need to be allocated in local memory and are shared by all
        /// work-items of a work-group.
        /// </summary>
        Local = 0x119C,

        /// <summary>
        /// The <c>__constant</c> or <c>constant</c> address space name is used to describe
        /// variables allocated in global memory and which are accessed inside a kernel(s)
        /// as read-only variables.
        /// </summary>
        Constant = 0x119D,

        /// <summary>
        /// Variables inside a kernel function not declared with an address space qualifier,
        /// all variables inside non-kernel functions, and all function arguments are in the
        /// <c>__private</c> or <c>private</c> address space. Variables declared as pointers
        /// are considered to point to the <c>__private</c> address space if an address
        /// space qualifier is not specified.
        /// </summary>
        Private = 0x119E,
    }
}
