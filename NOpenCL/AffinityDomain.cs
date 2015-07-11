// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    using System;

    /// <summary>
    /// Defines the cache parts of the cache hierarchy which are used for splitting a device into smaller
    /// aggregate devices containing one or more compute units that all share part of a cache hierarchy.
    /// </summary>
    /// <seealso cref="Device.PartitionByAffinityDomain"/>
    [Flags]
    public enum AffinityDomain : ulong
    {
        None = 0,

        /// <summary>
        /// Split the device into sub-devices comprised of compute units that share a NUMA node.
        /// </summary>
        Numa = 1 << 0,

        /// <summary>
        /// Split the device into sub-devices comprised of compute units that share a level 4 data cache.
        /// </summary>
        L4Cache = 1 << 1,

        /// <summary>
        /// Split the device into sub-devices comprised of compute units that share a level 3 data cache.
        /// </summary>
        L3Cache = 1 << 2,

        /// <summary>
        /// Split the device into sub-devices comprised of compute units that share a level 2 data cache.
        /// </summary>
        L2Cache = 1 << 3,

        /// <summary>
        /// Split the device into sub-devices comprised of compute units that share a level 1 data cache.
        /// </summary>
        L1Cache = 1 << 4,

        /// <summary>
        /// Split the device along the next partitionable affinity domain. The implementation shall find
        /// the first level along which the device or sub-device may be further subdivided in the order
        /// NUMA, L4, L3, L2, L1, and partition the device into sub-devices comprised of compute units
        /// that share memory subsystems at this level.
        /// </summary>
        NextPartitionable = 1 << 5,
    }
}
