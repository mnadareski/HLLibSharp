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

namespace HLLib.Mappings
{
    /// <summary>
    /// Mapping based on a physical file
    /// </summary>
    public sealed class FileMapping : Mapping
    {
        #region Fields

        /// <summary>
        /// Base file stream object
        /// </summary>
        public FileStream FileStream { get; private set; }

        /// <summary>
        /// Determines the current file mode
        /// </summary>
        public FileModeFlags InternalFileMode { get; private set; }

        /// <summary>
        /// Original filename
        /// </summary>
        public string OriginalFileName { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="fileName">File to create mapping from</param>
        public FileMapping(string fileName) : base()
        {
            FileStream = null;
            InternalFileMode = FileModeFlags.HL_MODE_INVALID;

            OriginalFileName = fileName;
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~FileMapping()
        {
            Close();
            OriginalFileName = null;
        }

        #region Descriptors

        /// <inheritdoc/>
        public override MappingType MappingType => MappingType.HL_MAPPING_FILE;

        /// <inheritdoc/>
        public override string FileName => OriginalFileName;

        /// <inheritdoc/>
        public override bool Opened => FileStream != null;

        /// <inheritdoc/>
        public override FileModeFlags FileMode => InternalFileMode;

        /// <inheritdoc/>
        public override long MappingSize => Opened ? FileStream.Length : 0;

        #endregion

        #region Opening and Closing

        /// <inheritdoc/>
        protected override bool OpenInternal(FileModeFlags uiMode, bool overwrite)
        {
            if (!Opened)
                throw new ArgumentException("Stream must be initialized before opening");

            FileAccess fileAccess = 0;
            if (uiMode.HasFlag(FileModeFlags.HL_MODE_READ) && uiMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                fileAccess = FileAccess.ReadWrite;
            else if (uiMode.HasFlag(FileModeFlags.HL_MODE_READ))
                fileAccess = FileAccess.Read;
            else if (uiMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                fileAccess = FileAccess.Write;

            FileShare fileShare = 0;
            if (uiMode.HasFlag(FileModeFlags.HL_MODE_VOLATILE))
                fileShare = FileShare.ReadWrite;
            else if (uiMode.HasFlag(FileModeFlags.HL_MODE_READ) && !uiMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                fileShare = FileShare.Read;

            FileMode fileMode = 0;
            if (uiMode.HasFlag(FileModeFlags.HL_MODE_WRITE) && uiMode.HasFlag(FileModeFlags.HL_MODE_CREATE))
                fileMode = overwrite ? System.IO.FileMode.Create : System.IO.FileMode.CreateNew;
            else if (uiMode.HasFlag(FileModeFlags.HL_MODE_READ) || uiMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
                fileMode = System.IO.FileMode.Open;

            if (fileAccess == 0 || fileMode == 0)
            {
                Console.WriteLine($"Invalid open mode ({uiMode}).");
                return false;
            }

            FileStream = File.Open(OriginalFileName, fileMode, fileAccess, fileShare);
            if (FileStream == null)
            {
                Console.WriteLine("Error opening file.");
                FileStream = null;
                return false;
            }

            InternalFileMode = uiMode;
            return true;
        }

        /// <inheritdoc/>
        protected override void CloseInternal()
        {
            FileStream?.Close();
            FileStream = null;
        }

        #endregion

        #region Mapping

        /// <inheritdoc/>
        protected override bool MapInternal(long offset, int length, ref View view)
        {
            view = null;
            if (!Opened)
                throw new ArgumentException("Stream must be initialized before mapping");

            // If we have an invalid offset
            if (offset < 0 || offset >= FileStream.Length)
                return false;

            // If we have an invalid lenght
            if (offset + length > (FileStream.Length - offset))
                return false;

            view = new View(this, offset + FileStream.Position, length);
            return true;
        }

        /// <inheritdoc/>
        protected override void UnmapInternal(View view)
        {
            if (!Opened)
                throw new ArgumentException("Stream must be initialized before unmapping");

            if (view.Mapping != this)
                throw new ArgumentException("Tried to unmap from an invalid parent");
        }

        #endregion

        #region Reading and Writing

        /// <inheritdoc/>
        public override byte[] Read(long offset, int length)
        {
            if (offset < 0 || offset >= FileStream.Length)
                return null;

            if (length < 0 || length > FileStream.Length)
                return null;

            if (offset + length >= FileStream.Length)
                return null;

            long currentPosition = FileStream.Position;
            FileStream.Seek(offset, SeekOrigin.Begin);

            byte[] buffer = new byte[length];
            FileStream.Read(buffer, 0, length);

            FileStream.Seek(currentPosition, SeekOrigin.Begin);
            return buffer;
        }

        /// <inheritdoc/>
        public override bool Write(byte[] data, long offset)
        {
            if (offset < 0 || offset >= FileStream.Length)
                return false;

            if (data == null || data.Length == 0 || data.Length > FileStream.Length)
                return false;

            if (offset + data.Length >= FileStream.Length)
                return false;

            long currentPosition = FileStream.Position;
            FileStream.Seek(offset, SeekOrigin.Begin);

            FileStream.Write(data, 0, data.Length);

            FileStream.Seek(currentPosition, SeekOrigin.Begin);
            return true;
        }

        #endregion
    }
}