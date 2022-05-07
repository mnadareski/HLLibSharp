﻿using System;
using System.IO;
using HLLib.Directory;
using HLLib.Packages;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (string arg in args)
            {
                try
                {
                    Console.WriteLine($"Path: {arg}");

                    // Derive the type from the file
                    PackageType packageType = DeriveType(arg);
                    Console.WriteLine($"Package Type: {packageType}");
                    if (packageType == PackageType.HL_PACKAGE_NONE)
                        continue;

                    // Create a new package from the file
                    Package package = Package.CreatePackage(packageType);
                    FileModeFlags mode = FileModeFlags.HL_MODE_READ | FileModeFlags.HL_MODE_WRITE | FileModeFlags.HL_MODE_NO_FILEMAPPING;
                    bool opened = package.Open(arg, mode, overwriteFiles: true);
                    if (!opened)
                    {
                        Console.WriteLine("Package could not be opened!");
                        continue;
                    }

                    Console.WriteLine("Package opened successfully!");

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        /// <summary>
        /// Derive the type for a file safely
        /// </summary>
        /// <param name="file">File path to check</param>
        /// <returns>Internal package type of the file</returns>
        private static PackageType DeriveType(string file)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File {file} does not exist, skipping...");
                return PackageType.HL_PACKAGE_NONE;
            }

            try
            {
                using (FileStream fs = File.Open(file, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    byte[] buffer = new byte[16];
                    fs.Read(buffer, 0, 16);
                    return Package.GetPackageTypeFromHeader(buffer);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return PackageType.HL_PACKAGE_NONE;
            }
        }
    }
}