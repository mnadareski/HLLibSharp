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
    public class BITMAPINFOHEADER
    {
        public uint Size { get; set; }

        public int Width { get; set; }

        public int Height { get; set; }

        public ushort Planes { get; set; }

        public ushort BitCount { get; set; }

        public uint Compression { get; set; }

        public uint SizeImage { get; set; }

        public int XPelsPerMeter { get; set; }

        public int YPelsPerMeter { get; set; }

        public uint ClrUsed { get; set; }

        public uint ClrImportant { get; set; }

        public static BITMAPINFOHEADER Create(byte[] data, ref int offset)
        {
            BITMAPINFOHEADER infoHeader = new BITMAPINFOHEADER();

            // Check to see if the data is valid
            if (data == null || data.Length < Marshal.SizeOf(infoHeader))
                return null;

            infoHeader.Size = BitConverter.ToUInt32(data, offset); offset += 4;
            infoHeader.Width = BitConverter.ToInt32(data, offset); offset += 4;
            infoHeader.Height = BitConverter.ToInt32(data, offset); offset += 4;
            infoHeader.Planes = BitConverter.ToUInt16(data, offset); offset += 2;
            infoHeader.BitCount = BitConverter.ToUInt16(data, offset); offset += 2;
            infoHeader.Compression = BitConverter.ToUInt32(data, offset); offset += 4;
            infoHeader.SizeImage = BitConverter.ToUInt32(data, offset); offset += 4;
            infoHeader.XPelsPerMeter = BitConverter.ToInt32(data, offset); offset += 4;
            infoHeader.YPelsPerMeter = BitConverter.ToInt32(data, offset); offset += 4;
            infoHeader.ClrUsed = BitConverter.ToUInt32(data, offset); offset += 4;
            infoHeader.ClrImportant = BitConverter.ToUInt32(data, offset); offset += 4;

            return infoHeader;
        }

        public byte[] Serialize()
        {
            int offset = 0;
            byte[] data = new byte[Marshal.SizeOf(new BITMAPFILEHEADER()) + 3];

            Array.Copy(BitConverter.GetBytes(Size), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(Width), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(Height), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(Planes), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(BitCount), 0, data, offset, 2); offset += 2;
            Array.Copy(BitConverter.GetBytes(Compression), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(SizeImage), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(XPelsPerMeter), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(YPelsPerMeter), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(ClrUsed), 0, data, offset, 4); offset += 4;
            Array.Copy(BitConverter.GetBytes(ClrImportant), 0, data, offset, 4); offset += 4;

            return data;
        }
    }
}
