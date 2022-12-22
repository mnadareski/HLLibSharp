/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.VPK
{
    public sealed class DirectoryItem
    {
        public string Extension;

        public string Path;

        public string Name;

        public DirectoryEntry DirectoryEntry;

        public byte[] PreloadData;
    }
}
