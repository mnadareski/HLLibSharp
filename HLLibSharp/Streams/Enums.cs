/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Streams
{
    /// <summary>
    /// Internal stream type
    /// </summary>
    public enum StreamType
    {
        HL_STREAM_NONE = 0,
        HL_STREAM_FILE,
        HL_STREAM_GCF,
        HL_STREAM_MAPPING,
        HL_STREAM_MEMORY,
        HL_STREAM_PROC,
        HL_STREAM_NULL
    }
}
