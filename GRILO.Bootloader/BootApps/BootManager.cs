//
// MIT License
//
// Copyright (c) 2022 Aptivi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using GRILO.Boot;
using GRILO.Bootloader.BootApps.CommonApps;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GRILO.Bootloader.Diagnostics;
using GRILO.Bootloader.Configuration;
using Terminaux.Writer.ConsoleWriters;

#if NET6_0_OR_GREATER
using System.Runtime.Loader;
#endif

namespace GRILO.Bootloader.BootApps
{
    /// <summary>
    /// Bootable application manager
    /// </summary>
    public static class BootManager
    {
        private static readonly Dictionary<string, BootAppInfo> bootApps = new()
        {
            { "Shutdown the system", new BootAppInfo("", "", Array.Empty<string>(), new Shutdown()) }
        };

        /// <summary>
        /// Adds all bootable applications to the bootloader
        /// </summary>
        public static void PopulateBootApps()
        {
            // Custom boot apps usually have the .DLL extension for .NET 6.0 and the .EXE extension for .NET Framework
            var bootDirs = Directory.EnumerateDirectories(GRILOPaths.GRILOBootablesPath).ToList();
            bootDirs.AddRange(Config.Instance.AdditionalScanFolders);
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot directories to scan: {0}", bootDirs.Count);

            foreach (var bootDir in bootDirs)
            {
                // Get the boot ID
                string bootId = Path.GetFileName(bootDir);

                // Create boot context
#if NET6_0_OR_GREATER
                var bootContext = new BootLoadContext
                {
                    bootPath = bootDir
                };
#else
                AppDomain ad2 = AppDomain.CreateDomain($"Boot context for {bootId}", null, bootId, bootId, false);
                var assemblyLoader = (BootLoader)ad2.CreateInstanceFromAndUnwrap(typeof(BootLoader).Assembly.CodeBase, typeof(BootLoader).FullName);
#endif

                // Now, check the metadata to get a bootable file path
                string metadataFile = Path.Combine(GRILOPaths.GRILOBootablesPath, bootId, "BootMetadata.json");
                List<string> paths = new();
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Trying to find metadata file {0} to get bootable file path...", metadataFile);

                // Now, check for metadata existence
                if (File.Exists(metadataFile))
                {
                    // Metadata file exists! Now, parse it.
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Metadata found! Parsing JSON...");
                    var metadataToken = JArray.Parse(File.ReadAllText(metadataFile));

                    // Enumerate through metadata array
                    foreach (var metadata in metadataToken)
                    {
                        string path = metadata["BootFilePath"]?.ToString() is not null ? Path.Combine(GRILOPaths.GRILOBootablesPath, bootId, metadata["BootFilePath"]?.ToString()) : "";
                        if (!paths.Contains(path) && !string.IsNullOrEmpty(path))
                            paths.Add(path);
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot path {0}.", path);
                    }
                }

                // Process boot files
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot ID: {0}", bootId);
                try
                {
                    // Using the boot ID, check for executable files
#if NETCOREAPP
                    var bootFiles = paths.Count > 0 ? paths : Directory.EnumerateFiles(bootDir, "*.dll");
#else
                    var bootFiles = paths.Count > 0 ? paths : Directory.EnumerateFiles(bootDir, "*.exe");
#endif
                    foreach (var bootFile in bootFiles)
                    {
                        // Exclude GRILO.Boot since it's just a minimal library that contains interface IBootable
                        if (Path.GetFileName(bootFile) == "GRILO.Boot.dll")
                            continue;

                        try
                        {
                            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Processing boot file {0}...", bootFile);

                            // Load the boot assembly file and check to see if it implements IBootable.
                            BootAppInfo bootApp;
                            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Loading assembly...");
#if NET6_0_OR_GREATER
                            bootContext.resolver = new AssemblyDependencyResolver(bootFile);
                            var asm = bootContext.LoadFromAssemblyPath(bootFile);
                            IBootable bootable = null;

                            // Get the implemented types of the assembly so that we can check every type found in this assembly for implementation of the
                            // BaseBootStyle class.
                            foreach (Type t in asm.GetTypes())
                            {
                                if (t.GetInterface(typeof(IBootable).Name) != null)
                                {
                                    // We found a boot app! Add it to the custom boot apps dictionary.
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Found a boot app!");
                                    bootable = (IBootable)asm.CreateInstance(t.FullName);
                                    break;
                                }
                            }

                            // Check to see if we successfully found the IBootable instance
                            if (bootable != null)
                            {
                                // We found it! Now, populate info from the metadata file
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Trying to find metadata file {0}...", metadataFile);

                                // Let's put some variables here
                                string bootOverrideTitle = "";
                                string[] bootArgs = Array.Empty<string>();

                                // Now, check for metadata existence
                                if (File.Exists(metadataFile))
                                {
                                    // Metadata file exists! Now, parse it.
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Metadata found! Parsing JSON...");
                                    var metadataToken = JArray.Parse(File.ReadAllText(metadataFile));

                                    // Enumerate through metadata array
                                    foreach (var metadata in metadataToken)
                                    {
                                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Filling entries...");

                                        // Fill them
                                        bootOverrideTitle = metadata["OverrideTitle"]?.ToString() ?? bootable.Title ?? asm.GetName().Name;
                                        bootArgs = metadata["Arguments"]?.ToObject<string[]>() ?? Array.Empty<string>();
                                        bootApp = new(bootFile, bootOverrideTitle, bootArgs, bootable);
                                        bootApps.Add(bootOverrideTitle, bootApp);
                                    }
                                }
                                else
                                {
                                    // Skip boot ID as metadata is not found.
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Skipping boot ID {0} because {1} is not found...", bootFile, metadataFile);
                                    break;
                                }
                            }
                            else
                            {
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Skipping boot ID {0} because it is an invalid boot file...", bootFile);
                                continue;
                            }
#else
                            assemblyLoader.lookupPath = bootDir;

                            // Now, check the metadata
                            if (assemblyLoader.VerifyBoot(File.ReadAllBytes(bootFile)))
                            {
                                // We found it! Now, populate info from the metadata file
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Trying to find metadata file {0}...", metadataFile);

                                // Let's put some variables here
                                string bootOverrideTitle = "";
                                string[] bootArgs = Array.Empty<string>();
                                
                                // Now, check for metadata existence
                                if (File.Exists(metadataFile))
                                {
                                    // Metadata file exists! Now, parse it.
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Metadata found! Parsing JSON...");
                                    var metadataToken = JArray.Parse(File.ReadAllText(metadataFile));
                                
                                    // Enumerate through metadata array
                                    foreach (var metadata in metadataToken)
                                    {
                                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Filling entries...");
                                        
                                        // Fill them
                                        var proxy = new BootProxy();
                                        proxy.loader = assemblyLoader;
                                        bootOverrideTitle = metadata["OverrideTitle"]?.ToString() ?? Path.GetFileNameWithoutExtension(bootFile);
                                        bootArgs = metadata["Arguments"]?.ToObject<string[]>() ?? Array.Empty<string>();
                                        bootApp = new(bootFile, bootOverrideTitle, bootArgs, proxy);
                                        bootApps.Add(bootOverrideTitle, bootApp);
                                    }
                                 }
                                 else
                                 {
                                    // Skip boot ID as metadata is not found.
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Skipping boot ID {0} because {1} is not found...", bootFile, metadataFile);
                                    break;
                                 }
                            }
#endif
                        }
                        catch (Exception ex)
                        {
                            // Either the boot file is invalid or can't be loaded.
                            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Can't load boot app. {0}: {1}", ex.GetType().Name, ex.Message);
                            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Stack trace:\n{0}", ex.StackTrace);
                            TextWriterColor.Write($"Failed to parse boot file {bootFile}: {ex.Message}");
                        }
                    }
                    
                }
                catch (Exception ex)
                {
                    // Boot ID invalid
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Unknown error when loading boot ID {0}: {1}", bootId, ex.Message);
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Stack trace:\n{0}", ex.StackTrace);
                    TextWriterColor.Write($"Unknown error when parsing boot ID {bootId}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Gets boot applications
        /// </summary>
        public static Dictionary<string, BootAppInfo> GetBootApps() =>
            new(bootApps);

        /// <summary>
        /// Gets boot application by name
        /// </summary>
        public static BootAppInfo GetBootApp(string name)
        {
            bootApps.TryGetValue(name, out var info);
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Got boot app {0}!", name);
            return info;
        }

        /// <summary>
        /// Gets boot application name by index
        /// </summary>
        public static string GetBootAppNameByIndex(int index)
        {
            for (int i = 0; i < bootApps.Count; i++)
            {
                if (i == index)
                {
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Got boot app at index {0}!", index);
                    return bootApps.ElementAt(index).Key;
                }
            }

            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "No boot app at index {0}. Returning empty string...", index);
            return "";
        }
    }
}
