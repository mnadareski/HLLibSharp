/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Packages.SGA
{
    public class SGASection<T>
    {
        /// <summary>
        /// Total size of a SGAHeaderBase object
        /// </summary>
        /// <remarks>
        /// This does not account for the typed variables
        /// </remarks>
        public const int ObjectSize = 64 + 64;

        public string Alias { get; set; }

        public string Name { get; set; }

        public T FolderStartIndex { get; set; }

        public T FolderEndIndex { get; set; }

        public T FileStartIndex { get; set; }

        public T FileEndIndex { get; set; }

        public T FolderRootIndex { get; set; }
    }
}
