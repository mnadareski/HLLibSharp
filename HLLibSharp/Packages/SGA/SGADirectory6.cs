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
    public class SGADirectory6 : SGASpecializedDirectory<SGAHeader6, SGADirectoryHeader5, SGASection5, SGAFolder5, SGAFile6, uint>
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public SGADirectory6(SGAFile file) : base(file) { }
    }
}
