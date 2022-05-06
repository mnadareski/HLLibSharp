/*
 * HLLib
 * Copyright (C) 2006-2010 Ryan Gregg

 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your Option) any later
 * version.
 */

using System.IO;

namespace HLLib
{
    public static class Utility
    {
        public static bool GetFileSize(string path, out long fileSize)
        {
            try
            {
                FileInfo info = new FileInfo(path);
                fileSize = info.Length;
                return true;
            }
            catch
            {
                fileSize = default;
                return false;
            }
        }

        public static bool CreateFolder(string path) => System.IO.Directory.CreateDirectory(path) != null;

        public static string FixupIllegalCharacters(string name) => name.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);

        public static string RemoveIllegalCharacters(string name)
        {
            foreach (string c in new string[] { "/", "\\", "?", "<", ">", ":", "*", "|", "\"", "\0" })
            {
                name = name.Replace(c, string.Empty);
            }

            return name;
        }
    }
}