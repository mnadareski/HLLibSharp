/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;
using System.Text;

namespace HLLib.Packages.SGA
{
    public abstract class SGAHeaderBase
    {
        /// <summary>
        /// Total size of a SGAHeaderBase object
        /// </summary>
        public const int ObjectSize = 8 + 2 + 2;

        public string Signature { get; set; }

        public ushort MajorVersion { get; set; }

        public ushort MinorVersion { get; set; }

        protected static void Fill(SGAHeaderBase headerBase, byte[] data, ref int offset)
        {
            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return;

            headerBase.Signature = Encoding.ASCII.GetString(data, offset, 8); offset += 8;
            headerBase.MajorVersion = BitConverter.ToUInt16(data, offset); offset += 2;
            headerBase.MinorVersion = BitConverter.ToUInt16(data, offset); offset += 2;
        }
    }
}
