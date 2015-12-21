// Copyright (c) Tunnel Vision Laboratories, LLC. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace NOpenCL
{
    public enum FilterMode
    {
        /// <summary>
        ///
        /// </summary>
        /// <remarks>
        /// If the image addressing mode is <see cref="AddressingMode.Repeat"/>,
        /// the image element at location (<em>i</em>, <em>j</em>, <em>k</em>)
        /// becomes the image element value, with <em>i</em>, <em>j</em> and
        /// <em>k</em> computed as follows.
        ///
        /// <code>
        /// u = (s – floor(s)) * wt
        /// i = (int)floor(u)
        /// if (i > wt – 1)
        ///   i = i – wt
        ///
        /// v = (t – floor(t)) * ht
        /// j = (int)floor(v)
        /// if (j > ht – 1)
        ///   j = j – ht
        ///
        /// w = (r – floor(r)) * dt
        /// k = (int)floor(w)
        /// if (k > dt – 1)
        ///   k = k – dt
        /// </code>
        ///
        /// <para>If the image addressing mode is <see cref="AddressingMode.MirroredRepeat"/>,
        /// the image element at location (<em>i</em>, <em>j</em>, <em>k</em>)
        /// becomes the image element value, with <em>i</em>, <em>j</em> and
        /// <em>k</em> computed as follows.</para>
        ///
        /// <code>
        /// s' = 2.0f * rint(0.5f * s)
        /// s' = fabs(s – s')
        /// u = s' * wt
        /// i = (int)floor(u)
        /// i = min(i, wt – 1)
        ///
        /// t' = 2.0f * rint(0.5f * t)
        /// t' = fabs(t – t’)
        /// v = t' * ht
        /// j = (int)floor(v)
        /// j = min(j, ht – 1)
        ///
        /// r' = 2.0f * rint(0.5f * r)
        /// r' = fabs(r – r')
        /// w = r’ * dt
        /// k = (int)floor(w)
        /// k = min(k, dt – 1)
        /// </code>
        ///
        /// <para>Otherwise, the image element in the image that is nearest (in
        /// Manhattan distance) to that specified by (u,v,w) is obtained.</para>
        /// </remarks>
        Nearest = 0x1140,

        Linear = 0x1141,
    }
}
