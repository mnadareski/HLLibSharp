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
using HLLib.Packages;
using static HLLib.Utility;

namespace HLLib.Directory
{
    /// <summary>
    /// Folder contained within a package
    /// </summary>
    public sealed class DirectoryFolder : DirectoryItem
    {
        #region Fields

        /// <summary>
        /// List of directory items in this folder
        /// </summary>
        public List<DirectoryItem> DirectoryItemVector { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public DirectoryFolder(Package package)
            : base("root", Package.HL_ID_INVALID, null, package, null)
        {
            DirectoryItemVector = new List<DirectoryItem>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public DirectoryFolder(string name, uint id, byte[] data, Package package, DirectoryFolder parent)
            : base(name, id, data, package, parent)
        {
            DirectoryItemVector = new List<DirectoryItem>();
        }

        /// <summary>
        /// Destructor
        /// </summary>
        ~DirectoryFolder()
        {
            // Delete children.
            for (int i = 0; i < DirectoryItemVector.Count; i++)
            {
                DirectoryItemVector[i] = null;
            }

            DirectoryItemVector = null;
        }

        #region Descriptors

        /// <inheritdoc/>
        public override DirectoryItemType ItemType => DirectoryItemType.HL_ITEM_FOLDER;

        /// <summary>
        /// Returns the number of directory items in this folder.
        /// </summary>
        public int Count => DirectoryItemVector.Count;

        /// <summary>
        /// Determine the size of the folder in the package
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public int GetSize(bool recurse = true)
        {
            int size = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        if (recurse)
                            size += ((DirectoryFolder)item).GetSize(recurse);

                        break;
                    case DirectoryItemType.HL_ITEM_FILE:
                        size += ((DirectoryFile)item).Size;
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Determine the size of the folder in the package
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public long GetSizeEx(bool recurse = true)
        {
            long size = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        if (recurse)
                            size += ((DirectoryFolder)item).GetSizeEx(recurse);

                        break;
                    case DirectoryItemType.HL_ITEM_FILE:
                        size += ((DirectoryFile)item).Size;
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Determine the size of the folder extracted from the package
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public int GetSizeOnDisk(bool recurse = true)
        {
            int size = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        if (recurse)
                            size += ((DirectoryFolder)item).GetSizeOnDisk(recurse);

                        break;
                    case DirectoryItemType.HL_ITEM_FILE:
                        size += ((DirectoryFile)item).SizeOnDisk;
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Determine the size of the folder extracted from the package
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public long GetSizeOnDiskEx(bool recurse = true)
        {
            long size = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        if (recurse)
                            size += ((DirectoryFolder)item).GetSizeOnDiskEx(recurse);

                        break;
                    case DirectoryItemType.HL_ITEM_FILE:
                        size += ((DirectoryFile)item).SizeOnDisk;
                        break;
                }
            }

            return size;
        }

        /// <summary>
        /// Get the number of subdirectories in the folder
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public int GetFolderCount(bool recurse = true)
        {
            int count = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        count++;
                        if (recurse)
                            count += ((DirectoryFolder)item).GetFolderCount(recurse);

                        break;
                }
            }

            return count;
        }

        /// <summary>
        /// Get the number of files in the folder
        /// </summary>
        /// <param name="recurse">True to recurse through subdirectories, false otherwise</param>
        public int GetFileCount(bool recurse = true)
        {
            int count = 0;
            foreach (DirectoryItem item in DirectoryItemVector)
            {
                switch (item.ItemType)
                {
                    case DirectoryItemType.HL_ITEM_FOLDER:
                        if (recurse)
                            count += ((DirectoryFolder)item).GetFileCount(recurse);

                        break;
                    case DirectoryItemType.HL_ITEM_FILE:
                        count++;
                        break;
                }
            }

            return count;
        }

        #endregion

        #region Children

        /// <summary>
        /// Add a child folder to this folder
        /// </summary>
        /// <param name="name">Child folder name</param>
        /// <param name="id">Child ID, defaults to INVALID</param>
        /// <param name="data">Child data, defaults to null</param>
        /// <returns>Newly created folder</returns>
        public DirectoryFolder AddFolder(string name, uint id = Package.HL_ID_INVALID, byte[] data = null)
        {
            DirectoryFolder folder = new DirectoryFolder(name, id, data, Package, this);
            DirectoryItemVector.Add(folder);
            return folder;
        }

        /// <summary>
        /// Add a child file to this folder
        /// </summary>
        /// <param name="name">Child file name</param>
        /// <param name="id">Child ID, defaults to INVALID</param>
        /// <param name="data">Child data, defaults to null</param>
        /// <returns>Newly created file</returns>
        public DirectoryFile AddFile(string name, uint id = Package.HL_ID_INVALID, byte[] data = null)
        {
            DirectoryFile file = new DirectoryFile(name, id, data, Package, this);
            DirectoryItemVector.Add(file);
            return file;
        }

        /// <summary>
        /// Returns the directory item at index index.
        /// </summary>
        public DirectoryItem GetItem(int index)
        {
            if (index >= DirectoryItemVector.Count)
                return null;
            
            return DirectoryItemVector[index];
        }

        /// <summary>
        /// Returns the directory item name. If the directory item
        /// does not exist null is returned.
        /// </summary>
        public DirectoryItem GetItem(string name, FindType findType = FindType.HL_FIND_ALL)
        {
            for (int i = 0; i < DirectoryItemVector.Count; i++)
            {
                DirectoryItem item = DirectoryItemVector[i];
                if ((item.ItemType == DirectoryItemType.HL_ITEM_FILE && findType.HasFlag(FindType.HL_FIND_FILES))
                    || (item.ItemType == DirectoryItemType.HL_ITEM_FOLDER && findType.HasFlag(FindType.HL_FIND_FOLDERS)))
                {
                    if (Compare(name, item.Name, findType) == 0)
                        return item;
                }
            }

            return null;
        }

        /// <summary>
        /// Returns the directory item path.  If the directory item
        /// does not exist null is returned.  Transverses sub-folders too.
        /// </summary>
        public DirectoryItem GetRelativeItem(string path, FindType findType = FindType.HL_FIND_ALL)
        {
            DirectoryFolder folder = this;
            int index = 0;

            string[] token = path.Split('\\', '/');
            if (token != null && Compare(folder.Name, token[index], findType) == 0)
                index++; // lpToken = strtok(0, "\\/");

            while (token != null)
            {
                if (token[index] == "\0" || token[index] == ".")
                {
                    index++; // lpToken = strtok(0, "\\/");
                }
                else if (token[index] == "..")
                {
                    if (folder.Parent != null)
                        folder = folder.Parent;
                    else
                        return null;

                    index++;
                }
                else
                {
                    int next = index + 1;

                    DirectoryItem item = null;

                    for (int i = 0; i < folder.Count; i++)
                    {
                        DirectoryItem tempItem = folder.GetItem(i);
                        if (next >= token.Length
                            && tempItem.ItemType == DirectoryItemType.HL_ITEM_FILE
                            && findType.HasFlag(FindType.HL_FIND_FILES)
                            && Compare(token[index], tempItem.Name, findType) == 0)
                        {
                            item = tempItem;
                            break;
                        }
                        else if (tempItem.ItemType == DirectoryItemType.HL_ITEM_FOLDER
                            && Compare(token[index], tempItem.Name, findType) == 0)
                        {
                            item = tempItem;
                            break;
                        }
                    }

                    if (item == null)
                    {
                        return null;
                    }

                    if (item.ItemType == DirectoryItemType.HL_ITEM_FOLDER)
                    {
                        folder = (DirectoryFolder)item;
                    }
                    else
                    {
                        return item;
                    }

                    index = next;
                }
            }

            if (findType.HasFlag(FindType.HL_FIND_FOLDERS))
                return folder;
            else
                return null;
        }

        /// <summary>
        /// Sort the internal set of items
        /// </summary>
        /// <param name="sortField">Field to sort items on</param>
        /// <param name="sortOrder">Order to sort items into</param>
        /// <param name="recurse">True to recurse into subfolders, false otherwise</param>
        public void Sort(SortField sortField = SortField.HL_FIELD_NAME, SortOrder sortOrder = SortOrder.HL_ORDER_ASCENDING, bool recurse = true)
        {
            DirectoryItemVector.Sort(new CompareDirectoryItems(sortField, sortOrder));
            if (recurse)
            {
                for (int i = 0; i < DirectoryItemVector.Count; i++)
                {
                    DirectoryItem item = DirectoryItemVector[i];
                    if (item.ItemType == DirectoryItemType.HL_ITEM_FOLDER)
                        ((DirectoryFolder)item).Sort(sortField, sortOrder, recurse);
                }
            }
        }

        /// <summary>
        /// Find the first item with a given search term
        /// </summary>
        /// <param name="search">Search term to use</param>
        /// <param name="findType">Finding type to use</param>
        /// <returns>First found directory item, null otherwise</returns>
        public DirectoryItem FindFirst(string search, FindType findType = FindType.HL_FIND_ALL) => FindNext(this, null, search, findType);

        /// <summary>
        /// Find the next item with a given search term
        /// </summary>
        /// <param name="item">DirectoryItem to search within</param>
        /// <param name="search">Search term to use</param>
        /// <param name="findType">Finding type to use</param>
        /// <returns>First found directory item, null otherwise</returns>
        public DirectoryItem FindNext(DirectoryItem item, string search, FindType findType = FindType.HL_FIND_ALL)
        {
            if (item == null)
                return null;
            
            if (item.ItemType == DirectoryItemType.HL_ITEM_FOLDER && !findType.HasFlag(FindType.HL_FIND_NO_RECURSE))
                return FindNext((DirectoryFolder)item, null, search, findType);
            else
                return FindNext(item.Parent, item, search, findType);
        }

        /// <summary>
        /// Find the next item with a given search term
        /// </summary>
        /// <param name="folder">Folder to search within</param>
        /// <param name="relative">Original item in the relative path</param>
        /// <param name="search">Search term to use</param>
        /// <param name="findType">Finding type to use</param>
        /// <returns>First found directory item, null otherwise</returns>
        private DirectoryItem FindNext(DirectoryFolder folder, DirectoryItem relative, string search, FindType findType = FindType.HL_FIND_ALL)
        {
            int first = 0;

            if (relative != null)
            {
                for (int i = 0; i < folder.Count; i++)
                {
                    if (folder.GetItem(i) == relative)
                    {
                        first = i + 1;
                        break;
                    }
                }
            }

            for (int i = first; i < folder.Count; i++)
            {
                DirectoryItem test = folder.GetItem(i);
                if ((test.ItemType == DirectoryItemType.HL_ITEM_FILE && findType.HasFlag(FindType.HL_FIND_FILES))
                    || (test.ItemType == DirectoryItemType.HL_ITEM_FOLDER && findType.HasFlag(FindType.HL_FIND_FOLDERS)))
                {
                    if (Match(test.Name, search, findType))
                        return test;
                }

                if (test.ItemType == DirectoryItemType.HL_ITEM_FOLDER && !findType.HasFlag(FindType.HL_FIND_NO_RECURSE))
                {
                    test = FindNext((DirectoryFolder)test, null, search, findType);
                    if (test != null)
                        return test;
                }
            }

            if (this == folder || relative == null || folder.Parent == null)
                return null;

            return FindNext(folder.Parent, folder, search, findType);
        }

        /// <summary>
        /// Match a string to a seach string.  Search string can contain wild cards like * (to match
        /// a substring) and ? (to match a letter).
        /// </summary>
        private bool Match(string str, string search, FindType findType)
        {
            int searchIndex = 0;
            int stringIndex = 0;

            if (findType.HasFlag(FindType.HL_FIND_MODE_STRING))
            {
                return Compare(str, search, findType) == 0;
            }
            else if (findType.HasFlag(FindType.HL_FIND_MODE_SUBSTRING))
            {
                int stringLength = str.Length;
                int searchLength = search.Length;
                int tests = stringLength - searchLength;

                if (findType.HasFlag(FindType.HL_FIND_CASE_SENSITIVE))
                {
                    for (int i = 0; i < tests; i++)
                    {
                        if (string.Equals(str.Substring(i, searchLength), search, StringComparison.Ordinal))
                            return true;
                    }
                }
                else
                {
                    for (int i = 0; i < tests; i++)
                    {
                        if (string.Equals(str.Substring(i, searchLength), search, StringComparison.OrdinalIgnoreCase))
                            return true;
                    }
                }

                return false;
            }
            else // if (eFind.HasFlag(HLFindType.HL_FIND_MODE_WILDCARD))
            {
                while (search != null && searchIndex < search.Length)
                {
                    if (search[searchIndex] == '*')
                    {
                        if (search[searchIndex + 1] == '*')
                        {
                            searchIndex++;
                            continue;
                        }
                        else if (search[searchIndex + 1] == '\0')
                        {
                            return true;
                        }
                        else
                        {
                            searchIndex++;
                            while (str != null && stringIndex < str.Length)
                            {
                                if (Match(str.Substring(stringIndex), search, findType))
                                    return true;

                                stringIndex++;
                            }

                            return false;
                        }
                    }
                    else if (search[searchIndex] == '?')
                    {
                        if (str[stringIndex] == '\0')
                            return false;

                        searchIndex++;
                        stringIndex++;
                    }
                    else
                    {
                        if (str[stringIndex] == '\0')
                        {
                            return false;
                        }
                        else
                        {
                            char iA = search[searchIndex];
                            char iB = str[stringIndex];

                            if (findType.HasFlag(FindType.HL_FIND_CASE_SENSITIVE))
                            {
                                if (iA >= 'a' && iA <= 'z')
                                    iA -= (char)('a' - 'A');
                                if (iB >= 'a' && iB <= 'z')
                                    iB -= (char)('a' - 'A');
                            }

                            if (iA != iB)
                                return false;
                        }
                    }

                    searchIndex++;
                    stringIndex++;
                }
            }

            return str[stringIndex] == '\0';
        }

        #endregion

        #region Extraction

        /// <inheritdoc/>
        public override bool Extract(string path, bool readEncrypted = true, bool overwrite = true)
        {
            string name = RemoveIllegalCharacters(Name);

            string folderName;
            if (path == null || path == "\0")
                folderName = name;
            else
                folderName = System.IO.Path.Combine(path, name);

            folderName = FixupIllegalCharacters(folderName);

            bool result;
            if (!CreateFolder(folderName))
            {
                Console.WriteLine("CreateDirectory() failed.");
                result = false;
            }
            else
            {
                result = true;
                for (int i = 0; i < DirectoryItemVector.Count; i++)
                {
                    result &= DirectoryItemVector[i].Extract(folderName, readEncrypted, overwrite);
                }
            }

            return result;
        }

        #endregion

        #region Comparison Helpers

        /// <summary>
        /// Compare two strings with a particular finding type
        /// </summary>
        private int Compare(string string0, string string1, FindType findType)
        {
            if (findType.HasFlag(FindType.HL_FIND_CASE_SENSITIVE))
                return string.Compare(string0, string1, StringComparison.Ordinal);
            else
                
                return string.Compare(string0, string1, StringComparison.OrdinalIgnoreCase);
        }

        private class CompareDirectoryItems : IComparer<DirectoryItem>
        {
            private SortField sortField;
            private SortOrder sortOrder;

            public CompareDirectoryItems(SortField sortField, SortOrder sortOrder)
            {
                this.sortField = sortField;
                this.sortOrder = sortOrder;
            }

            public int Compare(DirectoryItem item0, DirectoryItem item1)
            {
                DirectoryItemType type0 = item0.ItemType;
                DirectoryItemType type1 = item1.ItemType;

                if (type0 == DirectoryItemType.HL_ITEM_FOLDER && type1 == DirectoryItemType.HL_ITEM_FILE)
                    return 1;
                else if (type0 == DirectoryItemType.HL_ITEM_FILE && type1 == DirectoryItemType.HL_ITEM_FOLDER)
                    return -1;

                int result;
                switch (sortField)
                {
                    case SortField.HL_FIELD_SIZE:
                        {
                            int uiSize0 = type0 == DirectoryItemType.HL_ITEM_FILE ? ((DirectoryFile)item0).Size : ((DirectoryFolder)item0).Count;
                            int uiSize1 = type1 == DirectoryItemType.HL_ITEM_FILE ? ((DirectoryFile)item1).Size : ((DirectoryFolder)item1).Count;

                            result = uiSize0 - uiSize1;

                            if (result != 0)
                                break;

                            // Originally a fall-through to the next case
                            result = string.Compare(item0.Name, item1.Name);
                            break;
                        }
                    case SortField.HL_FIELD_NAME:
                    default:
                        {
                            result = string.Compare(item0.Name, item1.Name, StringComparison.OrdinalIgnoreCase);
                            break;
                        }
                }

                if (sortOrder == SortOrder.HL_ORDER_DESCENDING)
                    result *= -1;

                return result;
            }
        }

        #endregion
    }
}