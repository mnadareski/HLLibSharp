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
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.PAK
{
    /// <summary>
    /// Half-Life Package File
    /// </summary>
    public sealed class PAKFile : Package
    {
        #region Constants

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        /// <summary>
        /// View representing directory item data
        /// </summary>
        private View DirectoryItemView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized directory header data
        /// </summary>
        public PAKHeader Header { get; private set; }

        /// <summary>
        /// Deserialized directory items data
        /// </summary>
        public PAKDirectoryItem[] DirectoryItems { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public PAKFile() : base()
        {
            HeaderView = null;
            DirectoryItemView = null;
            Header = null;
            DirectoryItems = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~PAKFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_PAK;

        /// <inheritdoc/>
        public override string Extension => "pak";

        /// <inheritdoc/>
        public override string Description => "Half-Life Package File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);

            long itemCount = Header.DirectoryLength / PAKDirectoryItem.ObjectSize;

            // Loop through each file in the PAK file.
            for (uint i = 0; i < itemCount; i++)
            {
                string fileName = DirectoryItems[i].ItemName;

                // Check if we have just a file, or if the file has directories we need to create.
                if (fileName.IndexOf('/') == 0 && fileName.IndexOf('\\') == 0)
                {
                    root.AddFile(fileName, i);
                }
                else
                {
                    // Tokenize the file path and create the directories.
                    DirectoryFolder insertFolder = root;

                    string temp = string.Empty;
                    string[] token = fileName.Split('/', '\\');
                    int index = 0;
                    while (index < token.Length)
                    {
                        temp = token[index++];
                        if (index < token.Length)
                        {
                            // Check if the directory exists.
                            DirectoryItem item = insertFolder.GetItem(temp);
                            if (item == null || item.ItemType == DirectoryItemType.HL_ITEM_FILE)
                            {
                                // It doesn't, create it.
                                insertFolder = insertFolder.AddFolder(temp);
                            }
                            else
                            {
                                // It does, use it.
                                insertFolder = (DirectoryFolder)item;
                            }
                        }
                    }

                    // The file name is the last token, add it.
                    insertFolder.AddFile(temp, i);
                }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (PAKHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, PAKHeader.ObjectSize))
                return false;

            int pointer = 0;
            Header = PAKHeader.Create(HeaderView.ViewData, ref pointer);
            if (Header.Signature != "PACK")
            {
                Console.WriteLine("Invalid file: the file's signature does not match.");
                return false;
            }

            if (!Mapping.Map(ref DirectoryItemView, Header.DirectoryOffset, (int)Header.DirectoryLength))
                return false;

            long itemCount = Header.DirectoryLength / PAKDirectoryItem.ObjectSize;
            byte[] directoryItemViewBytes = DirectoryItemView.ViewData;
            pointer = 0;
            DirectoryItems = new PAKDirectoryItem[itemCount];
            for (int i = 0; i < itemCount; i++)
            {
                DirectoryItems[i] = PAKDirectoryItem.Create(directoryItemViewBytes, ref pointer);
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            DirectoryItems = null;
            Mapping.Unmap(DirectoryItemView);

            Header = null;
            Mapping.Unmap(HeaderView);
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = null;
            return false;
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = null;
            return false;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryItems[file.ID].ItemLength;
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryItems[file.ID].ItemLength;
            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            PAKDirectoryItem directoryItem = DirectoryItems[file.ID];
            stream = new MappingStream(Mapping, directoryItem.ItemOffset, directoryItem.ItemLength);
            return true;
        }

        #endregion
    }
}
