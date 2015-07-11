// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;

    public class EventStatusEventArgs : EventArgs
    {
        private readonly ExecutionStatus _status;

        public EventStatusEventArgs(ExecutionStatus status)
        {
            _status = status;
        }

        public ExecutionStatus Status
        {
            get
            {
                return _status;
            }
        }
    }
}
