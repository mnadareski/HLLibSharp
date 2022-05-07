/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;
using System.Text;

namespace HLLib.Packages.XZP
{
    public class XZPFooter
    {
        /// <summary>
        /// Total size of a XZPFooter object
        /// </summary>
        public const int ObjectSize = 4 + 4;

        public uint FileLength { get; set; }

        public string Signature { get; set; }

        public static XZPFooter Create(byte[] data, ref int offset)
        {
            XZPFooter footer = new XZPFooter();

            // Check to see if the data is valid
            if (data == null || data.Length < ObjectSize)
                return null;

            footer.FileLength = BitConverter.ToUInt32(data, offset); offset += 4;
            footer.Signature = Encoding.ASCII.GetString(data, offset, 4); offset += 4;

            return footer;
        }
    }
}
