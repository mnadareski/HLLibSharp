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
using HLLib.Packages.Common;
using HLLib.Streams;

namespace HLLib.Packages.WAD
{
    /// <summary>
    /// Half-Life Texture Package File
    /// </summary>
    public sealed class WADFile : Package
    {
        #region Constants

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Width", "Height", "Palette Entries", "Mipmaps", "Compressed", "Type" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        /// <summary>
        /// View representing the lump data
        /// </summary>
        private View LumpView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public WADHeader Header { get; private set; }

        /// <summary>
        /// Deserialized lumps data
        /// </summary>
        public WADLump[] Lumps { get; private set; }

        /// <summary>
        /// Deserialized lump infos data
        /// </summary>
        public WADLumpInfo[] LumpInfos { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public WADFile() : base()
        {
            HeaderView = null;
            LumpView = null;
            Header = null;
            Lumps = null;
            LumpInfos = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~WADFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_WAD;

        /// <inheritdoc/>
        public override string Extension => "wad";

        /// <inheritdoc/>
        public override string Description => "Half-Life Texture Package File";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder pRoot = new DirectoryFolder(this);

            // Loop through each lump in the WAD file.
            for (uint i = 0; i < Header.LumpCount; i++)
            {
                pRoot.AddFile($"{Lumps[i].Name}.bmp", i);
            }

            return pRoot;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (WADHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, WADHeader.ObjectSize))
                return false;

            int pointer = 0;
            Header = WADHeader.Create(HeaderView.ViewData, ref pointer);

            if (Header.Signature != "WAD3")
            {
                Console.WriteLine("Invalid file: the file's signature does not match.");
                return false;
            }

            if (!Mapping.Map(ref LumpView, Header.LumpOffset, (int)(Header.LumpCount * WADLump.ObjectSize)))
                return false;

            pointer = 0;
            Lumps = new WADLump[Header.LumpCount];
            for (int i = 0; i < Header.LumpCount; i++)
            {
                Lumps[i] = WADLump.Create(LumpView.ViewData, ref pointer);
            }

            LumpInfos = new WADLumpInfo[Header.LumpCount];
            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            Lumps = null;
            Mapping.Unmap(ref LumpView);

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
                case PackageAttributeType.HL_WAD_PACKAGE_VERSION:
                    uint versionValue = BitConverter.ToUInt32(Encoding.ASCII.GetBytes(Header.Signature), 0) - '0';
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], versionValue, false);
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
                    WADLump lump = Lumps[file.ID];
                    GetLumpInfo(file, out uint width, out uint height, out uint paletteSize, out _, out _, out _);
                    switch (packageAttribute)
                    {
                        case PackageAttributeType.HL_WAD_ITEM_WIDTH:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], width, false);
                            return true;
                        case PackageAttributeType.HL_WAD_ITEM_HEIGHT:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], height, false);
                            return true;
                        case PackageAttributeType.HL_WAD_ITEM_PALETTE_ENTRIES:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], paletteSize, false);
                            return true;
                        case PackageAttributeType.HL_WAD_ITEM_MIPMAPS:
                            uint mipmaps = 0;
                            if (lump.Type == 0x42)
                                mipmaps = 1;
                            else if (lump.Type == 0x43)
                                mipmaps = 4;

                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], mipmaps, false);
                            return true;
                        case PackageAttributeType.HL_WAD_ITEM_COMPRESSED:
                            attribute.SetBoolean(ItemAttributeNames[(int)packageAttribute], lump.Compression != 0);
                            return true;
                        case PackageAttributeType.HL_WAD_ITEM_TYPE:
                            attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], lump.Type, true);
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
            WADLump lump = Lumps[file.ID];
            extractable = (lump.Type == 0x42 || lump.Type == 0x43) && lump.Compression == 0;
            return true;
        }

        #endregion

        #region File Size

        /// <inheritdoc/>
        protected override bool GetFileSizeInternal(DirectoryFile file, out int size)
        {
            size = default;
            if (GetLumpInfo(file, out uint width, out uint height, out uint paletteSize, out _, out _, out _))
                return false;

            size = (int)(BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4 + width * height);
            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = (int)Lumps[file.ID].DiskLength;
            return true;
        }

        #endregion

        #region Image Size

        /// <summary>
        /// Get image size information from an internal file
        /// </summary>
        /// <param name="file">Internal file to get information from</param>
        /// <param name="width">Image width</param>
        /// <param name="height">Image height</param>
        /// <param name="paletteData">Palette data byte array</param>
        /// <param name="pixelData">Pixel data byte array</param>
        /// <returns>True if image size information could be retrieved, false otherwise</returns>
        public bool GetImageData(DirectoryFile file, out uint width, out uint height, out byte[] paletteData, out byte[] pixelData)
        {
            width = height = 0;
            paletteData = pixelData = null;
            if (!Opened || file == null || file.Package != this)
            {
                Console.WriteLine("File does not belong to package.");
                return false;
            }

            if (!GetLumpInfo(file, out width, out height, out _, out paletteData, out pixelData, out View view))
                return false;

            Mapping.Unmap(ref view);

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (!GetLumpInfo(file, out uint width, out uint height, out uint paletteSize, out byte[] palette, out byte[] pixels, out View view))
                return false;

            int bufferSize = (int)(BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4 + width * height);
            byte[] buffer = new byte[bufferSize];

            //
            // Allocate data.
            //

            BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();
            BITMAPINFOHEADER infoHeader = new BITMAPINFOHEADER();
            byte[] paletteData = new byte[paletteSize * 4];
            byte[] pixelData = new byte[width * height];

            //
            // Fill in headers.
            //

            fileHeader.Type = ('M' << 8) | 'B';
            fileHeader.Size = (BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4 + width * height);
            fileHeader.OffBits = (BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4);

            infoHeader.Size = BITMAPINFOHEADER.ObjectSize;
            infoHeader.Width = (int)width;
            infoHeader.Height = (int)height;
            infoHeader.Planes = 1;
            infoHeader.BitCount = 8;
            infoHeader.SizeImage = 0;
            infoHeader.ClrUsed = paletteSize;
            infoHeader.ClrImportant = paletteSize;

            //
            // Fill in Palette data.
            //

            for (uint i = 0; i < paletteSize; i++)
            {
                paletteData[i * 4 + 0] = palette[i * 3 + 2];
                paletteData[i * 4 + 1] = palette[i * 3 + 1];
                paletteData[i * 4 + 2] = palette[i * 3 + 0];
                paletteData[i * 4 + 3] = 0;
            }

            //
            // Fill in Index data.
            //

            for (uint i = 0; i < width; i++)
            {
                for (uint j = 0; j < height; j++)
                {
                    pixelData[i + (height - 1 - j) * width] = pixels[i + j * width];
                }
            }

            Mapping.Unmap(ref view);

            int pointer = 0;
            Array.Copy(fileHeader.Serialize(), 0, buffer, pointer, BITMAPFILEHEADER.ObjectSize); pointer += BITMAPFILEHEADER.ObjectSize;
            Array.Copy(infoHeader.Serialize(), 0, buffer, pointer, BITMAPINFOHEADER.ObjectSize); pointer += BITMAPINFOHEADER.ObjectSize;
            Array.Copy(paletteData, 0, buffer, pointer, paletteData.Length); pointer += paletteData.Length;
            Array.Copy(pixelData, 0, buffer, pointer, pixelData.Length); pointer += pixelData.Length;

            stream = new MemoryStream(buffer, bufferSize);

            return true;
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Get lump information from an internal file
        /// </summary>
        /// <param name="file">Internal file to get information from</param>
        /// <param name="width">Lump width</param>
        /// <param name="height">Lump height</param>
        /// <param name="paletteSize">Lump palette size</param>
        /// <param name="palette">Palette information byte array</param>
        /// <param name="pixels">Pixel information byte array</param>
        /// <param name="mipmap">Mipmap level</param>
        /// <returns>True if lump information could be retrieved, false otherwise</returns>
        private bool GetLumpInfo(DirectoryFile file, out uint width, out uint height, out uint paletteSize, out byte[] palette, out byte[] pixels, out View view, uint mipmap = 0)
        {
            WADLump lump = Lumps[file.ID];

            width = height = paletteSize = 0;
            palette = pixels = null;
            view = null;

            if (lump.Compression != 0)
            {
                Console.WriteLine($"Error reading lump: compression format {lump.Compression} not supported.");
                return false;
            }

            if (lump.Type == 0x42)
            {
                if (mipmap > 0)
                {
                    Console.WriteLine($"Error reading lump: invalid mipmap level {mipmap}.");
                    return false;
                }
            }
            else if (lump.Type == 0x43)
            {
                if (mipmap > 3)
                {
                    Console.WriteLine($"Error reading lump: invalid mipmap level {mipmap}.");
                    return false;
                }
            }
            else
            {
                Console.WriteLine($"Error reading lump: lump type {lump.Type} not supported.");
                return false;
            }

            if (!Mapping.Map(ref view, lump.Offset, (int)lump.DiskLength))
                return false;

            byte[] data = view.ViewData;
            int dataPointer = 0;

            // Type 0x42 has no name, type 0x43 does.  Are these flags?
            if (lump.Type == 0x42)
            {
                // Get Width.
                width = BitConverter.ToUInt32(data, dataPointer);
                dataPointer += 4;

                // Get Height.
                height = BitConverter.ToUInt32(data, dataPointer);
                dataPointer += 4;

                // Get pixel data.
                pixels = new byte[width * height];
                Array.Copy(data, dataPointer, pixels, 0, pixels.Length);
                dataPointer += (int)(width * height);

                // Get palette size.
                paletteSize = BitConverter.ToUInt16(data, dataPointer);
                dataPointer += 2;

                // Get palette.
                palette = new byte[paletteSize];
                Array.Copy(data, dataPointer, palette, 0, paletteSize);
            }
            else if (lump.Type == 0x43)
            {
                // Scan past name.
                dataPointer += 16;

                // Get Width.
                width = BitConverter.ToUInt32(data, dataPointer);
                dataPointer += 4;

                // Get Height.
                height = BitConverter.ToUInt32(data, dataPointer);
                dataPointer += 4;

                // Get pixel offset.
                uint pixelOffset = BitConverter.ToUInt32(data, dataPointer);
                dataPointer += 16;

                pixels = new byte[width * height];
                Array.Copy(data, pixelOffset, pixels, 0, pixels.Length);

                uint pixelSize = width * height;
                switch (mipmap)
                {
                    case 1:
                        dataPointer += (int)(pixelSize);
                        break;
                    case 2:
                        dataPointer += (int)((pixelSize) + (pixelSize / 4));
                        break;
                    case 3:
                        dataPointer += (int)((pixelSize) + (pixelSize / 4) + (pixelSize / 16));
                        break;
                }

                // Scan past data.
                dataPointer += (int)((pixelSize) + (pixelSize / 4) + (pixelSize / 16) + (pixelSize / 64));

                // Get palette size.
                paletteSize = BitConverter.ToUInt16(data, dataPointer);
                dataPointer += 2;

                // Get palette.
                palette = new byte[paletteSize];
                Array.Copy(data, dataPointer, palette, 0, paletteSize);
            }

            //Mapping.Unmap(pView);

            switch (mipmap)
            {
                case 1:
                    width /= 2;
                    height /= 2;
                    break;
                case 2:
                    width /= 4;
                    height /= 4;
                    break;
                case 3:
                    width /= 8;
                    height /= 8;
                    break;
            }

            return true;
        }

        #endregion
    }
}
