/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Models.Common;

// TODO: Include zlib to sync with newest version
namespace HLLib.Models.ZIP
{
    /// <summary>
    /// Zip File
    /// </summary>
    public sealed class File
    {
        /// <summary>
        /// Deserialized end of central directory record data
        /// </summary>
        public ZIPEndOfCentralDirectoryRecord EndOfCentralDirectoryRecord { get; set; }
    }
}
