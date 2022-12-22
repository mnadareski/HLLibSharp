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
    public class Section<T>
    {
        public string Alias;

        public string Name;

        public T FolderStartIndex;

        public T FolderEndIndex;

        public T FileStartIndex;

        public T FileEndIndex;

        public T FolderRootIndex;
    }
}
