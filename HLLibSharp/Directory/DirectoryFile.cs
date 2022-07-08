/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using HLLib.Packages;
using static HLLib.Utility;

namespace HLLib.Directory
{
    /// <summary>
    /// File contained within a package
    /// </summary>
    public sealed class DirectoryFile : DirectoryItem
    {
        #region Constants

        /// <summary>
        /// Default buffer size for extraction
        /// </summary>
        public const int HL_DEFAULT_COPY_BUFFER_SIZE = 131072;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public DirectoryFile(string name, uint id, byte[] data, Package package, DirectoryFolder parent)
            : base(name, id, data, package, parent)
        { }

        #region Descriptors

        /// <inheritdoc/>
        public override DirectoryItemType ItemType => DirectoryItemType.HL_ITEM_FILE;

        /// <summary>
        /// Determine if the file is extractable or not
        /// </summary>
        public bool Extractable
        {
            get
            {
                Package.GetFileExtractable(this, out bool extractable);
                return extractable;
            }
        }

        /// <summary>
        /// Internal size of the file
        /// </summary>
        public int Size
        {
            get
            {
                Package.GetFileSize(this, out int size);
                return size;
            }
        }

        /// <summary>
        /// Extracted size of the file
        /// </summary>
        public int SizeOnDisk
        {
            get
            {
                Package.GetFileSizeOnDisk(this, out int size);
                return size;
            }
        }

        /// <summary>
        /// Validation value for the file
        /// </summary>
        public Validation Validation
        {
            get
            {
                Package.GetFileValidation(this, out Validation validation);
                return validation;
            }
        }

        #endregion

        #region Streams

        /// <summary>
        /// Create a stream for this item from the source package
        /// </summary>
        /// <param name="readEncrypted">True to read encrypted files, false otherwise</param>
        /// <param name="stream">Stream created by the source package</param>
        /// <returns>True if the stream could be created, false otherwise</returns>
        public bool CreateStream(bool readEncrypted, out Streams.Stream stream) => Package.CreateStream(this, readEncrypted, out stream);

        /// <summary>
        /// Release the stream for this item from the source package
        /// </summary>
        /// <param name="stream">Stream created by the source package</param>
        public void ReleaseStream(Streams.Stream stream) => Package.ReleaseStream(stream);

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool Extract(string path, bool readEncrypted = true, bool overwrite = true)
        {
            string name = Name;
            name = RemoveIllegalCharacters(name);

            string fileName;
            if (string.IsNullOrEmpty(path) || path == "\0")
                fileName = name;
            else
                fileName = System.IO.Path.Combine(path.Trim(), name);

            fileName = FixupIllegalCharacters(fileName);

            bool result;
            if (!overwrite && System.IO.File.Exists(fileName))
            {
                result = true;
            }
            else
            {
                result = false;

                if (Package.CreateStream(this, readEncrypted, out Streams.Stream input))
                {
                    if (input.Open(FileModeFlags.HL_MODE_READ | FileModeFlags.HL_MODE_VOLATILE))
                    {
                        Streams.FileStream output = new Streams.FileStream(fileName);

                        if (output.Open(FileModeFlags.HL_MODE_WRITE | FileModeFlags.HL_MODE_CREATE))
                        {
                            int totalBytes = 0;
                            byte[] buffer = new byte[HL_DEFAULT_COPY_BUFFER_SIZE];

                            while (true)
                            {
                                int bytesRead = input.Read(buffer, 0, buffer.Length);
                                if (bytesRead == 0)
                                {
                                    result = totalBytes == input.Length;
                                    break;
                                }

                                if (output.Write(buffer, 0, bytesRead) != bytesRead)
                                    break;

                                totalBytes += bytesRead;
                            }

                            output.Close();
                        }

                        input.Close();
                    }

                    Package.ReleaseStream(input);
                }
            }

            return result;
        }

        #endregion
    }
}