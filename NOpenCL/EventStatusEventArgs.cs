/*
 * Copyright (c) 2013 Sam Harwell, Tunnel Vision Laboratories LLC
 * All rights reserved.
 */

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
