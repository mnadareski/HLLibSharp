/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Directory;
using SeekOrigin = System.IO.SeekOrigin;

namespace HLLib.Streams
{
    public sealed class NullStream : Stream
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public NullStream() : base()
        {
        }

        #region Descriptors

        /// <inheritdoc/>
        public override StreamType StreamType => StreamType.HL_STREAM_NULL;

        /// <inheritdoc/>
        public override string FileName => string.Empty;

        /// <inheritdoc/>
        public override long Length => 0;

        /// <inheritdoc/>
        public override long Pointer => 0;

        #endregion

        #region Stream Operations

        /// <inheritdoc/>
        public override bool Open(FileModeFlags fileMode)
        {
            InternalOpened = true;
            FileMode = fileMode;
            return true;
        }

        /// <inheritdoc/>
        public override void Close()
        {
            InternalOpened = false;
            FileMode = FileModeFlags.HL_MODE_INVALID;
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin seekMode) => 0;

        /// <inheritdoc/>
        public override bool Read(out char chr)
        {
            chr = default;
            return false;
        }

        /// <inheritdoc/>
        public override int Read(byte[] data, long offset, long bytes) => 0;

        /// <inheritdoc/>
        public override bool Write(char chr) => false;

        /// <inheritdoc/>
        public override int Write(byte[] data, long offset, long bytes) => 0;

        #endregion
    }
}
