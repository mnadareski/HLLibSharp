/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 *
 * Adler32() and CRC32() based on zlib 1.2.3:
 *
 * zlib.h -- interface of the 'zlib' general purpose compression library
 * version 1.2.3, July 18th, 2005
 *
 * Copyright (C) 1995-2005 Jean-loup Gailly and Mark Adler
 *
 * This software is provided 'as-is', without any express or implied
 * warranty.  In no event will the authors be held liable for any damages
 * arising from the use of this software.
 *
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would be
 *    appreciated but is not required.
 * 2. Altered source versions must be plainly marked as such, and must not be
 *    misrepresented as being the original software.
 * 3. This notice may not be removed or altered from any source distribution.
 *
 * Jean-loup Gailly jloup@gzip.org
 * Mark Adler madler@alumni.caltech.edu
 */

using System;

namespace HLLib.Checksums
{
    public sealed class CRC32Checksum : Checksum
    {
        /// <inheritdoc/>
        public override uint DigestSize => 4;

        /// <summary>
        /// Internal CRC32 checksum
        /// </summary>
        private uint InternalChecksum;

        /// <summary>
        /// Constructor
        /// </summary>
        public CRC32Checksum() => Initialize();

        /// <inheritdoc/>
        public override void Initialize()
        {
            InternalChecksum = 0;
        }

        /// <inheritdoc/>
        public override void Update(byte[] buffer, int bufferSize)
        {
            InternalChecksum = CRC32(buffer, bufferSize, InternalChecksum);
        }

        /// <inheritdoc/>
        public override bool Finalize(out byte[] hash)
        {
            hash = BitConverter.GetBytes(InternalChecksum);
            return true;
        }
    }
}
