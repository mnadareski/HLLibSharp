/*
 * HLLib
 * Copyright (C) 2006-2013 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later
 * version.
 */

using System.Collections.Generic;
using System.Text;

namespace HLLib.Packages.VPK
{
    public class VPKDirectoryItem
    {
        public string Extension { get; set; }

        public string Path { get; set; }

        public string Name { get; set; }

        public VPKDirectoryEntry DirectoryEntry { get; set; }

        public byte[] PreloadData { get; set; }

        public VPKDirectoryItem(string extension, string path, string name, VPKDirectoryEntry directoryEntry, byte[] preloadData)
        {
            Extension = extension;
            Path = path;
            Name = name;
            DirectoryEntry = directoryEntry;
            PreloadData = preloadData;
        }

        private VPKDirectoryItem()
        {
            // Private for Create constructor below
        }

        public static VPKDirectoryItem Create(byte[] data, ref int offset)
        {
            VPKDirectoryItem directoryItem = new VPKDirectoryItem();

            // Check to see if the data is valid
            if (data == null)
                return null;

            List<byte> temp = new List<byte>();
            while (data[offset] != 0)
            {
                temp.Add(data[offset++]);
            }
            directoryItem.Extension = Encoding.ASCII.GetString(temp.ToArray());
            offset++;

            temp = new List<byte>();
            while (data[offset] != 0)
            {
                temp.Add(data[offset++]);
            }
            directoryItem.Path = Encoding.ASCII.GetString(temp.ToArray());
            offset++;

            temp = new List<byte>();
            while (data[offset] != 0)
            {
                temp.Add(data[offset++]);
            }
            directoryItem.Name = Encoding.ASCII.GetString(temp.ToArray());
            offset++;

            directoryItem.DirectoryEntry = VPKDirectoryEntry.Create(data, ref offset);
            directoryItem.PreloadData = null; // TODO: Figure out how to populate this

            return directoryItem;
        }

        public byte[] Serialize()
        {
            List<byte> dataList = new List<byte>();

            dataList.AddRange(Encoding.ASCII.GetBytes(Extension)); dataList.Add(0x00);
            dataList.AddRange(Encoding.ASCII.GetBytes(Path)); dataList.Add(0x00);
            dataList.AddRange(Encoding.ASCII.GetBytes(Name)); dataList.Add(0x00);
            dataList.AddRange(DirectoryEntry.Serialize());

            return dataList.ToArray();
        }
    }
}
