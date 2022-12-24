/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.VBSP
{
    public sealed class Lump
    {
        public uint Offset;

        public uint Length;

        /// <summary>
        /// Default to zero.
        /// </summary>
        public uint Version;

        /// <summary>
        /// Default to (char)0, (char)0, (char)0, (char)0.
        /// </summary>
        public char[] FourCC;
    }
}
