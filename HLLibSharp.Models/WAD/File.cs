/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.WAD
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Deserialized header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Deserialized lumps data
        /// </summary>
        public Lump[] Lumps { get; set; }

        /// <summary>
        /// Deserialized lump infos data
        /// </summary>
        public LumpInfo[] LumpInfos { get; set; }
    }
}
