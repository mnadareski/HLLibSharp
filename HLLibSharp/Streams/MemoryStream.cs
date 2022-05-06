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

namespace HLLib.Streams
{
    public sealed class MemoryStream : Stream
    {
        #region Fields

        public System.IO.MemoryStream InternalStream { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public MemoryStream(byte[] data, int bufferSize) : base()
        {
            InternalStream = new System.IO.MemoryStream(data, 0, bufferSize, true);
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~MemoryStream() => Close();

        #region Descriptors

        /// <inheritdoc/>
        public override StreamType StreamType => StreamType.HL_STREAM_MEMORY;

        /// <inheritdoc/>
        public override string FileName => string.Empty;

        /// <inheritdoc/>
        public override long Length
        {
            get
            {
                if (!Opened)
                    return 0;

                return InternalStream.Length;
            }
        }

        /// <inheritdoc/>
        public override long Pointer
        {
            get
            {
                if (!Opened)
                    return 0;

                return InternalStream.Position;
            }
        }

        #endregion

        #region Stream Operations

        /// <inheritdoc/>
        public override bool Open(FileModeFlags fileMode)
        {
            if (InternalStream.Capacity != 0 && InternalStream == null)
            {
                Console.WriteLine("Memory stream is null.");
                return false;
            }

            if ((FileMode & (FileModeFlags.HL_MODE_READ | FileModeFlags.HL_MODE_WRITE)) == 0)
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            InternalOpened = true;
            FileMode = fileMode;

            return true;
        }

        /// <inheritdoc/>
        public override void Close()
        {
            if (Opened)
            {
                InternalStream.Close();
                InternalStream = null;
                FileMode = FileModeFlags.HL_MODE_INVALID;
            }
        }

        /// <inheritdoc/>
        public override long Seek(long offset, SeekOrigin seekMode)
        {
            if (!Opened)
                return 0;

            return InternalStream.Seek(offset, seekMode);
        }

        /// <inheritdoc/>
        public override bool Read(out char chr)
        {
            chr = default;
            if (!Opened)
                return false;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("Stream not in read mode.");
                return false;
            }

            byte[] buffer = new byte[1];
            int bytesRead = InternalStream.Read(buffer, 0, 1);
            if (bytesRead == 1)
                chr = (char)buffer[0];
            else
                Console.WriteLine("ReadFile() failed.");

            return bytesRead == 1;
        }

        /// <inheritdoc/>
        public override int Read(byte[] data, long offset, long bytes)
        {
            if (!Opened)
                return 0;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_READ))
            {
                Console.WriteLine("Stream not in read mode.");
                return 0;
            }

            int bytesRead = InternalStream.Read(data, (int)offset, (int)bytes);
            if (bytesRead == 0)
                Console.WriteLine("ReadFile() failed.");

            return bytesRead;
        }

        /// <inheritdoc/>
        public override bool Write(char chr)
        {
            if (!Opened)
                return false;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Stream not in write mode.");
                return false;
            }

            try
            {
                InternalStream.WriteByte((byte)chr);
                return true;
            }
            catch
            {
                Console.WriteLine("WriteFile() failed.");
                return false;
            }
        }

        /// <inheritdoc/>
        public override int Write(byte[] data, long offset, long bytes)
        {
            if (!Opened)
                return 0;

            if (!FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                Console.WriteLine("Stream not in write mode.");
                return 0;
            }

            try
            {
                InternalStream.Write(data, (int)offset, (int)bytes);
                return (int)bytes;
            }
            catch
            {
                Console.WriteLine("WriteFile() failed.");
                return 0;
            }
        }

        #endregion
    }
}
