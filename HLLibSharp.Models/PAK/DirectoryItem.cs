/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.PAK
{
    public sealed class DirectoryItem
    {
        /// <summary>
        /// Item Name
        /// </summary>
        public string ItemName;

        /// <summary>
        /// Item Offset
        /// </summary>
        public uint ItemOffset;

        /// <summary>
        /// Item Length
        /// </summary>
        public uint ItemLength;
    }
}
