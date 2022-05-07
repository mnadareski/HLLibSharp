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
using System.IO;
using HLLib.Directory;
using HLLib.Mappings;
using HLLib.Packages.GCF;

namespace HLLib.Streams
{
    public sealed class GCFStream : Stream
    {
        #region Fields

        /// <summary>
        /// Source GCF package
        /// </summary>
        public GCFFile Package { get; private set; }

        /// <summary>
        /// Package file ID
        /// </summary>
        public uint FileID { get; private set; }

        /// <summary>
        /// Block entry index within the package
        /// </summary>
        public uint BlockEntryIndex { get; private set; }

        /// <summary>
        /// Block entry offset within the package
        /// </summary>
        public long BlockEntryOffset { get; private set; }
        
        /// <summary>
        /// Data block index within the package
        /// </summary>
        public uint DataBlockIndex { get; private set; }

        /// <summary>
        /// Data block offset within the package
        /// </summary>
        public long DataBlockOffset { get; private set; }

        /// <summary>
        /// Current internal pointer
        /// </summary>
        public long InternalPointer { get; private set; }

        /// <summary>
        /// Total internal length
        /// </summary>
        public long InternalLength { get; private set; }

        /// <summary>
        /// View representing the data
        /// </summary>
        private View View;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public GCFStream(GCFFile gcfFile, uint fileID) : base()
        {
            Package = gcfFile;
            FileID = fileID;

            View = null;
            InternalPointer = 0;
            InternalLength = 0;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~GCFStream() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override StreamType StreamType => StreamType.HL_STREAM_GCF;

        /// <inheritdoc/>
        public override string FileName => Package.DirectoryName((int)Package.DirectoryEntries[FileID].NameOffset);

        /// <inheritdoc/>
        public override long Length => InternalLength;

        /// <inheritdoc/>
        public override long Pointer => InternalPointer;

        #endregion

        #region Stream Operations

        /// <inheritdoc/>
        public override bool Open(FileModeFlags fileMode)
        {
            Close();

            if(!Package.Opened)
            {
                Console.WriteLine("GCF file not opened.");
                return false;
            }

            if(!fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            if(fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !Package.Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("GCF file does not have read permissions.");
                return false;
            }

            if(fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE) && !Package.Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("GCF file does not have write permissions.");
                return false;
            }

            InternalPointer = 0;
            InternalLength = fileMode.HasFlag(FileModeFlags.HL_MODE_READ) ? Package.DirectoryEntries[FileID].ItemSize : 0;

            InternalOpened = true;
            FileMode = fileMode;

            BlockEntryIndex = Package.DirectoryMapEntries[FileID].FirstBlockIndex;
            BlockEntryOffset = 0;
            DataBlockIndex = Package.BlockEntries[BlockEntryIndex].FirstDataBlockIndex;
            DataBlockOffset = 0;

            return true;
        }

        /// <inheritdoc/>
        public override void Close()
        {
            InternalOpened = false;
            FileMode = FileModeFlags.HL_MODE_INVALID;

            Package.Mapping.Unmap(ref View);

            InternalPointer = 0;
            InternalLength = 0;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin seekMode)
        {
            if (!InternalOpened)
                return 0;

            switch (seekMode)
            {
                case SeekOrigin.Begin:
                    InternalPointer = 0;
                    break;
                case SeekOrigin.Current:
                    break;
                case SeekOrigin.End:
                    InternalPointer = InternalLength;
                    break;
            }

            long iPointer = InternalPointer + offset;
            if (iPointer < 0)
                iPointer = 0;
            else if (iPointer > InternalLength)
                iPointer = InternalLength;

            InternalPointer = iPointer;
            return InternalPointer;
        }

        /// <inheritdoc/>
        public override bool Read(out char chr)
        {
            chr = default;
            if (!InternalOpened)
                return false;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("Stream not in read mode.");
                return false;
            }

            if (InternalPointer < InternalLength)
            {
                if (!Map(InternalPointer))
                    return false;

                long viewPointer = InternalPointer - (BlockEntryOffset + DataBlockOffset);
                long viewBytes = View.Length - viewPointer;

                if (viewBytes >= 1)
                {
                    chr = (char)View.ViewData[viewPointer];
                    InternalPointer++;
                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override int Read(byte[] data, long offset, long bytes)
        {
            if (!InternalOpened)
                return 0;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("Stream not in read mode.");
                return 0;
            }

            if (InternalPointer == InternalLength)
            {
                return 0;
            }
            else
            {
                while (bytes != 0 && InternalPointer < InternalLength)
                {
                    if (!Map(InternalPointer))
                        break;

                    long viewPointer = InternalPointer - (BlockEntryOffset + DataBlockOffset);
                    long viewBytes = View.Length - viewPointer;

                    if (viewBytes >= bytes)
                    {
                        Array.Copy(View.ViewData, viewPointer, data, offset, bytes);
                        InternalPointer += bytes;
                        offset += bytes;
                        break;
                    }
                    else
                    {
                        Array.Copy(View.ViewData, viewPointer, data, offset, viewBytes);
                        InternalPointer += viewBytes;
                        offset += viewBytes;
                        bytes -= (int)viewBytes;
                    }
                }

                return (int)offset;
            }
        }

        /// <inheritdoc/>
        public override bool Write(char chr)
        {
            if (!InternalOpened)
                return false;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Stream not in write mode.");
                return false;
            }

            if (InternalPointer < Package.DirectoryEntries[FileID].ItemSize)
            {
                if (!Map(InternalPointer))
                    return false;

                long viewPointer = InternalPointer - (BlockEntryOffset + DataBlockOffset);
                long viewBytes = View.Length - viewPointer;

                if (viewBytes >= 1)
                {
                    View.ViewData[viewPointer] = (byte)chr;
                    InternalPointer++;

                    if (InternalPointer > InternalLength)
                        InternalLength = InternalPointer;

                    return true;
                }
            }

            return false;
        }

        /// <inheritdoc/>
        public override int Write(byte[] data, long offset, long bytes)
        {
            if (!InternalOpened)
                return 0;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Stream not in write mode.");
                return 0;
            }

            if (InternalPointer == Package.DirectoryEntries[FileID].ItemSize)
            {
                return 0;
            }
            else
            {
                while (bytes != 0 && InternalPointer < Package.DirectoryEntries[FileID].ItemSize)
                {
                    if (!Map(InternalPointer))
                        break;

                    long viewPointer = InternalPointer - (BlockEntryOffset + DataBlockOffset);
                    long viewBytes = View.Length - viewPointer;

                    if (viewBytes >= bytes)
                    {
                        Array.Copy(data, offset, View.ViewData, viewPointer, bytes);
                        InternalPointer += bytes;
                        offset += bytes;
                        break;
                    }
                    else
                    {
                        Array.Copy(data, offset, View.ViewData, viewPointer, viewBytes);
                        InternalPointer += viewBytes;
                        offset += viewBytes;
                        bytes -= (int)viewBytes;
                    }
                }

                if (InternalPointer > InternalLength)
                    InternalLength = InternalPointer;

                return (int)offset;
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Map a new pointer based on the stream
        /// </summary>
        /// <param name="pointer">New pointer to map</param>
        /// <returns>True if the mapping was successful, false otherwise</returns>
        private bool Map(long pointer)
        {
            if (pointer < BlockEntryOffset + DataBlockOffset)
            {
                BlockEntryIndex = Package.DirectoryMapEntries[FileID].FirstBlockIndex;
                BlockEntryOffset = 0;
                DataBlockIndex = Package.BlockEntries[FileID].FirstDataBlockIndex;
                DataBlockOffset = 0;
            }

            int length = (int)(DataBlockOffset + Package.DataBlockHeader.BlockSize > Package.BlockEntries[BlockEntryIndex].FileDataSize ? Package.BlockEntries[BlockEntryIndex].FileDataSize - DataBlockOffset : Package.DataBlockHeader.BlockSize);
            //int uiDataBlockTerminator = gcfFile.DataBlockHeader.BlockCount >= 0x0000FFFF ? 0xFFFFFFFF : 0x0000FFFF;
            uint uiDataBlockTerminator = Package.FragmentationMapHeader.Terminator == 0 ? 0x0000FFFF : 0xFFFFFFFF;

            while ((pointer > BlockEntryOffset + DataBlockOffset + length) && BlockEntryIndex != Package.DataBlockHeader.BlockCount)
            {
                // Loop through each data block fragment.
                while ((pointer >= BlockEntryOffset + DataBlockOffset + length) && (DataBlockIndex < uiDataBlockTerminator && DataBlockOffset < Package.BlockEntries[BlockEntryIndex].FileDataSize))
                {
                    // Get the next data block fragment.
                    DataBlockIndex = Package.FragmentationMaps[DataBlockIndex].NextDataBlockIndex;
                    DataBlockOffset += Package.DataBlockHeader.BlockSize;

                    length = (int)(DataBlockOffset + Package.DataBlockHeader.BlockSize > Package.BlockEntries[BlockEntryIndex].FileDataSize ? Package.BlockEntries[BlockEntryIndex].FileDataSize - DataBlockOffset : Package.DataBlockHeader.BlockSize);
                }

                if (DataBlockOffset >= Package.BlockEntries[BlockEntryIndex].FileDataSize)
                {
                    // Get the next data block.
                    BlockEntryOffset += Package.BlockEntries[BlockEntryIndex].FileDataSize;
                    BlockEntryIndex = Package.BlockEntries[BlockEntryIndex].NextBlockEntryIndex;

                    DataBlockOffset = 0;
                    if (BlockEntryIndex != Package.DataBlockHeader.BlockCount)
                        DataBlockIndex = Package.BlockEntries[BlockEntryIndex].FirstDataBlockIndex;

                    length = (int)(DataBlockOffset + Package.DataBlockHeader.BlockSize > Package.BlockEntries[BlockEntryIndex].FileDataSize ? Package.BlockEntries[BlockEntryIndex].FileDataSize - DataBlockOffset : Package.DataBlockHeader.BlockSize);
                }
            }

            if (BlockEntryIndex == Package.DataBlockHeader.BlockCount || DataBlockIndex >= uiDataBlockTerminator)
            {
                if (BlockEntryOffset + DataBlockOffset < Package.DirectoryEntries[FileID].ItemSize)
                    Console.WriteLine($"Unexpected end of GCF stream ({BlockEntryOffset + DataBlockOffset} B of {Package.DirectoryEntries[FileID].ItemSize} B).  Has the GCF file been completely acquired?");

                Package.Mapping.Unmap(ref View);
                return false;
            }

            return Package.Mapping.Map(ref View, Package.DataBlockHeader.FirstBlockOffset + DataBlockIndex * Package.DataBlockHeader.BlockSize, length);
        }

        #endregion
    }
}