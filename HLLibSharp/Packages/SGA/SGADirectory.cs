/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Directory;
using HLLib.Streams;

namespace HLLib.Packages.SGA
{
    public abstract class SGADirectory
    {
        #region Mappings

        /// <summary>
        /// Per-directory creation of a root directory object, if possible
        /// </summary>
        /// <returns>Root directory folder, null on error</returns>
        protected virtual DirectoryFolder CreateRoot() => null;

        /// <summary>
        /// Internally map all data structures
        /// </summary>
        /// <returns>True if all structures could be mapped, false otherwise</returns>
        protected virtual bool MapDataStructures() => false;

        /// <summary>
        /// Internally unmap all data structures
        /// </summary>
        protected virtual void UnmapDataStructures() { }

        #endregion

        #region Attributes

        /// <summary>
        /// Per-directory creation of an item attribute object, if possible
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        protected virtual bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = null;
            return false;
        }

        #endregion

        #region File Extraction Check

        /// <summary>
        /// Per-directory implementation of file extraction checks
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="extractable">True if the file can be extracted, false otherwise</param>
        /// <returns>True if the extractability could be derived, false otherwise</returns>
        protected virtual bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            extractable = false;
            return false;
        }

        #endregion

        #region File Validation

        /// <summary>
        /// Per-package implementation of file extraction checks
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="validation">Output validation value</param>
        /// <returns>True if the validaiton could be performed, false otherwise</returns>
        protected virtual bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            validation = Validation.HL_VALIDATES_ERROR;
            return false;
        }

        #endregion

        #region File Size

        /// <summary>
        /// Per-directory implementation of internal size check
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        protected virtual bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = default;
            return false;
        }

        /// <summary>
        /// Per-directory implementation of extracted size check
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        protected virtual bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = default;
            return false;
        }

        #endregion

        #region Streams

        /// <summary>
        /// Internal implementation of stream creation
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="readEncrypted">True to read encrypted files, false otherwise</param>
        /// <param name="stream">Output stream</param>
        /// <returns>True if the stream could be created, false otherwise</returns>
        protected virtual bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            return false;
        }

        #endregion
    }
}
