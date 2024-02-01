//
// GRILO  Copyright (C) 2022  Aptivi
//
// This file is part of GRILO
//
// GRILO is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// GRILO is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY, without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/>.
//

using SpecProbe.Platform;
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
        public static string GRILOPath
        {
            get
            {
                if (PlatformHelper.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO");
            }
        }

        /// <summary>
        /// Path to GRILO configuration file
        /// </summary>
        public static string GRILOConfigPath
        {
            get
            {
                if (PlatformHelper.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/BootloaderConfig.json");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/BootloaderConfig.json");
            }
        }

        /// <summary>
        /// Path to GRILO boot styles folder
        /// </summary>
        public static string GRILOStylesPath
        {
            get
            {
                if (PlatformHelper.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/Styles");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/Styles");
            }
        }

        /// <summary>
        /// Path to GRILO bootable apps list folder
        /// </summary>
        public static string GRILOBootablesPath
        {
            get
            {
#if NETCOREAPP
                if (PlatformHelper.IsOnWindows())
                    return Path.Combine(Environment.GetEnvironmentVariable("LOCALAPPDATA"), "GRILO/Bootables");
                else
                    return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".config/GRILO/Bootables");
#else
                if (PlatformHelper.IsOnWindows())
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
                if (PlatformHelper.IsOnWindows())
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
