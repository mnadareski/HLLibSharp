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
using System.Linq;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Streams;

namespace HLLib.Packages
{
    public abstract class Package
    {
        #region Constants

        /// <summary>
        /// Invalid item ID
        /// </summary>
        public const uint HL_ID_INVALID = 0xFFFFFFFF;

        /// <summary>
        /// Set of valid attribute names
        /// </summary>
        public abstract string[] AttributeNames { get; }

        /// <summary>
        /// Set of valid item attribute names
        /// </summary>
        public abstract string[] ItemAttributeNames { get; }

        #endregion

        #region Fields

        /// <summary>
        /// Enable deleting the source stream on close
        /// </summary>
        public bool DeleteStream { get; private set; }

        /// <summary>
        /// Enable deleting the source mapping on close
        /// </summary>
        public bool DeleteMapping { get; private set; }

        /// <summary>
        /// Source stream the package is based on
        /// </summary>
        public Stream Stream { get; private set; }

        /// <summary>
        /// Source mapping the package is based on
        /// </summary>
        public Mapping Mapping { get; private set; }

        /// <summary>
        /// Root directory of the package
        /// </summary>
        public DirectoryFolder Root { get; private set; }

        /// <summary>
        /// List of open streams in the package
        /// </summary>
        public List<Stream> Streams { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Package()
        {
            DeleteStream = false;
            DeleteMapping = false;
            Stream = null;
            Mapping = null;
            Root = null;
            Streams = null;
        }

        #region Static Helpers

        /// <summary>
        /// Detect a package type from the magic number in the header
        /// </summary>
        /// <param name="magic">Magic number from the header (at least 8 bytes)</param>
        /// <param name="extension">File extension to check</param>
        /// <returns>Detected package type, if possible</returns>
        public static PackageType GetPackageType(byte[] magic, string extension = null)
        {
            // Magic number data
            if (magic != null && magic.Length >= 8)
            {
                // BSP
                if (new byte[] { 0x1e, 0x00, 0x00, 0x00 }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_BSP;

                // GCF
                if (new byte[] { 0x01, 0x00, 0x00, 0x00, 0x01, 0x00, 0x00, 0x00 }.SequenceEqual(magic.Take(8)))
                    return PackageType.HL_PACKAGE_GCF;

                // NCF
                if (new byte[] { 0x01, 0x00, 0x00, 0x00, 0x02, 0x00, 0x00, 0x00 }.SequenceEqual(magic.Take(8)))
                    return PackageType.HL_PACKAGE_NCF;

                // PAK
                if (new byte[] { (byte)'P', (byte)'A', (byte)'C', (byte)'K' }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_PAK;

                // SGA
                if (new byte[] { (byte)'_', (byte)'A', (byte)'R', (byte)'C', (byte)'H', (byte)'I', (byte)'V', (byte)'E' }.SequenceEqual(magic.Take(8)))
                    return PackageType.HL_PACKAGE_SGA;

                // VBSP
                if (new byte[] { (byte)'V', (byte)'B', (byte)'S', (byte)'P' }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_VBSP;

                // VPK
                if (new byte[] { 0x34, 0x12, 0x55, 0xaa }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_VPK;

                // WAD
                if (new byte[] { (byte)'W', (byte)'A', (byte)'D', (byte)'3' }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_WAD;

                // XZP
                if (new byte[] { (byte)'p', (byte)'i', (byte)'Z', (byte)'x' }.SequenceEqual(magic.Take(4)))
                    return PackageType.HL_PACKAGE_XZP;

                // ZIP
                if (new byte[] { (byte)'P', (byte)'K' }.SequenceEqual(magic.Take(2)))
                    return PackageType.HL_PACKAGE_ZIP;
            }

            // Extension
            if (extension != null && extension.Length > 0)
            {
                extension = extension.TrimStart('.').ToLowerInvariant();

                // BSP
                if (extension == new BSP.BSPFile().Extension)
                    return PackageType.HL_PACKAGE_BSP;

                // GCF
                if (extension == new GCF.GCFFile().Extension)
                    return PackageType.HL_PACKAGE_GCF;

                // NCF
                if (extension == new NCF.NCFFile().Extension)
                    return PackageType.HL_PACKAGE_NCF;

                // PAK
                if (extension == new PAK.PAKFile().Extension)
                    return PackageType.HL_PACKAGE_PAK;

                // SGA
                if (extension == new SGA.SGAFile().Extension)
                    return PackageType.HL_PACKAGE_SGA;

                // VBSP
                if (extension == new VBSP.VBSPFile().Extension)
                    return PackageType.HL_PACKAGE_VBSP;

                // VPK
                if (extension == new VPK.VPKFile().Extension)
                    return PackageType.HL_PACKAGE_VPK;

                // WAD
                if (extension == new WAD.WADFile().Extension)
                    return PackageType.HL_PACKAGE_WAD;

                // XZP
                if (extension == new XZP.XZPFile().Extension)
                    return PackageType.HL_PACKAGE_XZP;

                // ZIP
                if (extension == new ZIP.ZIPFile().Extension)
                    return PackageType.HL_PACKAGE_ZIP;
            }

            return PackageType.HL_PACKAGE_NONE;
        }

        /// <summary>
        /// Create a package specified by the type
        /// </summary>
        /// <param name="packageType">Package type to generate</param>
        /// <returns>Empty package file, null on error</returns>
        public static Package CreatePackage(PackageType packageType)
        {
            switch (packageType)
            {
                case PackageType.HL_PACKAGE_NONE:
                    Console.WriteLine("Unsupported package type.");
                    return null;
                case PackageType.HL_PACKAGE_BSP:
                    return new BSP.BSPFile();
                case PackageType.HL_PACKAGE_GCF:
                    return new GCF.GCFFile();
                case PackageType.HL_PACKAGE_NCF:
                    return new NCF.NCFFile();
                case PackageType.HL_PACKAGE_PAK:
                    return new PAK.PAKFile();
                case PackageType.HL_PACKAGE_VBSP:
                    return new VBSP.VBSPFile();
                case PackageType.HL_PACKAGE_VPK:
                    return new VPK.VPKFile();
                case PackageType.HL_PACKAGE_WAD:
                    return new WAD.WADFile();
                case PackageType.HL_PACKAGE_XZP:
                    return new XZP.XZPFile();
                case PackageType.HL_PACKAGE_ZIP:
                    return new ZIP.ZIPFile();
                default:
                    Console.WriteLine($"Invalid package type {packageType}.");
                    return null;
            }
        }

        /// <summary>
        /// Print all descriptor infoemation to console
        /// </summary>
        /// <param name="package">Package to print info for</param>
        public static void PrintPackageInfo(Package package)
        {
            // Invalid pavkages can't have info printed
            if (package == null || package.PackageType == PackageType.HL_PACKAGE_NONE)
            {
                Console.WriteLine("Invalid package.");
                return;
            }

            // Common information
            Console.WriteLine($"Package Type: {package.PackageType}");
            Console.WriteLine($"Extension: {package.Extension}");
            Console.WriteLine($"Description: {package.Description}");
            if (package.AttributeNames.Length > 0)
                Console.WriteLine($"Attribute Names: {string.Join(", ", package.AttributeNames)}");
            if (package.ItemAttributeNames.Length > 0)
                Console.WriteLine($"Item Attribute Names: {string.Join(", ", package.ItemAttributeNames)}");

            // Per-package information
            // TODO: Add mapping/object information
            switch (package.PackageType)
            {
                case PackageType.HL_PACKAGE_BSP:
                    var bspPackage = package as BSP.BSPFile;
                    break;

                case PackageType.HL_PACKAGE_GCF:
                    var gcfPackage = package as GCF.GCFFile;
                    break;

                case PackageType.HL_PACKAGE_NCF:
                    var ncfPackage = package as NCF.NCFFile;
                    Console.WriteLine($"Root Path: {ncfPackage.RootPath}");
                    break;

                case PackageType.HL_PACKAGE_PAK:
                    var pakPackage = package as PAK.PAKFile;
                    break;

                case PackageType.HL_PACKAGE_VBSP:
                    var vbspPackage = package as VBSP.VBSPFile;
                    break;

                case PackageType.HL_PACKAGE_VPK:
                    var vpkPackage = package as VPK.VPKFile;
                    Console.WriteLine($"Archive Count: {vpkPackage.ArchiveCount}");
                    break;

                case PackageType.HL_PACKAGE_WAD:
                    var wadPackage = package as WAD.WADFile;
                    break;

                case PackageType.HL_PACKAGE_XZP:
                    var xzpPackage = package as XZP.XZPFile;
                    break;

                case PackageType.HL_PACKAGE_ZIP:
                    var zipPackage = package as ZIP.ZIPFile;
                    break;
            }
        }

        #endregion

        #region Descriptors

        /// <summary>
        /// Internal package type
        /// </summary>
        public abstract PackageType PackageType { get; }

        /// <summary>
        /// Common file extension for this package type
        /// </summary>
        /// <remarks>
        /// The extension does not have a leading '.' and is all lower-case
        /// </remarks>
        public abstract string Extension { get; }

        /// <summary>
        /// Short description of the package type
        /// </summary>
        public abstract string Description { get; }

        #endregion

        #region Opening and Closing

        /// <summary>
        /// Indicate of the package is opened or not
        /// </summary>
        public bool Opened => Mapping != null;

        /// <summary>
        /// Open package from a stream
        /// </summary>
        /// <param name="stream">Stream to open package from</param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        public bool Open(Stream stream, FileModeFlags fileMode, bool overwriteFiles = true) => Open(stream, fileMode, false, overwriteFiles);

        /// <summary>
        /// Open package from a mapping
        /// </summary>
        /// <param name="mapping">Mapping to open package from</param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        public bool Open(Mapping mapping, FileModeFlags fileMode, bool overwriteFiles = true) => Open(mapping, fileMode, false, overwriteFiles);

        /// <summary>
        /// Open package from a file path
        /// </summary>
        /// <param name="fileName">File path to open package from</param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        public bool Open(string fileName, FileModeFlags fileMode, bool overwriteFiles = true)
        {
            if (fileMode.HasFlag(FileModeFlags.HL_MODE_NO_FILEMAPPING))
                return Open(new FileStream(fileName), fileMode, true, overwriteFiles);
            else
                return Open(new FileMapping(fileName), fileMode, true, overwriteFiles);
        }

        /// <summary>
        /// Open package from buffered data
        /// </summary>
        /// <param name="data">Byte array to open package from</param>
        /// <param name="bufferSize">Size of the byte array</param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        public bool Open(byte[] data, int bufferSize, FileModeFlags fileMode, bool overwriteFiles = true) => Open(new MemoryStream(data, bufferSize), fileMode, true, overwriteFiles);

        /// <summary>
        /// Open package from user data
        /// </summary>
        /// <param name="userData">User data byte array to open package from</param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        public bool Open(byte[] userData, FileModeFlags fileMode, bool overwriteFiles = true) => Open(new MemoryStream(userData, userData.Length), fileMode, true, overwriteFiles);

        /// <summary>
        /// Open package from a stream
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="deleteStream">Determine if the stream should be closed when the package is closed</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        private bool Open(Stream stream, FileModeFlags fileMode, bool deleteStream, bool overwriteFiles)
        {
            Close();

            Stream = stream;
            DeleteStream = deleteStream;

            DeleteMapping = true;
            Mapping = new StreamMapping(Stream);

            if (!Mapping.Open(fileMode, overwriteFiles))
            {
                Close();
                return false;
            }

            if (!MapDataStructures())
            {
                UnmapDataStructures();
                Close();
                return false;
            }

            Streams = new List<Stream>();
            return true;
        }

        /// <summary>
        /// Open package from a mapping
        /// </summary>
        /// <param name="mapping"></param>
        /// <param name="fileMode">File mode for source access</param>
        /// <param name="deleteMapping">Determine if the mapping should be closed when the package is closed</param>
        /// <param name="overwriteFiles">True if files should be overwritten on output, false otherwise</param>
        /// <returns>True if the package could be opened, false otherwise</returns>
        private bool Open(Mapping mapping, FileModeFlags fileMode, bool deleteMapping, bool overwriteFiles)
        {
            Close();

            DeleteMapping = deleteMapping;
            Mapping = mapping;

            if (!Mapping.Open(fileMode, overwriteFiles))
            {
                Close();
                return false;
            }

            if (!MapDataStructures())
            {
                UnmapDataStructures();
                Close();
                return false;
            }

            Streams = new List<Stream>();
            return true;
        }

        /// <summary>
        /// Close the package
        /// </summary>
        public void Close()
        {
            if (Streams != null)
            {
                for (int i = 0; i < Streams.Count; i++)
                {
                    Stream stream = Streams[i];
                    stream.Close();
                    Streams[i] = null;
                }

                Streams = null;
            }

            if (Mapping != null)
            {
                UnmapDataStructures();
                Mapping.Close();
            }

            if (Root != null)
            {
                ReleaseRoot();
                Root = null;
            }

            if (DeleteMapping)
            {
                Mapping.Close();
                DeleteMapping = false;
            }
            Mapping = null;

            if (DeleteStream)
            {
                Stream.Close();
                DeleteStream = false;
            }
            Stream = null;
        }

        #endregion

        #region Defragmentation

        /// <summary>
        /// Defragment the package, if supported
        /// </summary>
        /// <param name="forceDefragment">True to force defragmentation, false otherwise</param>
        /// <returns>True if the package was defragmented, false otherwise</returns>
        public bool Defragment(bool forceDefragment)
        {
            if (!Opened)
            {
                Console.WriteLine("Package not opened.");
                return false;
            }

            if (Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Package does not have write privileges, please enable them.");
                return false;
            }

            if (Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_VOLATILE))
            {
                Console.WriteLine("Package has volatile access enabled, please disable it.");
                return false;
            }

            return DefragmentInternal(forceDefragment);
        }

        /// <summary>
        /// Per-package implementation of defragmentation
        /// </summary>
        /// <param name="forceDefragment">True to force defragmentation, false otherwise</param>
        /// <returns>True if the package was defragmented, false otherwise</returns>
        protected virtual bool DefragmentInternal(bool forceDefragment) => true;

        #endregion

        #region Mappings

        /// <summary>
        /// Get the root folder of the package
        /// </summary>
        /// <returns>Root directory folder, null on error</returns>
        public DirectoryFolder GetRoot()
        {
            if (!Opened)
                return null;

            if (Root == null)
            {
                Root = CreateRoot();
                Root.Sort();
            }

            return Root;
        }

        /// <summary>
        /// Per-package creation of a root directory object, if possible
        /// </summary>
        /// <returns>Root directory folder, null on error</returns>
        protected abstract DirectoryFolder CreateRoot();

        /// <summary>
        /// Release the root directory
        /// </summary>
        protected virtual void ReleaseRoot() { }

        /// <summary>
        /// Internally map all data structures
        /// </summary>
        /// <returns>True if all structures could be mapped, false otherwise</returns>
        protected abstract bool MapDataStructures();

        /// <summary>
        /// Internally unmap all data structures
        /// </summary>
        protected abstract void UnmapDataStructures();

        #endregion

        #region Attributes

        /// <summary>
        /// Get the total number of attributes in the package
        /// </summary>
        /// <returns>Total number of attributes in the package, 0 on error</returns>
        public int GetAttributeCount()
        {
            if (!Opened)
                return 0;

            return AttributeNames?.Length ?? 0;
        }

        /// <summary>
        /// Get the attribute name for the package attribute value
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get value for</param>
        /// <returns>Package attribute name, null on error</returns>
        public string GetAttributeName(PackageAttributeType packageAttribute)
        {
            if (!Opened)
                return null;

            if ((int)packageAttribute < GetAttributeCount())
                return AttributeNames[(int)packageAttribute];

            return null;
        }

        /// <summary>
        /// Get a package attribute based on the package attribute value
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        public bool GetAttribute(PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            if (!Opened)
            {
                Console.WriteLine("Package not open.");
                return false;
            }

            return GetAttributeInternal(packageAttribute, out attribute);
        }

        /// <summary>
        /// Get the total number of item attributes in the package
        /// </summary>
        /// <returns>Total number of item attributes in the package, 0 on error</returns>
        public int GetItemAttributeCount()
        {
            if (!Opened)
                return 0;

            return ItemAttributeNames?.Length ?? 0;
        }

        /// <summary>
        /// Get the item attribute name for the package attribute value
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get value for</param>
        /// <returns>Item attribute name, null on error</returns>
        public string GetItemAttributeName(PackageAttributeType packageAttribute)
        {
            if (!Opened)
                return null;

            if ((int)packageAttribute < GetItemAttributeCount())
                return ItemAttributeNames[(int)packageAttribute];

            return null;
        }

        /// <summary>
        /// Get an item attribute based on the package attribute value
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        public bool GetItemAttribute(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
        {
            attribute = new PackageAttribute();
            if (!Opened || item == null || item.Package != this)
            {
                Console.WriteLine("Item does not belong to package.");
                return false;
            }

            return GetItemAttributeInternal(item, packageAttribute, out attribute);
        }

        /// <summary>
        /// Per-package creation of a package attribute object, if possible
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        protected abstract bool GetAttributeInternal(PackageAttributeType packageAttribute, out PackageAttribute attribute);

        /// <summary>
        /// Per-package creation of an item attribute object, if possible
        /// </summary>
        /// <param name="packageAttribute">Package attribute to get derive from</param>
        /// <param name="attribute">Output attribute for that value</param>
        /// <returns>True if the value could be derived, false otherwise</returns>
        protected abstract bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute);

        #endregion

        #region File Extraction Check

        /// <summary>
        /// Get if a file in the package is extractable
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="extractable">True if the file can be extracted, false otherwise</param>
        /// <returns>True if the extractability could be derived, false otherwise</returns>
        public bool GetFileExtractable(DirectoryFile file, out bool extractable)
        {
            extractable = false;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            return GetFileExtractableInternal(file, out extractable);
        }

        /// <summary>
        /// Per-package implementation of file extraction checks
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="extractable">True if the file can be extracted, false otherwise</param>
        /// <returns>True if the extractability could be derived, false otherwise</returns>
        protected virtual bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
        {
            extractable = true;
            return true;
        }

        #endregion

        #region File Validation

        /// <summary>
        /// Get the validation value for a file in the package
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="validation">Output validation value</param>
        /// <returns>True if the validaiton could be performed, false otherwise</returns>
        public bool GetFileValidation(DirectoryFile file, out Validation validation)
        {
            validation = Validation.HL_VALIDATES_ASSUMED_OK;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            return GetFileValidationInternal(file, out validation);
        }

        /// <summary>
        /// Per-package implementation of file extraction checks
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="validation">Output validation value</param>
        /// <returns>True if the validaiton could be performed, false otherwise</returns>
        protected virtual bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
        {
            validation = Validation.HL_VALIDATES_ASSUMED_OK;
            return true;
        }

        #endregion

        #region File Size

        /// <summary>
        /// Get the internal size of an internal file
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        public bool GetFileSize(DirectoryFile file, out int size)
        {
            size = 0;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            return GetFileSizeInternal(file, out size);
        }

        /// <summary>
        /// Get the extracted size of an internal file
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        public bool GetFileSizeOnDisk(DirectoryFile file, out int size)
        {
            size = 0;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            return GetFileSizeOnDiskInternal(file, out size);
        }

        /// <summary>
        /// Per-package implementation of internal size check
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        protected abstract bool GetFileSizeInternal(DirectoryFile file, out int size);

        /// <summary>
        /// Per-package implementation of extracted size check
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="size">Output size value</param>
        /// <returns>True if the size could be derived, false otherwise</returns>
        protected abstract bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size);

        #endregion

        #region Streams

        /// <summary>
        /// Create a stream from an internal file
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="readEncrypted">True to read encrypted files, false otherwise</param>
        /// <param name="stream">Output stream</param>
        /// <returns>True if the stream could be created, false otherwise</returns>
        public bool CreateStream(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            if (!CreateStreamInternal(file, readEncrypted, out stream))
                return false;

            Streams.Add(stream);
            return true;
        }

        /// <summary>
        /// Release a stream owned by this package
        /// </summary>
        /// <param name="stream">Stream to release</param>
        public void ReleaseStream(Stream stream)
        {
            if (!Opened)
                return;

            for (int i = 0; i < Streams.Count; i++)
            {
                if (Streams[i] == stream)
                {
                    stream.Close();
                    Streams.RemoveAt(i);
                    break;
                }
            }
        }

        /// <summary>
        /// Internal implementation of stream creation
        /// </summary>
        /// <param name="file">DirectoryFile representing the internal file</param>
        /// <param name="readEncrypted">True to read encrypted files, false otherwise</param>
        /// <param name="stream">Output stream</param>
        /// <returns>True if the stream could be created, false otherwise</returns>
        protected abstract bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream);

        #endregion
    }
}