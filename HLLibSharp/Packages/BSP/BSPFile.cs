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
using HLLib.Packages.Common;
using HLLib.Streams;

namespace HLLib.Packages.BSP
{
    /// <summary>
    /// Half-Life Level
    /// </summary>
    public sealed class BSPFile : Package
    {
        #region Constants

        /// <summary>
        /// Index for the entities lump
        /// </summary>
        public const int HL_BSP_LUMP_ENTITIES = 0;

        /// <summary>
        /// Index for the texture data lump
        /// </summary>
        public const int HL_BSP_LUMP_TEXTUREDATA = 2;

        /// <summary>
        /// Number of valid mipmap levels
        /// </summary>
        public const int HL_BSP_MIPMAP_COUNT = 4;

        /// <inheritdoc/>
        public override string[] AttributeNames => new string[] { "Version" };

        /// <inheritdoc/>
        public override string[] ItemAttributeNames => new string[] { "Width", "Height", "Palette Entries" };

        #endregion

        #region Views

        /// <summary>
        /// View representing header data
        /// </summary>
        private View HeaderView;

        /// <summary>
        /// View representing texture data
        /// </summary>
        private View TextureView;

        #endregion

        #region Fields

        /// <summary>
        /// Deserialized header data
        /// </summary>
        public BSPHeader Header { get; private set; }

        /// <summary>
        /// Deserialized texture header data
        /// </summary>
        public BSPTextureHeader TextureHeader { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public BSPFile() : base()
        {
            HeaderView = null;
            TextureView = null;
            Header = null;
            TextureHeader = null;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~BSPFile() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override PackageType PackageType => PackageType.HL_PACKAGE_BSP;

        /// <inheritdoc/>
        public override string Extension => "bsp";

        /// <inheritdoc/>
        public override string Description => "Half-Life Level";

        #endregion

        #region Mappings

        /// <inheritdoc/>
        protected override DirectoryFolder CreateRoot()
        {
            DirectoryFolder root = new DirectoryFolder(this);

            if (Header.Lumps[HL_BSP_LUMP_ENTITIES].Length != 0)
            {
                string fileName = System.IO.Path.GetFileName(Mapping.FileName);
                if (fileName[fileName.Length - 4] == '\0')
                {
                    root.AddFile("entities.ent", TextureHeader.TextureCount);
                }
                else
                {
                    fileName += ".ent";
                    root.AddFile(fileName, TextureHeader.TextureCount);
                }
            }

            // Loop through each texture in the BSP file.
            for (uint i = 0; i < TextureHeader.TextureCount; i++)
            {
                if ((int)TextureHeader.Offsets[i] == -1)
                    continue;

                int pointer = (int)TextureHeader.Offsets[i];
                BSPTexture texture = BSPTexture.Create(TextureView.ViewData, ref pointer);
                if (texture.Offsets[0] == 0)
                    continue;

                // Add the lump as a bitmap.
                root.AddFile($"{texture.Name}.bmp", i);
            }

            return root;
        }

        /// <inheritdoc/>
        protected override bool MapDataStructures()
        {
            if (BSPHeader.ObjectSize > Mapping.MappingSize)
            {
                Console.WriteLine("Invalid file: the file map is too small for it's header.");
                return false;
            }

            if (!Mapping.Map(ref HeaderView, 0, BSPHeader.ObjectSize))
                return false;

            int pointer = 0;
            Header = BSPHeader.Create(HeaderView.ViewData, ref pointer);
            if (Header.Version != 30)
            {
                Console.WriteLine($"Invalid BSP version (v{Header.Version}): you have a version of a BSP file that HLLib does not know how to read. Check for product updates.");
                return false;
            }

            if (!Mapping.Map(ref TextureView, Header.Lumps[HL_BSP_LUMP_TEXTUREDATA].Offset, (int)Header.Lumps[HL_BSP_LUMP_TEXTUREDATA].Length))
                return false;

            pointer = 0;
            TextureHeader = BSPTextureHeader.Create(TextureView.ViewData, ref pointer);

            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapDataStructures()
        {
            TextureHeader = null;
            Mapping.Unmap(ref TextureView);

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
                case PackageAttributeType.HL_BSP_PACKAGE_VERSION:
                    attribute.SetUnsignedInteger(AttributeNames[(int)packageAttribute], Header.Version, false);
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
                    if (file.ID < TextureHeader.TextureCount)
                    {
                        GetLumpInfo(file, out int width, out int height, out uint paletteSize, out _, out _, 0);
                        switch (packageAttribute)
                        {
                            case PackageAttributeType.HL_BSP_ITEM_WIDTH:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], (uint)width, false);
                                return true;
                            case PackageAttributeType.HL_BSP_ITEM_HEIGHT:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], (uint)height, false);
                                return true;
                            case PackageAttributeType.HL_BSP_ITEM_PALETTE_ENTRIES:
                                attribute.SetUnsignedInteger(ItemAttributeNames[(int)packageAttribute], paletteSize, false);
                                return true;
                        }
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
            size = default;
            if (file.ID < TextureHeader.TextureCount)
            {
                if (!GetLumpInfo(file, out int width, out int height, out uint paletteSize, out _, out _, 0))
                    return false;

                size = (int)(BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4 + width * height);
            }
            else
            {
                size = (int)Header.Lumps[HL_BSP_LUMP_ENTITIES].Length - 1;
            }

            return true;
        }

        /// <inheritdoc/>
        protected override bool GetFileSizeOnDiskInternal(DirectoryFile file, out int size)
        {
            size = default;
            if (file.ID < TextureHeader.TextureCount)
            {
                if (!GetLumpInfo(file, out int width, out int height, out uint paletteSize, out _, out _, 0))
                    return false;

                int pixelSize = 0;
                for (int i = 0; i < HL_BSP_MIPMAP_COUNT; i++)
                {
                    //if(pTexture.lpOffsets[i] != 0)
                    {
                        pixelSize += (width >> i) * (height >> i);
                    }
                }

                size = (int)(BSPTexture.ObjectSize + pixelSize + 2 + paletteSize * 3);
            }
            else
            {
                size = (int)Header.Lumps[HL_BSP_LUMP_ENTITIES].Length - 1;
            }

            return true;
        }

        #endregion

        #region Streams

        /// <inheritdoc/>
        protected override bool CreateStreamInternal(DirectoryFile file, bool readEncrypted, out Stream stream)
        {
            stream = null;
            if (file.ID < TextureHeader.TextureCount)
            {
                if (!GetLumpInfo(file, out int width, out int height, out uint paletteSize, out byte[] palette, out byte[] pixels, 0))
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
                fileHeader.Size = (uint)(BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4 + width * height);
                fileHeader.OffBits = (uint)(BITMAPFILEHEADER.ObjectSize + BITMAPINFOHEADER.ObjectSize + paletteSize * 4);

                infoHeader.Size = (uint)BITMAPINFOHEADER.ObjectSize;
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

                int pointer = 0;
                Array.Copy(fileHeader.Serialize(), 0, buffer, pointer, BITMAPFILEHEADER.ObjectSize); pointer += BITMAPFILEHEADER.ObjectSize;
                Array.Copy(infoHeader.Serialize(), 0, buffer, pointer, BITMAPINFOHEADER.ObjectSize); pointer += BITMAPINFOHEADER.ObjectSize;
                Array.Copy(paletteData, 0, buffer, pointer, paletteData.Length); pointer += paletteData.Length;
                Array.Copy(pixelData, 0, buffer, pointer, pixelData.Length); pointer += pixelData.Length;

                stream = new MemoryStream(buffer, bufferSize);
            }
            else
            {
                stream = new MappingStream(Mapping, Header.Lumps[HL_BSP_LUMP_ENTITIES].Offset, Header.Lumps[HL_BSP_LUMP_ENTITIES].Length - 1);
            }

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
        private bool GetLumpInfo(DirectoryFile file, out int width, out int height, out uint paletteSize, out byte[] palette, out byte[] pixels, uint mipmap)
        {
            width = 0; height = 0; paletteSize = 0;
            palette = null; pixels = null;
            if (mipmap > HL_BSP_MIPMAP_COUNT)
            {
                Console.WriteLine($"Error reading texture: invalid mipmap level {mipmap}.");
                return false;
            }

            byte[] textureViewData = TextureView.ViewData;
            int pointer = BSPTextureHeader.ObjectSize + (int)TextureHeader.Offsets[file.ID];
            BSPTexture texture = BSPTexture.Create(textureViewData, ref pointer);

            width = (int)texture.Width;
            height = (int)texture.Height;

            int pixelSize = 0;
            for (int i = 0; i < HL_BSP_MIPMAP_COUNT; i++)
            {
                if (texture.Offsets[i] != 0)
                {
                    pixelSize += (width >> i) * (height >> i);
                }
            }

            pixels = new byte[pixelSize];
            Array.Copy(textureViewData, texture.Offsets[mipmap], pixels, 0, pixelSize);

            paletteSize = BitConverter.ToUInt16(textureViewData, (int)(texture.Offsets[0] + pixelSize));

            palette = new byte[paletteSize];
            Array.Copy(textureViewData, texture.Offsets[0] + pixelSize + 2, palette, 0, paletteSize);

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