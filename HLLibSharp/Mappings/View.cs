/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

namespace HLLib.Mappings
{
    /// <summary>
    /// Pointer used within a mapping
    /// </summary>
    public sealed class View
    {
        #region Fields

        /// <summary>
        /// Source mapping that this view applies to
        /// </summary>
        public Mapping Mapping { get; private set; }

        /// <summary>
        /// Offset in the mapping to start at
        /// </summary>
        public long Offset { get; internal set; }

        /// <summary>
        /// Length of the view in the mapping
        /// </summary>
        public int Length { get; internal set; }

        /// <summary>
        /// Byte array representing the view data
        /// </summary>
        public byte[] ViewData
        {
            get => GetView();
            set => SetView(value);
        }

        /// <summary>
        /// Cached view data
        /// </summary>
        private byte[] cachedViewData;

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public View(Mapping mapping, long offset, int length)
        {
            Mapping = mapping;
            Offset = offset;
            Length = length;
        }

        #region Data Reading and Writing

        /// <summary>
        /// Get the data represented by this view
        /// </summary>
        /// <returns>Byte array representing the data, if possible</returns>
        private byte[] GetView()
        {
            if (cachedViewData != null)
                return cachedViewData;

            cachedViewData = Mapping.Read(Offset, Length);
            return cachedViewData;
        }

        /// <summary>
        /// Set the data represented by this view
        /// </summary>
        /// <param name="data">Byte array representing the data to write</param>
        /// <returns>True if the data could be written, false otherwise</returns>
        /// <remarks>
        /// This method also resets the Length field, assuming the entire view is being rewritten
        /// </remarks>
        private bool SetView(byte[] data)
        {
            if (Mapping.Write(data, Offset))
            {
                cachedViewData = data;
                Length = data.Length;
                return true;
            }

            return false;
        }

        #endregion
    }
}