/*
 * HLLib
 * Copyright (C) 2006-2012 Ryan Gregg

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

namespace HLLib.Packages.SGA
{
    public class SGAFile : Package
    {
        #region Constants

        /// <summary>
        /// Length of a SGA checksum in bytes
        /// </summary>
        public const int HL_SGA_CHECKSUM_LENGTH = 0x00008000;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Major Version", "Minor Version", "File MD5", "Name", "Header MD5" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Section Alias", "Section Name", "Modified", "Type", "CRC", "Verification" };

        /// <summary>
        /// Set of valid verification names
        /// </summary>
        public static readonly string[] VerificationNames = new string[] { "None", "CRC", "CRC Blocks", "MD5 Blocks", "SHA1 Blocks" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public SGAHeaderBase Header { get; private set; }

        /// <summary>
        /// Deserialized directory data
        /// </summary>
        public SGADirectory Directory { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public SGAFile() : base()
        {
            HeaderView = null;

            Header = null;
            Directory = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~SGAFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_SGA;

        /// <inheritdoc/>
        public override string Extension => "sga";

        /// <inheritdoc/>
        public override string Description => "Archive File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot() => Directory.CreateRoot();

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            int maxHeaderSize = Math.Max(SGAHeader4.ObjectSize, SGAHeader6.ObjectSize);
            if (maxHeaderSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, maxHeaderSize))
                return false;

            int pointer = 0;
            Header = SGAHeaderBase.Create(HeaderView.ViewData, ref pointer);

            if (Header.Signature != "_ARCHIVE")
            {
                Console.WriteLine("Invalid file: the file's signature does not match.");
                return false;
            }

            if ((Header.MajorVersion != 4 || Header.MinorVersion != 0) &&
                (Header.MajorVersion != 5 || Header.MinorVersion != 0) &&
                (Header.MajorVersion != 6 || Header.MinorVersion != 0) &&
                (Header.MajorVersion != 7 || Header.MinorVersion != 0))
            {
                Console.WriteLine("Invalid SGA version (v%hu.%hu): you have a version of a SGA file that HLLib does not know how to read. Check for product updates.", Header.MajorVersion, Header.MinorVersion);
                return false;
            }

            switch (Header.MajorVersion)
            {
                case 4:
                    pointer = 0;
                    Header = SGAHeader4.Create(HeaderView.ViewData, ref pointer);
                    if (((SGAHeader4)Header).HeaderLength > Mapping.MappingSize)
		            {
                        Console.WriteLine("Invalid file: the file map is too small for it's extended header.");
                        return false;
                    }

                    Directory = new SGADirectory4(this);
                    break;
                case 5:
                    pointer = 0;
                    Header = SGAHeader4.Create(HeaderView.ViewData, ref pointer);
                    if (((SGAHeader4)Header).HeaderLength > Mapping.MappingSize)
                    {
                        Console.WriteLine("Invalid file: the file map is too small for it's extended header.");
                        return false;
                    }

                    Directory = new SGADirectory5(this);
                    break;
                case 6:
                    pointer = 0;
                    Header = SGAHeader6.Create(HeaderView.ViewData, ref pointer);
                    if (((SGAHeader6)Header).HeaderLength > Mapping.MappingSize)
                    {
                        Console.WriteLine("Invalid file: the file map is too small for it's extended header.");
                        return false;
                    }

                    Directory = new SGADirectory6(this);
                    break;
                case 7:
                    pointer = 0;
                    Header = SGAHeader6.Create(HeaderView.ViewData, ref pointer);
                    if (((SGAHeader6)Header).HeaderLength > Mapping.MappingSize)
                    {
                        Console.WriteLine("Invalid file: the file map is too small for it's extended header.");
                        return false;
                    }

                    Directory = new SGADirectory7(this);
                    break;
                default:
                    return false;
            }

            if (!Directory.MapDataStructures())
                return false;

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            Directory = null;

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
                case PackageAttributeType.HL_SGA_PACKAGE_VERSION_MAJOR:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.MajorVersion, false);
                    return true;
                case PackageAttributeType.HL_SGA_PACKAGE_VERSION_MINOR:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.MinorVersion, false);
                    return true;
                case PackageAttributeType.HL_SGA_PACKAGE_MD5_FILE:
                    if (Header.MajorVersion >= 4 && Header.MajorVersion <= 5)
                    {
                        attribute.SetString(AttributeNames[(int)packageAttribute], BitConverter.ToString(((SGAHeader4)Header).FileMD5).Replace("-", string.Empty));
                        return true;
                    }
                    return false;
                case PackageAttributeType.HL_SGA_PACKAGE_NAME:
                    if (Header.MajorVersion >= 4 && Header.MajorVersion <= 5)
                    {
                        attribute.SetString(AttributeNames[(int)packageAttribute], ((SGAHeader4)Header).Name);
                        return true;
                    }
                    if (Header.MajorVersion >= 6 && Header.MajorVersion <= 6)
                    {
                        attribute.SetString(AttributeNames[(int)packageAttribute], ((SGAHeader6)Header).Name);
                        return true;
                    }
                    return false;
                case PackageAttributeType.HL_SGA_PACKAGE_MD5_HEADER:
                    if (Header.MajorVersion >= 4 && Header.MajorVersion <= 5)
                    {
                        attribute.SetString(AttributeNames[(int)packageAttribute], BitConverter.ToString(((SGAHeader4)Header).HeaderMD5).Replace("-", string.Empty));
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }

        /// <inheritdoc/>
        protected override bool GetItemAttributeInternal(DirectoryItem item, PackageAttributeType packageAttribute, out PackageAttribute attribute)
            => Directory.GetItemAttributeInternal(item, packageAttribute, out attribute);

        #endregion

        #region File Extraction Check

        /// <inheritdoc/>
        protected override bool GetFileExtractableInternal(DirectoryFile file, out bool extractable)
            => Directory.GetFileExtractableInternal(file, out extractable);

        #endregion

        #region File Validation

        /// <inheritdoc/>
        protected override bool GetFileValidationInternal(DirectoryFile file, out Validation validation)
            => Directory.GetFileValidationInternal(file, out validation);

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
            => Directory.GetFileSizeInternal(file, out size);

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
            => Directory.GetFileSizeOnDiskInternal(file, out size);

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
            => Directory.CreateStreamInternal(file, readEncrypted, out stream);

        #endregion
    }
}
