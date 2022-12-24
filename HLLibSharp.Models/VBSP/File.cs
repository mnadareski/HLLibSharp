/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Models.Common;

namespace HLLib.Models.VBSP
{
    /// <summary>
    /// Half-Life 2 Level
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public Header Header { get; set; }

        /// <summary>
        /// Deserialized end of central directory record data
        /// </summary>
        public ZIPEndOfCentralDirectoryRecord EndOfCentralDirectoryRecord { get; set; }
    }
}
