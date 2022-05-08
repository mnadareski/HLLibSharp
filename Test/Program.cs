using System;
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
                    if (packageType == PackageType.HL_PACKAGE_NONE)
                    {
                        Console.WriteLine("Could not determine package type!");
                        Console.WriteLine();
                        continue;
                    }

                    // Create a new package from the file
                    Package package = Package.CreatePackage(packageType);
                    FileModeFlags mode = FileModeFlags.HL_MODE_READ | FileModeFlags.HL_MODE_WRITE | FileModeFlags.HL_MODE_NO_FILEMAPPING;
                    bool opened = package.Open(arg, mode, overwriteFiles: true);
                    if (!opened)
                    {
                        Console.WriteLine("Package could not be opened!");
                        Console.WriteLine();
                        continue;
                    }

                    // Print all package info
                    Package.PrintPackageInfo(package);

                    // Create the root directory
                    DirectoryFolder rootDirectory = package.GetRoot();

                    // Extract all files
                    string outputPath = Path.Combine(Path.GetDirectoryName(Path.GetFullPath(arg)), Path.GetFileNameWithoutExtension(arg));
                    rootDirectory.Extract(outputPath, readEncrypted: true, overwrite: true);

                    // Close the package explicitly
                    package.Close();
                    Console.WriteLine();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    Console.WriteLine();
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
                    return Package.GetPackageType(buffer, Path.GetExtension(file));
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