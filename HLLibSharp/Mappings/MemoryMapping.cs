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

namespace HLLib.Mappings
{
    /// <summary>
    /// Mapping based on bytes in memory
    /// </summary>
    public sealed class MemoryMapping : Mapping
    {
        #region Fields

        /// <summary>
        /// Determines if the memory mapping is considered open
        /// </summary>
        public bool InternalOpened { get; private set; }

        /// <summary>
        /// Determines the current file mode
        /// </summary>
        public FileModeFlags InternalFileMode { get; private set; }

        /// <summary>
        /// Determines the backing data
        /// </summary>
        public byte[] Data { get; private set; }

        /// <summary>
        /// Determines the internal buffer size
        /// </summary>
        public long BufferSize { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="data">Data buffer to create mapping from</param>
        /// <param name="bufferSize">Total buffer size to use</param>
        public MemoryMapping(byte[] data, long bufferSize) : base()
        {
            InternalOpened = false;
            InternalFileMode = FileModeFlags.HL_MODE_INVALID;
            Data = data;
            BufferSize = bufferSize;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MemoryMapping() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override MappingType MappingType => MappingType.HL_MAPPING_MEMORY;

        public byte[] GetBuffer() => Data;

        public long GetBufferSize() => BufferSize;

        /// <inheritdoc/>
        public override bool Opened => InternalOpened;

        /// <inheritdoc/>
        public override FileModeFlags FileMode => InternalFileMode;

        /// <inheritdoc/>
        public override long MappingSize => InternalOpened ? BufferSize : 0;

        #endregion

        #region Opening and Closing

        /// <inheritdoc/>
        protected override bool OpenInternal(FileModeFlags fileMode, bool overwrite)
        {
            if (InternalOpened)
                throw new ArgumentException("Memory must be initialized before opening");

            if (BufferSize != 0 && Data == null)
            {
                Console.WriteLine("Memory stream is null.");
                return false;
            }

            if (!fileMode.HasFlag(FileModeFlags.HL_MODE_READ) || !fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            InternalOpened = true;
            InternalFileMode = fileMode;

            return true;
        }

        /// <inheritdoc/>
        protected override void CloseInternal()
        {
            InternalOpened = false;
            InternalFileMode = FileModeFlags.HL_MODE_INVALID;
        }

        #endregion

        #region Mapping

        /// <inheritdoc/>
        protected override bool MapInternal(long offset, int length, ref View view)
        {
            view = null;
            if (!Opened)
                throw new ArgumentException("Memory must be initialized before mapping");

            if (offset + length > BufferSize)
            {
                Console.WriteLine($"Requested view ({offset}, {length}) does not fit inside mapping, (0, {this.BufferSize}).");
                return false;
            }

            view = new View(this, offset, length);
            return true;
        }

        #endregion

        #region Reading and Writing

        /// <inheritdoc/>
        public override byte[] Read(long offset, int length)
        {
            if (offset < 0 || offset >= Data.Length)
                return null;

            if (length < 0 || length > Data.Length)
                return null;

            if (offset + length >= Data.Length)
                return null;

            byte[] buffer = new byte[length];
            Array.Copy(Data, offset, buffer, 0, length);
            return buffer;
        }

        /// <inheritdoc/>
        public override bool Write(byte[] data, long offset)
        {
            if (offset < 0 || offset >= Data.Length)
                return false;

            if (data == null || data.Length == 0 || data.Length > Data.Length)
                return false;

            if (offset + data.Length >= Data.Length)
                return false;

            Array.Copy(data, 0, Data, offset, data.Length);
            return true;
        }

        #endregion
    }
}
