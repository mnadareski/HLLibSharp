/**************************************************************************
*                                                                         *
* wingdi.h -- GDI procedure declarations, constant definitions and macros *
*                                                                         *
* Copyright (c) Microsoft Corp. All rights reserved.                      *
*                                                                         *
**************************************************************************/

using System;
using System.Runtime.InteropServices;

namespace HLLib.Packages.WAD
{
    public class BITMAPFILEHEADER
    {
        public ushort Type { get; set; }

        public uint Size { get; set; }

        public ushort Reserved1 { get; set; }

        public ushort Reserved2 { get; set; }

        public uint OffBits { get; set; }

        public static BITMAPFILEHEADER Create(byte[] data, ref int offset)
        {
            BITMAPFILEHEADER fileHeader = new BITMAPFILEHEADER();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(fileHeader))
                return null;

            fileHeader.Type = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.Size = BitConverter.ToUInt32(data, offset); offset += 4;
            fileHeader.Reserved1 = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.Reserved2 = BitConverter.ToUInt16(data, offset); offset += 2;
            fileHeader.OffBits = BitConverter.ToUInt32(data, offset); offset += 4;

            return fileHeader;
        }

        public byte[] Serialize()
        {
            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(new BITMAPFILEHEADER()) + 3];

            Array.Copy(BitConverter.GetBytes(Type), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(Size), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(Reserved1), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(Reserved2), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(OffBits), 0, data, offset, 4); offset += 4;

            return data;
        }
    }
}
