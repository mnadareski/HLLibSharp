/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Packages
{
    /// <summary>
    /// Internal attribute type
    /// </summary>
    public enum AttributeType
    {
        HL_ATTRIBUTE_INVALID = 0,
        HL_ATTRIBUTE_BOOLEAN,
        HL_ATTRIBUTE_INTEGER,
        HL_ATTRIBUTE_UNSIGNED_INTEGER,
        HL_ATTRIBUTE_FLOAT,
        HL_ATTRIBUTE_STRING
    }

    /// <summary>
    /// Internal package attributes
    /// </summary>
    public enum PackageAttributeType
    {
        HL_BSP_PACKAGE_VERSION = 0,

        HL_BSP_ITEM_WIDTH = 0,
        HL_BSP_ITEM_HEIGHT,
        HL_BSP_ITEM_PALETTE_ENTRIES,

        HL_GCF_PACKAGE_VERSION = 0,
        HL_GCF_PACKAGE_ID,
        HL_GCF_PACKAGE_ALLOCATED_BLOCKS,
        HL_GCF_PACKAGE_USED_BLOCKS,
        HL_GCF_PACKAGE_BLOCK_LENGTH,
        HL_GCF_PACKAGE_LAST_VERSION_PLAYED,

        HL_GCF_ITEM_ENCRYPTED = 0,
        HL_GCF_ITEM_COPY_LOCAL,
        HL_GCF_ITEM_OVERWRITE_LOCAL,
        HL_GCF_ITEM_BACKUP_LOCAL,
        HL_GCF_ITEM_FLAGS,
        HL_GCF_ITEM_FRAGMENTATION,

        HL_NCF_PACKAGE_VERSION = 0,
        HL_NCF_PACKAGE_ID,
        HL_NCF_PACKAGE_LAST_VERSION_PLAYED,

        HL_NCF_ITEM_ENCRYPTED = 0,
        HL_NCF_ITEM_COPY_LOCAL,
        HL_NCF_ITEM_OVERWRITE_LOCAL,
        HL_NCF_ITEM_BACKUP_LOCAL,
        HL_NCF_ITEM_FLAGS,

        HL_SGA_PACKAGE_VERSION_MAJOR = 0,
        HL_SGA_PACKAGE_VERSION_MINOR,
        HL_SGA_PACKAGE_MD5_FILE,
        HL_SGA_PACKAGE_NAME,
        HL_SGA_PACKAGE_MD5_HEADER,

        HL_SGA_ITEM_SECTION_ALIAS = 0,
        HL_SGA_ITEM_SECTION_NAME,
        HL_SGA_ITEM_MODIFIED,
        HL_SGA_ITEM_TYPE,
        HL_SGA_ITEM_CRC,
        HL_SGA_ITEM_VERIFICATION,

        HL_VBSP_PACKAGE_VERSION = 0,
        HL_VBSP_PACKAGE_MAP_REVISION,

        HL_VBSP_ITEM_VERSION = 0,
        HL_VBSP_ITEM_FOUR_CC,
        HL_VBSP_ZIP_PACKAGE_DISK,
        HL_VBSP_ZIP_PACKAGE_COMMENT,
        HL_VBSP_ZIP_ITEM_CREATE_VERSION,
        HL_VBSP_ZIP_ITEM_EXTRACT_VERSION,
        HL_VBSP_ZIP_ITEM_FLAGS,
        HL_VBSP_ZIP_ITEM_COMPRESSION_METHOD,
        HL_VBSP_ZIP_ITEM_CRC,
        HL_VBSP_ZIP_ITEM_DISK,
        HL_VBSP_ZIP_ITEM_COMMENT,

        HL_VPK_PACKAGE_Archives = 0,
        HL_VPK_PACKAGE_Version,

        HL_VPK_ITEM_PRELOAD_BYTES = 0,
        HL_VPK_ITEM_ARCHIVE,
        HL_VPK_ITEM_CRC,

        HL_WAD_PACKAGE_VERSION = 0,

        HL_WAD_ITEM_WIDTH = 0,
        HL_WAD_ITEM_HEIGHT,
        HL_WAD_ITEM_PALETTE_ENTRIES,
        HL_WAD_ITEM_MIPMAPS,
        HL_WAD_ITEM_COMPRESSED,
        HL_WAD_ITEM_TYPE,

        HL_XZP_PACKAGE_VERSION = 0,
        HL_XZP_PACKAGE_PRELOAD_BYTES,

        HL_XZP_ITEM_CREATED = 0,
        HL_XZP_ITEM_PRELOAD_BYTES,

        HL_ZIP_PACKAGE_DISK = 0,
        HL_ZIP_PACKAGE_COMMENT,

        HL_ZIP_ITEM_CREATE_VERSION = 0,
        HL_ZIP_ITEM_EXTRACT_VERSION,
        HL_ZIP_ITEM_FLAGS,
        HL_ZIP_ITEM_COMPRESSION_METHOD,
        HL_ZIP_ITEM_CRC,
        HL_ZIP_ITEM_DISK,
        HL_ZIP_ITEM_COMMENT,
    }

    /// <summary>
    /// Internal package type
    /// </summary>
    public enum PackageType
    {
        HL_PACKAGE_NONE = 0,
        HL_PACKAGE_BSP,
        HL_PACKAGE_GCF,
        HL_PACKAGE_PAK,
        HL_PACKAGE_VBSP,
        HL_PACKAGE_WAD,
        HL_PACKAGE_XZP,
        HL_PACKAGE_ZIP,
        HL_PACKAGE_NCF,
        HL_PACKAGE_VPK,
        HL_PACKAGE_SGA
    }

    /// <summary>
    /// Internal file verification type for SGA packages
    /// </summary>
    public enum SGAFileVerification
    {
        VERIFICATION_NONE,
        VERIFICATION_CRC,
        VERIFICATION_CRC_BLOCKS,
        VERIFICATION_MD5_BLOCKS,
        VERIFICATION_SHA1_BLOCKS,
        VERIFICATION_COUNT,
    }

    /// <summary>
    /// Directory item validation value
    /// </summary>
    public enum Validation
    {
        HL_VALIDATES_OK = 0,
        HL_VALIDATES_ASSUMED_OK,
        HL_VALIDATES_INCOMPLETE,
        HL_VALIDATES_CORRUPT,
        HL_VALIDATES_CANCELED,
        HL_VALIDATES_ERROR
    }
}
