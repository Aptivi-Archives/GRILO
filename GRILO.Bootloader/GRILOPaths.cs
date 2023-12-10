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

using System;
using System.IO;

namespace GRILO.Bootloader
{
    /// <summary>
    /// GRILO paths module
    /// </summary>
    public static class GRILOPaths
    {
        /// <summary>
        /// Path to GRILO folder
        /// </summary>
        public static string GRILOPath {
            get
            {
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO");
            }
        }

        /// <summary>
        /// Path to GRILO configuration file
        /// </summary>
        public static string GRILOConfigPath {
            get
            {
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/BootloaderConfig.json");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/BootloaderConfig.json");
            }
        }

        /// <summary>
        /// Path to GRILO boot styles folder
        /// </summary>
        public static string GRILOStylesPath {
            get
            {
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/Styles");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/Styles");
            }
        }

        /// <summary>
        /// Path to GRILO bootable apps list folder
        /// </summary>
        public static string GRILOBootablesPath {
            get
            {
#if NETCOREAPP
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/Bootables");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/Bootables");
#else
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/Bootables_DotNetFx");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/Bootables_DotNetFx");
#endif
            }
        }

        /// <summary>
        /// Path to GRILO debug file
        /// </summary>
        public static string GRILODebugPath
        {
            get
            {
                if (GRILOPlatform.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/BootloaderDebug.log");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/BootloaderDebug.log");
            }
        }

        /// <summary>
        /// Makes directories for GRILO
        /// </summary>
        public static void MakePaths()
        {
            if (!Directory.Exists(GRILOPath))
                Directory.CreateDirectory(GRILOPath);
            if (!Directory.Exists(GRILOBootablesPath))
                Directory.CreateDirectory(GRILOBootablesPath);
            if (!Directory.Exists(GRILOStylesPath))
                Directory.CreateDirectory(GRILOStylesPath);
        }
    }
}
