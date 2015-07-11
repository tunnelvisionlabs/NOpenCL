﻿// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    /// <summary>
    /// The program binary type for a device.
    /// </summary>
    public enum BinaryType
    {
        /// <summary>
        /// There is no binary associated with the device.
        /// </summary>
        None = 0,

        /// <summary>
        /// A compiled binary is associated with the device. This is the case if a program
        /// was created using <see cref="Context.CreateProgramWithSource"/> and compiled
        /// using <see cref="Program.Compile()"/> or a compiled binary is loaded using
        /// <see cref="Context.CreateProgramWithBinary"/>.
        /// </summary>
        CompiledObject = 0x0001,

        /// <summary>
        /// A library binary is associated with the device. This is the case if a program
        /// was created by <see cref="Program.Link"/> which is called with the
        /// <c>–createlibrary</c> link option or if a library binary is loaded using
        /// <see cref="Context.CreateProgramWithBinary"/>.
        /// </summary>
        Library = 0x0002,

        /// <summary>
        /// An executable binary is associated with the device. This is the case if a
        /// program was created by <see cref="Program.Link"/> without the <c>–createlibrary</c>
        /// link option or the program was created by <see cref="Program.Build()"/>
        /// or an executable binary is loaded using <see cref="Context.CreateProgramWithBinary"/>.
        /// </summary>
        Executable = 0x0004,
    }
}
