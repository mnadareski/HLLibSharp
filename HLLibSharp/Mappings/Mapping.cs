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
using System.Collections.Generic;
using HLLib.Directory;

namespace HLLib.Mappings
{
    /// <summary>
    /// View to stream mapping object
    /// </summary>
    public abstract class Mapping
    {
        #region Fields

        /// <summary>
        /// List of registered views in the mapping
        /// </summary>
        public List<View> Views { get; protected set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Mapping()
        {
            Views = null;
        }

        #region Descriptors

        /// <summary>
        /// Internal mapping type
        /// </summary>
        public abstract MappingType MappingType { get; }

        /// <summary>
        /// File name represented by this mapping
        /// </summary>
        public virtual string FileName { get; } = string.Empty;

        /// <summary>
        /// Determines if the mapping is usable or not
        /// </summary>
        public abstract bool Opened { get; }

        /// <summary>
        /// File mode that the mapping was opened with
        /// </summary>
        public abstract FileModeFlags FileMode { get; }

        /// <summary>
        /// Size of the current mapping
        /// </summary>
        public abstract long MappingSize { get; }

        #endregion

        #region Opening and Closing

        /// <summary>
        /// Open a mapping with a given file mode and overwrite flag
        /// </summary>
        /// <param name="fileMode">FileMode representing mapping access</param>
        /// <param name="overwrite">True to enable overwrite on output, false otherwise</param>
        /// <returns>True if the mapping could be opened, false otherwise</returns>
        public bool Open(FileModeFlags fileMode, bool overwrite)
        {
            Close();

            if (OpenInternal(fileMode, overwrite))
            {
                Views = new List<View>();
                return true;
            }
            else
            {
                CloseInternal();
                return false;
            }
        }

        /// <summary>
        /// Close the current mapping
        /// </summary>
        public void Close()
        {
            if (Views != null)
            {
                for (int i = 0; i < Views.Count; i++)
                {
                    UnmapInternal(Views[i]);
                }

                Views = null;
            }

            CloseInternal();
        }

        /// <summary>
        /// Per-mapping implementation of mapping opening
        /// </summary>
        /// <param name="fileMode">FileMode representing mapping access</param>
        /// <param name="overwrite">True to enable overwrite on output, false otherwise</param>
        /// <returns>True if the mapping could be opened, false otherwise</returns>
        protected abstract bool OpenInternal(FileModeFlags fileMode, bool overwrite);

        /// <summary>
        /// Per-mapping implementation of mapping closing
        /// </summary>
        protected abstract void CloseInternal();

        #endregion

        #region Mapping

        /// <summary>
        /// Create a view mapping from an offset and length
        /// </summary>
        /// <param name="view">Possibly existing view to map</param>
        /// <param name="offset">Offset in the source to create the view</param>
        /// <param name="length">Length of the view</param>
        /// <returns>True if the mapping was successful, false otherwise</returns>
        public bool Map(ref View view, long offset, int length)
        {
            if (!Opened)
            {
                Console.WriteLine("Mapping not open.");
                return false;
            }

            if (view != null && view.Mapping != this)
            {
                Console.WriteLine("View does not belong to mapping.");
                return false;
            }

            if (Unmap(view) && MapInternal(offset, length, ref view))
            {
                Views.Add(view);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Unmap a view from this mapping
        /// </summary>
        /// <param name="view">Possibly existing view to unmap</param>
        /// <returns>True if the unmapping was successful, false otherwise</returns>
        public bool Unmap(View view)
        {
            if (view == null)
                return true;

            if (Opened && view.Mapping == this)
            {
                for (int i = 0; i < Views.Count; i++)
                {
                    if (Views[i] == view)
                    {
                        UnmapInternal(view);
                        view = null;
                        Views.RemoveAt(i);
                        return true;
                    }
                }
            }

            Console.WriteLine("View does not belong to mapping.");
            return false;
        }

        /// <summary>
        /// Commit changes to a view
        /// </summary>
        /// <param name="view">View to commit changes for</param>
        /// <returns>True if changes could be commited, false otherwise</returns>
        public bool Commit(View view) => Commit(view, 0, view.Length);

        /// <summary>
        /// Commit changes to a view
        /// </summary>
        /// <param name="view">View to commit changes for</param>
        /// <param name="offset">New offset to update</param>
        /// <param name="length">New length to update</param>
        /// <returns>True if changes could be commited, false otherwise</returns>
        public bool Commit(View view, long offset, long length)
        {
            if (!Opened || view.Mapping != this)
            {
                Console.WriteLine("View does not belong to mapping.");
                return false;
            }

            if (offset + length > view.Length)
            {
                Console.WriteLine($"Requested range ({offset}, {length}) does not fit inside view, (0, {view.Length}).");
                return false;
            }

            if (FileMode.HasFlag(FileModeFlags.HL_MODE_WRITE))
            {
                // Not in write mode so nothing to do.
                return true;
            }

            return CommitInternal(view, offset, length);
        }

        /// <summary>
        /// Per-mapping implementation of mapping
        /// </summary>
        /// <param name="offset">Offset in the source to create the view</param>
        /// <param name="length">Length of the view</param>
        /// <param name="view">Output view generated</param>
        /// <returns>True if the mapping was successful, false otherwise</returns>
        protected abstract bool MapInternal(long offset, int length, ref View view);

        /// <summary>
        /// Per-mapping implementation of unmapping
        /// </summary>
        /// <param name="view">Possibly existing view to unmap</param>
        protected virtual void UnmapInternal(View view) { }

        /// <summary>
        /// Per-mapping implementation of committing changes
        /// </summary>
        /// <param name="view">View to commit changes for</param>
        /// <param name="offset">New offset to update</param>
        /// <param name="length">New length to update</param>
        /// <returns>True if changes could be commited, false otherwise</returns>
        protected virtual bool CommitInternal(View view, long offset, long length) => true;

        #endregion

        #region Reading and Writing

        /// <summary>
        /// Read a block of data from the mapping source
        /// </summary>
        /// <param name="offset">Offset to read data from</param>
        /// <param name="length">Length of the data to read</param>
        /// <returns>Byte array representing the data, null on error</returns>
        public abstract byte[] Read(long offset, int length);

        /// <summary>
        /// Write a block of data to the mapping source
        /// </summary>
        /// <param name="data">Byte array representing the data to write</param>
        /// <param name="offset">Offset to write data to</param>
        /// <returns>True if the data could be written, false otherwise</returns>
        public abstract bool Write(byte[] data, long offset);

        #endregion
    }
}