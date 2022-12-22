/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.BSP
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Lumps
        /// </summary>
        public Lump[] Lumps { get; set; }

        /// <summary>
        /// Texture header data
        /// </summary>
        public TextureHeader TextureHeader { get; set; }
    }
}