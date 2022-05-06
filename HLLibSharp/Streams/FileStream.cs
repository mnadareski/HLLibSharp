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

namespace HLLib.Streams
{
    public sealed class FileStream : Stream
    {
        #region Fields

        public System.IO.FileStream InternalStream { get; private set; }

        public string OriginalFileName { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public FileStream(string fileName) : base()
        {
            OriginalFileName = fileName;

            InternalStream = null;
        }

        ~FileStream()
        {
            Close();
            OriginalFileName = null;
        }

        #region Descriptors

        /// <inheritdoc/>
        public override bool Opened => InternalStream != null;

        /// <inheritdoc/>
        public override StreamType StreamType => StreamType.HL_STREAM_FILE;

        /// <inheritdoc/>
        public override string FileName => OriginalFileName;

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
            Close();

            System.IO.FileAccess dwDesiredAccess = 0;
            if (fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                dwDesiredAccess = System.IO.FileAccess.ReadWrite;
            else if (fileMode.HasFlag(FileModeFlags.HL_MODE_READ))
                dwDesiredAccess = System.IO.FileAccess.Read;
            else if (fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                dwDesiredAccess = System.IO.FileAccess.Write;

            System.IO.FileShare dwShareMode = 0;
            if (fileMode.HasFlag(FileModeFlags.HL_MODE_VOLATILE))
                dwShareMode = System.IO.FileShare.ReadWrite;
            else if (fileMode.HasFlag(FileModeFlags.HL_MODE_READ) && !fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                dwShareMode = System.IO.FileShare.Read;

            System.IO.FileMode dwCreationDisposition = 0;
            if (fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE) && fileMode.HasFlag(FileModeFlags.HL_MODE_CREATE))
                dwCreationDisposition = System.IO.FileMode.Create;
            else if (fileMode.HasFlag(FileModeFlags.HL_MODE_READ) || fileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                dwCreationDisposition = System.IO.FileMode.Open;

            if (dwDesiredAccess == 0 || dwCreationDisposition == 0)
            {
                Console.WriteLine($"Invalid open mode ({fileMode}).");
                return false;
            }

            InternalStream = System.IO.File.Open(OriginalFileName, dwCreationDisposition, dwDesiredAccess, dwShareMode);
            if (InternalStream == null)
            {
                Console.WriteLine("Error opening file.");
                InternalStream = null;
                return false;
            }

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
        public override long Seek(long offset, System.IO.SeekOrigin seekMode)
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