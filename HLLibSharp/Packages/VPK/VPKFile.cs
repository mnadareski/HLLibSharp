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
using System.Collections.Generic;
using System.Text;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages.VPK
{
    /// <summary>
    /// Valve Package File
    /// </summary>
    public sealed class VPKFile : Package
    {
        #region Constants

        /// <summary>
        /// VPK header signature as an integer
        /// </summary>
        public const int HL_VPK_SIGNATURE = 0x55aa1234;

        /// <summary>
        /// Index indicating that there is no archive
        /// </summary>
        public const int HL_VPK_NO_ARCHIVE = 0x7fff;

        /// <summary>
        /// Length of a VPK checksum in bytes
        /// </summary>
        public const int HL_VPK_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Archives", "Version" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Preload Bytes", "Archive", "CRC" };

        #endregion

        #region Views

        /// <summary>
        /// View representing the data
        /// </summary>
        private View View;

        #endregion

        #region Fields

        /// <summary>
        /// Number of internal archives
        /// </summary>
        public uint ArchiveCount { get; private set; }

        /// <summary>
        /// Deserialized internal archives data
        /// </summary>
        public VPKArchive[] Archives { get; private set; }

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public VPKHeader Header { get; private set; }

        /// <summary>
        /// Deserialized directory items data
        /// </summary>
        public List<VPKDirectoryItem> DirectoryItems { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public VPKFile() : base()
        {
            View = null;

            ArchiveCount = 0;
            Archives = null;
            Header = null;
            DirectoryItems = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~VPKFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_VPK;

        /// <inheritdoc/>
        public override string Extension => "vpk";

        /// <inheritdoc/>
        public override string Description => "Valve Package File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);

            string lastPath = null;
            DirectoryFolder lastInsertFolder = null;

            // Loop through each file in the VPK file.
            foreach (VPKDirectoryItem directoryItem in DirectoryItems)
            {
                DirectoryFolder insertFolder;
                if (directoryItem.Path == lastPath)
                {
                    insertFolder = lastInsertFolder;
                }
                else
                {
                    insertFolder = root;
                    if (!string.IsNullOrWhiteSpace(directoryItem.Path))
                    {
                        // Tokenize the file path and create the directories.
                        string path = directoryItem.Path;
                        string[] token = path.Split('/', '\\');
                        int index = 0;
                        while (index < token.Length)
                        {
                            // Check if the directory exists.
                            DirectoryItem item = insertFolder.GetItem(token[index]);
                            if (item == null || item.ItemType == DirectoryItemType.HL_ITEM_FILE)
                            {
                                // It doesn't, create it.
                                insertFolder = insertFolder.AddFolder(token[index]);
                            }
                            else
                            {
                                // It does, use it.
                                insertFolder = (DirectoryFolder)item;
                            }

                            index++;
                        }
                    }

                    lastPath = directoryItem.Path;
                    lastInsertFolder = insertFolder;
                }

                string fileName = $"{directoryItem.Name}.{directoryItem.Extension}";
                unchecked { insertFolder.AddFile(fileName, (uint)-1, directoryItem.Serialize()); }
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (!Mapping.Map(ref View, 0, (int)Mapping.MappingSize))
                return false;

            DirectoryItems = new List<VPKDirectoryItem>();

            byte[] viewData = View.ViewData;
            int viewDataStart = 0;
            int viewDataEnd = View.Length;
            int viewDirectoryDataEnd = viewDataEnd;

            if (VPKHeader.ObjectSize > viewDataEnd)
            {
                Console.WriteLine("Invalid file: The file map is not within mapping bounds.");
                return false;
            }

            int pointer = 0;
            Header = VPKHeader.Create(viewData, ref pointer);

            if (Header.Signature != HL_VPK_SIGNATURE)
            {
                // The original version had no signature.
                Header = null;
            }
            else
            {
                viewDataStart += VPKHeader.ObjectSize;
                viewDirectoryDataEnd = (int)(viewDataStart + Header.DirectoryLength);
            }

            while (viewDataStart != viewDirectoryDataEnd)
            {
                if (!MapString(viewData, ref viewDataStart, viewDirectoryDataEnd, out int extension))
                    return false;

                string extensionString = Encoding.ASCII.GetString(viewData, extension, viewDataStart - extension);
                if (viewData[extension] == '\0')
                    break;

                while (true)
                {
                    if (!MapString(viewData, ref viewDataStart, viewDirectoryDataEnd, out int path))
                        return false;

                    string pathString = Encoding.ASCII.GetString(viewData, path, viewDataStart - path);
                    if (viewData[path] == '\0')
                        break;

                    while (true)
                    {
                        if (!MapString(viewData, ref viewDataStart, viewDirectoryDataEnd, out int name))
                            return false;

                        string nameString = Encoding.ASCII.GetString(viewData, name, viewDataStart - name);
                        if (viewData[name] == '\0')
                            break;

                        if (viewDataStart + VPKDirectoryEntry.ObjectSize > viewDirectoryDataEnd)
                        {
                            Console.WriteLine("Invalid file: The file map is not within mapping bounds.");
                            return false;
                        }

                        pointer = viewDataStart;
                        VPKDirectoryEntry directoryEntry = VPKDirectoryEntry.Create(viewData, ref pointer);
                        viewDataStart += VPKDirectoryEntry.ObjectSize;

                        int preloadDataPointer = -1;
                        if (directoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
                        {
                            if (directoryEntry.EntryLength > 0)
                            {
                                if (viewDirectoryDataEnd + directoryEntry.EntryOffset + directoryEntry.EntryLength <= viewDataEnd)
                                    preloadDataPointer = (int)(viewDirectoryDataEnd + directoryEntry.EntryOffset);
                            }
                        }
                        else
                        {
                            if (directoryEntry.PreloadBytes > 0)
                            {
                                if (viewDataStart + directoryEntry.PreloadBytes > viewDirectoryDataEnd)
                                {
                                    Console.WriteLine("Invalid file: The file map is not within mapping bounds.");
                                    return false;
                                }

                                preloadDataPointer = viewDataStart;
                                viewDataStart += directoryEntry.PreloadBytes;
                            }

                            if ((uint)directoryEntry.ArchiveIndex + 1 > ArchiveCount)
                                ArchiveCount = (uint)directoryEntry.ArchiveIndex + 1;
                        }

                        byte[] preloadData = null;
                        if (preloadDataPointer > -1)
                        {
                            preloadData = new byte[viewDirectoryDataEnd - viewDataEnd];
                            Array.Copy(viewData, viewDataStart, preloadData, 0, preloadData.Length);
                        }

                        DirectoryItems.Add(new VPKDirectoryItem(extensionString, pathString, nameString, directoryEntry, preloadData));
                    }
                }
            }

            string fileName = Mapping.FileName;
            if (ArchiveCount > 0 && fileName != null)
            {
                int extensionPointer = fileName.IndexOf('.');
                if (extensionPointer != 0 && fileName.Length - extensionPointer > 3 && fileName.Substring(extensionPointer, 3) != "dir")
                {
                    // We need 5 digits to print a short, but we already have 3 for dir.
                    string archiveFileName = fileName;

                    Archives = new VPKArchive[ArchiveCount];
                    for (uint i = 0; i < ArchiveCount; i++)
                    {
                        archiveFileName += $"{i.ToString().PadLeft(3, '0')}.{fileName.Substring(extensionPointer, 3)}";
                        if (Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_NO_FILEMAPPING))
                        {
                            Archives[i].Stream = new FileStream(archiveFileName);
                            Archives[i].Mapping = new StreamMapping(Archives[i].Stream);
                            if (!Archives[i].Mapping.Open(Mapping.FileMode, overwrite: true))
                            {
                                Archives[i].Mapping.Close();
                                Archives[i].Mapping = null;

                                Archives[i].Stream.Close();
                                Archives[i].Stream = null;
                            }
                        }
                        else
                        {
                            Archives[i].Mapping = new FileMapping(archiveFileName);
                            if (!Archives[i].Mapping.Open(Mapping.FileMode, overwrite: true))
                            {
                                Archives[i].Mapping.Close();
                                Archives[i].Mapping = null;
                            }
                        }
                    }
                }
            }

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            if (Archives != null)
            {
                for (uint i = 0; i < ArchiveCount; i++)
                {
                    if (Archives[i].Mapping != null)
                        Archives[i].Mapping.Close();

                    if (Archives[i].Stream != null)
                        Archives[i].Stream.Close();
                }
            }

            ArchiveCount = 0;
            Archives = null;

            Header = null;
            if (DirectoryItems != null)
                DirectoryItems = null;

            Mapping.Unmap(ref View);
        }

        /// <summary>
        /// Find a single string in the view data
        /// </summary>
        /// <param name="viewData">View data to search through</param>
        /// <param name="viewDataStart">Start of where to search in the view</param>
        /// <param name="viewDirectoryDataEnd">End of the directory data</param>
        /// <param name="viewDataEnd">Output value representing the end of the string</param>
        /// <returns>True if the string could be found, false otherwise</returns>
        private bool MapString(byte[] viewData, ref int viewDataStart, int viewDirectoryDataEnd, out int viewDataEnd)
        {
            viewDataEnd = viewDataStart;
            while (true)
            {
                if (viewDataStart == viewDirectoryDataEnd)
                {
                    Console.WriteLine("Invalid file: Mapping bounds exceeded while searching for string null terminator.");
                    return false;
                }

                bool mapped = viewData[viewDataStart++] == '\0';
                if (mapped)
                    return true;
            }
        }

        #endregion

        #region Attributes

        /// <inheritdoc/>
        protected override bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            switch (packageAttribute)
            {
                case PackageAttributeType.HL_VPK_PACKAGE_Archives:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], ArchiveCount, false);
                    return true;
                case PackageAttributeType.HL_VPK_PACKAGE_Version:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header?.Version ?? 0, false);
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
                    int pointer = 0;
                    VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_VPK_ITEM_PRELOAD_BYTES:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.DirectoryEntry.PreloadBytes, false);
                            return true;
                        case PackageAttributeType.HL_VPK_ITEM_ARCHIVE:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.DirectoryEntry.ArchiveIndex, false);
                            return true;
                        case PackageAttributeType.HL_VPK_ITEM_CRC:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], directoryItem.DirectoryEntry.CRC, true);
                            return true;
                    }
                    break;
            }

            return false;
        }

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        protected override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            int pointer = 0;
            VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                extractable = directoryItem.PreloadData != null;
            }
            else if (directoryItem.DirectoryEntry.EntryLength != 0)
            {
                Mapping mapping = Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping;
                extractable = mapping != null ? directoryItem.DirectoryEntry.EntryOffset + directoryItem.DirectoryEntry.EntryLength <= mapping.MappingSize : false;
            }
            else
            {
                extractable = true;
            }

            return true;
        }

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            if (GetFileExtractableInternal(file, out bool extractable))
            {
                if (extractable)
                {
                    if (CreateStreamInternal(file, true, out Stream stream))
                    {
                        if (stream.Open(FileModeFlags.HL_MODE_READ))
                        {
                            uint checksum = 0;
                            validation = Validation.HL_VALIDATES_OK;

                            long uiTotalBytes = 0;
                            int bufferSize;
                            byte[] buffer = new byte[HL_VPK_CHECKSUM_LENGTH];
                            while ((bufferSize = stream.Read(buffer, 0, HL_VPK_CHECKSUM_LENGTH)) != 0)
                            {
                                checksum = Checksum.CRC32(buffer, bufferSize, checksum);
                                uiTotalBytes += bufferSize;
                            }

                            int pointer = 0;
                            VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
                            if (directoryItem.DirectoryEntry.CRC != checksum)
                            {
                                validation = Validation.HL_VALIDATES_CORRUPT;
                            }

                            stream.Close();
                        }
                        else
                        {
                            validation = Validation.HL_VALIDATES_ERROR;
                        }

                        ReleaseStream(stream);
                    }
                    else
                    {
                        validation = Validation.HL_VALIDATES_ERROR;
                    }
                }
                else
                {
                    validation = Validation.HL_VALIDATES_INCOMPLETE;
                }
            }
            else
            {
                validation = Validation.HL_VALIDATES_ERROR;
            }

            return true;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            int pointer = 0;
            VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
            size = (int)(directoryItem.DirectoryEntry.EntryLength + directoryItem.DirectoryEntry.PreloadBytes);
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            int pointer = 0;
            VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
            if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
            {
                if (directoryItem.PreloadData == null)
                    size = 0;
                else
                    size = (int)directoryItem.DirectoryEntry.EntryLength;
            }
            else if (directoryItem.DirectoryEntry.EntryLength != 0)
            {
                size = (int)directoryItem.DirectoryEntry.EntryLength;
                Mapping mapping = Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping;
                if (mapping == null)
                {
                    size = 0;
                }
                else
                {
                    uint mappingSize = (uint)mapping.MappingSize;
                    if (directoryItem.DirectoryEntry.EntryOffset >= mappingSize)
                    {
                        size = 0;
                    }
                    else if (directoryItem.DirectoryEntry.EntryOffset + size > mappingSize)
                    {
                        size = (int)(mappingSize - directoryItem.DirectoryEntry.EntryOffset);
                    }
                }

                size += directoryItem.DirectoryEntry.PreloadBytes;
            }
            else
            {
                size = directoryItem.DirectoryEntry.PreloadBytes;
            }

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            int pointer = 0;
            VPKDirectoryItem directoryItem = VPKDirectoryItem.Create(file.Data, ref pointer);
            if (directoryItem.DirectoryEntry.EntryLength != 0)
            {
                if (directoryItem.DirectoryEntry.ArchiveIndex == HL_VPK_NO_ARCHIVE)
                {
                    if (directoryItem.PreloadData != null)
                        stream = new MemoryStream(directoryItem.PreloadData, (int)directoryItem.DirectoryEntry.EntryLength);
                    else
                        return false;
                }
                else if (Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping != null)
                {
                    if (directoryItem.DirectoryEntry.PreloadBytes != 0)
                    {
                        View view = null;
                        if (!Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping.Map(ref view, directoryItem.DirectoryEntry.EntryOffset, (int)directoryItem.DirectoryEntry.EntryLength))
                            return false;

                        int bufferSize = (int)(directoryItem.DirectoryEntry.EntryLength + directoryItem.DirectoryEntry.PreloadBytes);
                        byte[] lpBuffer = new byte[bufferSize];
                        Array.Copy(directoryItem.PreloadData, 0, lpBuffer, 0, directoryItem.DirectoryEntry.PreloadBytes);
                        Array.Copy(view.ViewData, 0, lpBuffer, directoryItem.DirectoryEntry.PreloadBytes, directoryItem.DirectoryEntry.EntryLength);

                        Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping.Unmap(ref view);

                        stream = new MemoryStream(lpBuffer, bufferSize);
                    }
                    else
                    {
                        stream = new MappingStream(Archives[directoryItem.DirectoryEntry.ArchiveIndex].Mapping, directoryItem.DirectoryEntry.EntryOffset, directoryItem.DirectoryEntry.EntryLength);
                    }
                }
                else
                {
                    return false;
                }
            }
            else if (directoryItem.DirectoryEntry.PreloadBytes != 0)
            {
                stream = new MemoryStream(directoryItem.PreloadData, directoryItem.DirectoryEntry.PreloadBytes);
            }
            else
            {
                stream = new NullStream();
            }

            return true;
        }

        #endregion
    }
}
