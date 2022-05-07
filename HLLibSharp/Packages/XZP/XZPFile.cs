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
using System.Text;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.XZP
{
    public class XZPFile : Package
    {
        #region Constants

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version", "Preload Bytes" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Created", "Preload Bytes" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        /// <summary>
        /// View representing directory entry data
        /// </summary>
        private View DirectoryEntryView;

        /// <summary>
        /// View representing directory item data
        /// </summary>
        private View DirectoryItemView;

        /// <summary>
        /// View representing footer data
        /// </summary>
        private View FooterView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public XZPHeader Header { get; private set; }

        /// <summary>
        /// Deserialized directory entries data
        /// </summary>
        public XZPDirectoryEntry[] DirectoryEntries { get; private set; }

        /// <summary>
        /// Deserialized preload directory entries data
        /// </summary>
        public XZPDirectoryEntry[] PreloadDirectoryEntries { get; private set; }

        /// <summary>
        /// Deserialized preload directory mappings data
        /// </summary>
        public XZPDirectoryMapping[] PreloadDirectoryMappings { get; private set; }

        /// <summary>
        /// Deserialized directory items data
        /// </summary>
        public XZPDirectoryItem[] DirectoryItems { get; private set; }

        /// <summary>
        /// Deserialized footer data
        /// </summary>
        public XZPFooter Footer { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public XZPFile() : base()
        {
            HeaderView = null;
            DirectoryEntryView = null;
            DirectoryItemView = null;
            FooterView = null;

            Header = null;
            DirectoryEntries = null;
            PreloadDirectoryEntries = null;
            PreloadDirectoryMappings = null;
            DirectoryItems = null;
            Footer = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~XZPFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_XZP;

        /// <inheritdoc/>
        public override string Extension => "xzp";

        /// <inheritdoc/>
        public override string Description => "XBox Package File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);
            if (Header.DirectoryItemCount != 0)
            {
                // Loop through each file in the XZP file.
                for (uint i = 0; i < Header.DirectoryEntryCount; i++)
                {
                    // Find it's info (file name).
                    for (uint j = 0; j < Header.DirectoryItemCount; j++)
                    {
                        if (DirectoryEntries[i].FileNameCRC == DirectoryItems[j].FileNameCRC)
                        {
                            string fileName = Encoding.ASCII.GetString(DirectoryItemView.ViewData, (int)(DirectoryItems[j].NameOffset - Header.DirectoryItemOffset), 256);

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
                                int tokenPointer = 0;
                                while (tokenPointer < token.Length)
                                {
                                    temp = token[tokenPointer++];
                                    if (tokenPointer < token.Length)
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
                            break;
                        }
                    }
                }
            }
            else
            {
                // No file name information, just file name CRCs.
                for (uint i = 0; i < Header.DirectoryEntryCount; i++)
                {
                    string temp = BitConverter.ToString(BitConverter.GetBytes(DirectoryEntries[i].FileNameCRC)).Replace("-", string.Empty);
                    root.AddFile(temp, i);
                }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (XZPHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, XZPHeader.ObjectSize))
                return false;

            int pointer = 0;
            Header = XZPHeader.Create(HeaderView.ViewData, ref pointer);

            if (Header.Signature != "piZx")
            {
                Console.WriteLine("Invalid file: the file's header signature does not match.");
                return false;
            }

            if (Header.Version != 6)
            {
                Console.WriteLine($"Invalid XZP version (v{Header.Version}): you have a version of a XZP file that HLLib does not know how to read. Check for product updates.");
                return false;
            }

            if (Header.HeaderLength != XZPHeader.ObjectSize)
            {
                Console.WriteLine("Invalid file: the file's header size does not match.");
                return false;
            }

            if (!Mapping.Map(ref DirectoryEntryView, XZPHeader.ObjectSize, (int)(Header.PreloadBytes != 0 ? (Header.DirectoryEntryCount + Header.PreloadDirectoryEntryCount) * XZPDirectoryEntry.ObjectSize + Header.DirectoryEntryCount * XZPDirectoryMapping.ObjectSize : Header.DirectoryEntryCount * XZPDirectoryEntry.ObjectSize)))
                return false;

            pointer = 0;
            DirectoryEntries = new XZPDirectoryEntry[Header.DirectoryEntryCount];
            for (int i = 0; i < Header.DirectoryEntryCount; i++)
            {
                DirectoryEntries[i] = XZPDirectoryEntry.Create(DirectoryEntryView.ViewData, ref pointer);
            }

            if (Header.PreloadBytes != 0)
            {
                PreloadDirectoryEntries = new XZPDirectoryEntry[Header.PreloadDirectoryEntryCount];
                for (int i = 0; i < Header.PreloadDirectoryEntryCount; i++)
                {
                    PreloadDirectoryEntries[i] = XZPDirectoryEntry.Create(DirectoryEntryView.ViewData, ref pointer);
                }

                PreloadDirectoryMappings = new XZPDirectoryMapping[Header.PreloadDirectoryEntryCount];
                for (int i = 0; i < Header.PreloadDirectoryEntryCount; i++)
                {
                    PreloadDirectoryMappings[i] = XZPDirectoryMapping.Create(DirectoryEntryView.ViewData, ref pointer);
                }
            }
            else
            {
                PreloadDirectoryEntries = null;
                PreloadDirectoryMappings = null;
            }

            if (Header.DirectoryItemCount != 0)
            {
                if (!Mapping.Map(ref DirectoryItemView, Header.DirectoryItemOffset, (int)Header.DirectoryItemLength))
                    return false;

                pointer = 0;
                DirectoryItems = new XZPDirectoryItem[Header.DirectoryItemCount];
                for (int i = 0; i < Header.DirectoryItemCount; i++)
                {
                    DirectoryItems[i] = XZPDirectoryItem.Create(DirectoryItemView.ViewData, ref pointer);
                }
            }

            if (!Mapping.Map(ref FooterView, Mapping.MappingSize - XZPFooter.ObjectSize, XZPFooter.ObjectSize))
                return false;

            pointer = 0;
            Footer = XZPFooter.Create(FooterView.ViewData, ref pointer);

            if (Footer.Signature != "tFzX")
            {
                Console.WriteLine("Invalid file: the file's footer signature does not match.");
                return false;
            }

            if (Footer.FileLength != Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is not within mapping bounds.");
                return false;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            Footer = null;
            Mapping.Unmap(ref FooterView);

            DirectoryItems = null;
            Mapping.Unmap(ref DirectoryItemView);

            DirectoryEntries = null;
            PreloadDirectoryEntries = null;
            PreloadDirectoryMappings = null;
            Mapping.Unmap(ref DirectoryEntryView);

            Header = null;
            Mapping.Unmap(ref HeaderView);
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_XZP_PACKAGE_VERSION:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.Version, false);
                    return true;
                case PackageAttributeType.HL_XZP_PACKAGE_PRELOAD_BYTES:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.PreloadBytes, false);
                    return true;
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (item.ItemType)
            {
                case DirectoryItemType.HL_ITEM_FILE:
                    DirectoryFile file = (DirectoryFile)item;
                    XZPDirectoryEntry directoryEntry = DirectoryEntries[file.ID];
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_XZP_ITEM_CREATED:
                            for (uint i = 0; i < Header.DirectoryItemCount; i++)
                            {
                                if (DirectoryItems[i].FileNameCRC == directoryEntry.FileNameCRC)
                                {
                                    // TODO: This is supposed to be human-readable time
                                    attribute.SetString(ItemAttributeNames[(int)packageAttribute], DirectoryItems[i].TimeCreated.ToString());
                                    return true;
                                }
                            }

                            break;
                        case PackageAttributeType.HL_XZP_ITEM_PRELOAD_BYTES:
                            uint size = 0;
                            if (PreloadDirectoryMappings != null)
                            {
                                ushort uiIndex = PreloadDirectoryMappings[file.ID].PreloadDirectoryEntryIndex;
                                if (uiIndex != 0xffff && PreloadDirectoryEntries[uiIndex].FileNameCRC == directoryEntry.FileNameCRC)
                                    size = PreloadDirectoryEntries[uiIndex].EntryLength;
                            }

                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], size, false);
                            return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryEntries[file.ID].EntryLength;
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = (int)DirectoryEntries[file.ID].EntryLength;
            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            XZPDirectoryEntry directoryEntry = DirectoryEntries[file.ID];
            stream = new MappingStream(Mapping, directoryEntry.EntryOffset, directoryEntry.EntryLength);
            return true;
        }

        #endregion
    }
}
