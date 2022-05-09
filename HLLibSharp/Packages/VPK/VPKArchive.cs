/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.VPK
{
    public sealed class VPKArchive
    {
        public Stream Stream { get; set; }

        public Mapping Mapping { get; set; }
    }
}
