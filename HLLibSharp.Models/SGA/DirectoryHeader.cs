/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.SGA
{
    public class DirectoryHeader<T>
    {
        public uint SectionOffset;

        public T SectionCount;

        public uint FolderOffset;

        public T FolderCount;

        public uint FileOffset;

        public T FileCount;

        public uint StringTableOffset;

        public T StringTableCount;
    }
}
