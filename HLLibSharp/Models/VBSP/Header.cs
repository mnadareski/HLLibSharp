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
    public sealed class Header
    {
        /// <summary>
        /// BSP file signature.
        /// </summary>
        public string Signature;

        /// <summary>
        /// BSP file version.
        /// </summary>
        public int Version;

        /// <summary>
        /// Lumps.
        /// </summary>
        public Lump[] Lumps;

        /// <summary>
        /// The map's revision (iteration, version) number.
        /// </summary>
        public int MapRevision;
    }
}
