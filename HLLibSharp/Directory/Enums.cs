/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System;

namespace HLLib.Directory
{
    /// <summary>
    /// Package internal item type
    /// </summary>
    public enum DirectoryItemType
    {
        HL_ITEM_NONE = 0,
        HL_ITEM_FOLDER,
        HL_ITEM_FILE
    }

    /// <summary>
    /// File mode flags that can be translated to native file modes
    /// </summary>
    [Flags]
    public enum FileModeFlags
    {
        HL_MODE_INVALID = 0x00,
        HL_MODE_READ = 0x01,
        HL_MODE_WRITE = 0x02,
        HL_MODE_CREATE = 0x04,
        HL_MODE_VOLATILE = 0x08,
        HL_MODE_NO_FILEMAPPING = 0x10,
        HL_MODE_QUICK_FILEMAPPING = 0x20
    }

    /// <summary>
    /// Item search flags
    /// </summary>
    [Flags]
    public enum FindType
    {
        HL_FIND_FILES = 0x01,
        HL_FIND_FOLDERS = 0x02,
        HL_FIND_NO_RECURSE = 0x04,
        HL_FIND_CASE_SENSITIVE = 0x08,
        HL_FIND_MODE_STRING = 0x10,
        HL_FIND_MODE_SUBSTRING = 0x20,
        HL_FIND_MODE_WILDCARD = 0x00,
        HL_FIND_ALL = HL_FIND_FILES | HL_FIND_FOLDERS
    }

    /// <summary>
    /// Sorting direction
    /// </summary>
    public enum SortOrder
    {
        HL_ORDER_ASCENDING = 0,
        HL_ORDER_DESCENDING
    }

    /// <summary>
    /// Sorting field
    /// </summary>
    public enum SortField
    {
        HL_FIELD_NAME = 0,
        HL_FIELD_SIZE
    }
}
