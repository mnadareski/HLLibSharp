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

namespace HLLib.Streams
{
    public sealed class MappingStream : Stream
    {
        #region Constants

        /// <summary>
        /// Default size for a View
        /// </summary>
        public const int HL_DEFAULT_VIEW_SIZE = 131072;

        #endregion

        #region Fields

        public Mapping Mapping { get; private set; }

        public long MappingOffset { get; private set; }

        public long MappingSize { get; private set; }

        public long ViewSize { get; private set; }

        public long InternalPointer { get; private set; }

        public long InternalLength { get; private set; }

        private View View;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MappingStream(Mapping mapping, long mappingOffset, long mappingSize, long viewSize = 0) : base()
        {
            Mapping = mapping;
            MappingOffset = mappingOffset;
            MappingSize = mappingSize;
            ViewSize = viewSize;

            View = null;
            InternalPointer = 0;
            InternalLength = 0;

            if (ViewSize == 0)
            {
                switch (Mapping.MappingType)
                {
                    case MappingType.HL_MAPPING_FILE:
                        if (Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_QUICK_FILEMAPPING))
                            ViewSize = mappingSize;

                        break;
                    case MappingType.HL_MAPPING_MEMORY:
                        ViewSize = mappingSize;
                        break;
                    default:
                        ViewSize = HL_DEFAULT_VIEW_SIZE;
                        break;
                }
            }
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MappingStream() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override StreamType StreamType => StreamType.HL_STREAM_MAPPING;

        /// <inheritdoc/>
        public override string FileName => string.Empty;

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

            if (!fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            if (fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("Mapping does not have read permissions.");
                return false;
            }

            if (fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE) && !Mapping.FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Mapping does not have write permissions.");
                return false;
            }

            InternalPointer = 0;
            InternalLength = fileMode.HasFlag(FileModeFlags.HL_MODE_READ) ? MappingSize : 0;

            InternalOpened = true;
            FileMode = fileMode;

            return true;
        }

        /// <inheritdoc/>
        public override void Close() { }

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

            long pointer = InternalPointer + offset;

            if (pointer < 0)
                pointer = 0;
            else if (pointer > InternalLength)
                pointer = InternalLength;

            InternalPointer = pointer;
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

                long viewPointer = InternalPointer - (View.Offset - MappingOffset);
                long viewBytes = View.Length - viewPointer;

                if (viewBytes >= 1)
                {
                    chr = BitConverter.ToChar(View.ViewData, (int)viewPointer);
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

                    long viewPointer = InternalPointer - (View.Offset - MappingOffset);
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
                        bytes -= viewBytes;
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

            if (InternalPointer < MappingSize)
            {
                if (!Map(InternalPointer))
                    return false;

                long viewPointer = InternalPointer - (View.Offset - MappingOffset);
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

            if (InternalPointer == MappingSize)
            {
                return 0;
            }
            else
            {
                while (bytes != 0 && InternalPointer < MappingSize)
                {
                    if (!Map(InternalPointer))
                        break;

                    long viewPointer = InternalPointer - (View.Offset - MappingOffset);
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
                        bytes -= viewBytes;
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
            pointer = (pointer / ViewSize) * ViewSize;

            if (View != null)
            {
                if (MappingOffset == pointer)
                    return true;
            }

            long Length = pointer + ViewSize > MappingSize ? MappingSize - pointer : ViewSize;
            return Mapping.Map(ref View, MappingOffset + pointer, (int)Length);
        }

        #endregion
    }
}