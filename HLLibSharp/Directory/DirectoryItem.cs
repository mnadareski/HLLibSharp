/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System.Collections.Generic;
using HLLib.Packages;

namespace HLLib.Directory
{
    /// <summary>
    /// Item contained within a package
    /// </summary>
    public abstract class DirectoryItem
    {
        #region Fields

        /// <summary>
        /// Item name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Item ID
        /// </summary>
        public uint ID { get; private set; }

        /// <summary>
        /// Item data
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Source item package
        /// </summary>
        public Package Package { get; private set; }

        /// <summary>
        /// Parent folder, null means this item is root
        /// </summary>
        public DirectoryFolder Parent { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public DirectoryItem(string name, uint id, byte[] data, Package package, DirectoryFolder parent)
        {
            Name = name;
            ID = id;
            Data = data;
            Package = package;
            Parent = parent;
        }

        #region Descriptors

        /// <summary>
        /// Internal directory item type
        /// </summary>
        public abstract DirectoryItemType ItemType { get; }

        #endregion

        #region Paths

        /// <summary>
        /// Get the full path for an item, limited by size
        /// </summary>
        /// <param name="pathSize">Size to limit the length to, -1 for unlimited</param>
        /// <returns>Full path of the item</returns>
        public string GetPath(int pathSize = -1)
        {
            var temp = new List<string>();
            temp.Add(Name);

            DirectoryItem item = Parent;
            while (item != null)
            {
                temp.Add(item.Name);
                item = item.Parent;
            }

            temp.Reverse();
            string tempPath = string.Join(System.IO.Path.DirectorySeparatorChar.ToString(), temp);
            if (pathSize > 0)
                tempPath = tempPath.Substring(0, pathSize);

            return tempPath;
        }

        #endregion

        #region Extraction

        /// <summary>
        /// Exract this directory item to a local path
        /// </summary>
        /// <param name="path">Local path to write to</param>
        /// <param name="readEncrypted">True to read encrypted files, false otherwise</param>
        /// <param name="overwrite">True to overwrite existing files, false otherwise</param>
        /// <returns>True if the item could be extracted, false otherwise</returns>
        public virtual bool Extract(string path, bool readEncrypted = true, bool overwrite = true) => false;

        #endregion
    }
}