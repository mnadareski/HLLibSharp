/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System.IO;
using HLLib.Directory;

namespace HLLib.Streams
{
    public abstract class Stream
    {
        #region Fields

        /// <summary>
        /// Internal value representing if the stream is opened or not
        /// </summary>
        public bool InternalOpened { get; protected set; }

        /// <summary>
        /// File mode that the stream was opened with
        /// </summary>
        public FileModeFlags FileMode { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Stream()
        {
            InternalOpened = false;
            FileMode = FileModeFlags.HL_MODE_INVALID;
        }

        #region Descriptors

        /// <summary>
        /// Value representing if the stream is usable
        /// </summary>
        public virtual bool Opened => InternalOpened;

        /// <summary>
        /// Internal stream type
        /// </summary>
        public abstract StreamType StreamType { get; }

        /// <summary>
        /// File name associated with the stream
        /// </summary>
        public abstract string FileName { get; }

        /// <summary>
        /// Length of the stream
        /// </summary>
        public abstract long Length { get; }

        /// <summary>
        /// Pointer to the current stream location
        /// </summary>
        public abstract long Pointer { get; }

        #endregion

        #region Stream Operations

        /// <summary>
        /// Open the stream with a given mode
        /// </summary>
        /// <param name="fileMode">FileMode to open the stream with</param>
        /// <returns>True if the stream could be opened, false otherwise</returns>
        public abstract bool Open(FileModeFlags fileMode);

        /// <summary>
        /// Close the stream
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// Seek to a position in the stream
        /// </summary>
        /// <param name="offset">Offset to seek to</param>
        /// <param name="seekMode">Mode to use for seeking</param>
        /// <returns>New position within the stream</returns>
        public abstract long Seek(long offset, SeekOrigin seekMode);

        /// <summary>
        /// Read a single character from the stream
        /// </summary>
        /// <param name="chr">Character read from the stream</param>
        /// <returns>True if the read was successful, false otherwise</returns>
        public abstract bool Read(out char chr);

        /// <summary>
        /// Read a byte array from the stream
        /// </summary>
        /// <param name="data">Buffer to read into</param>
        /// <param name="offset">Offset within the buffer to read to</param>
        /// <param name="bytes">Number of bytes to read</param>
        /// <returns>True if the read was successful, false otherwise</returns>
        public abstract int Read(byte[] data, long offset, long bytes);

        /// <summary>
        /// Write a single character to the stream
        /// </summary>
        /// <param name="chr">Character to write to the stream</param>
        /// <returns>True if the write was successful, false otherwise</returns>
        public abstract bool Write(char chr);

        /// <summary>
        /// Write a byte array to the stream
        /// </summary>
        /// <param name="data">Buffer to write from</param>
        /// <param name="offset">Offset within the buffer to write from</param>
        /// <param name="bytes">Number of bytes to write</param>
        /// <returns>True if the write was successful, false otherwise</returns>
        public abstract int Write(byte[] data, long offset, long bytes);

        #endregion
    }
}