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
    public class SGADirectory4 : SGASpecializedDirectory<SGAHeader4, SGADirectoryHeader4, SGASection4, SGAFolder4, SGAFile4, ushort>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SGADirectory4(SGAFile file) : base(file) { }
    }
}
