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
using HLLib.Streams;
using SeekOrigin = System.IO.SeekOrigin;

namespace HLLib.Mappings
{
    /// <summary>
    /// Mapping based on a stream
    /// </summary>
    public sealed class StreamMapping : Mapping
    {
        #region Fields

        /// <summary>
        /// Base stream object
        /// </summary>
        public Stream Stream { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="stream">Stream to create mapping from</param>
        public StreamMapping(Stream stream) : base()
        {
            Stream = stream;
            Stream.Close();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~StreamMapping() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override MappingType MappingType => MappingType.HL_MAPPING_STREAM;

        /// <inheritdoc/>
        public override string FileName => Stream.FileName;

        /// <inheritdoc/>
        public override bool Opened => Stream.Opened;

        /// <inheritdoc/>
        public override FileModeFlags FileMode => Stream.FileMode;

        /// <inheritdoc/>
        public override long MappingSize => Stream.Length;

        #endregion

        #region Opening and Closing

        /// <inheritdoc/>
        protected override bool OpenInternal(FileModeFlags fileMode, bool overwrite)
        {
            if (Opened)
                return false;

            if (!fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            return Stream.Open(fileMode);
        }

        /// <inheritdoc/>
        protected override void CloseInternal() => Stream.Close();

        #endregion

        #region Mapping

        /// <inheritdoc/>
        protected override bool MapInternal(long offset, int length, ref View view)
        {
            if (!Opened)
                return false;

            // If we have an invalid offset
            if (offset < 0 || offset >= Stream.Length)
                return false;

            // If we have an invalid length
            if (offset + length > Stream.Length)
            {
                Console.WriteLine($"Requested view ({offset}, {length}) does not fit inside mapping, (0, {Stream.Length}).");
                return false;
            }

            view = new View(this, offset, length);
            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapInternal(View view)
        {
            if (!Opened)
                return;

            if (view.Mapping != this)
                return;
        }

        /// <inheritdoc/>
        protected override bool CommitInternal(View view, long offset, long length)
        {
            if (!Opened)
                return false;

            long fileOffset = view.Offset + offset;

            if (Stream.Seek(fileOffset, SeekOrigin.Begin) != fileOffset)
                return false;

            if (Stream.Write(view.ViewData, offset, length) != length)
                return false;

            return true;
        }

        #endregion

        #region Reading and Writing

        /// <inheritdoc/>
        public override byte[] Read(long offset, int length)
        {
            if (offset < 0 || offset >= Stream.Length)
                return null;

            if (length < 0 || length > Stream.Length)
                return null;

            if (offset + length > Stream.Length)
                return null;

            long currentPosition = Stream.Pointer;
            Stream.Seek(offset, SeekOrigin.Begin);

            byte[] buffer = new byte[length];
            Stream.Read(buffer, 0, length);

            Stream.Seek(currentPosition, SeekOrigin.Begin);
            return buffer;
        }

        /// <inheritdoc/>
        public override bool Write(byte[] data, long offset)
        {
            if (offset < 0 || offset >= Stream.Length)
                return false;

            if (data == null || data.Length == 0 || data.Length > Stream.Length)
                return false;

            if (offset + data.Length > Stream.Length)
                return false;

            long currentPosition = Stream.Pointer;
            Stream.Seek(offset, SeekOrigin.Begin);

            Stream.Write(data, 0, data.Length);

            Stream.Seek(currentPosition, SeekOrigin.Begin);
            return true;
        }

        #endregion
    }
}
