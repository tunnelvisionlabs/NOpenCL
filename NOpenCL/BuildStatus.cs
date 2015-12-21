// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// Returns the build, compile, or link status, whichever was performed last
    /// on the specified program and device.
    /// </summary>
    /// <seealso cref="O:NOpenCL.Program.Build"/>
    /// <seealso cref="Program.Compile"/>
    /// <seealso cref="Program.Link"/>
    public enum BuildStatus
    {
        /// <summary>
        /// The build was successful.
        /// </summary>
        Success = 0,

        /// <summary>
        /// No build, compile, or link has been performed on the specified program and device.
        /// </summary>
        None = -1,

        /// <summary>
        /// The build generated an error.
        /// </summary>
        Error = -2,

        /// <summary>
        /// The build has not finished.
        /// </summary>
        InProgress = -3,
    }
}
