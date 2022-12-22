/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Models.VBSP
{
    public sealed class LMPHeader
    {
        public int LumpOffset;

        public int LumpID;

        public int LumpVersion;

        public int LumpLength;

        public int MapRevision;
    }
}
