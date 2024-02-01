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

namespace GRILO.Bootloader.Common.Configuration
{
    /// <summary>
    /// GRILO paths module
    /// </summary>
    public static class ConfigPaths
    {
        /// <summary>
        /// Path to GRILO folder
        /// </summary>
        public static string GRILOPath
        {
            get
            {
                string config =
                    PlatformHelper.IsOnWindows() ?
                    Environment.GetEnvironmentVariable("LOCALAPPDATA") :
                    Environment.GetEnvironmentVariable("HOME") + "/.config";
                return Path.Combine(config, "GRILO");
            }
        }

        /// <summary>
        /// Path to GRILO configuration file
        /// </summary>
        public static string GRILOConfigPath =>
            FormPath("BootloaderConfig.json");

        /// <summary>
        /// Path to GRILO boot styles folder
        /// </summary>
        public static string GRILOStylesPath =>
            FormPath("Styles");

        /// <summary>
        /// Path to GRILO bootable apps list folder
        /// </summary>
        public static string GRILOBootablesPath =>
#if NETCOREAPP
            FormPath("Bootables");
#else
            FormPath("Bootables_DotNetFx");
#endif

        /// <summary>
        /// Path to GRILO debug file
        /// </summary>
        public static string GRILODebugPath =>
            FormPath("BootloaderDebug.log");

        internal static void MakePaths()
        {
            if (!Directory.Exists(GRILOPath))
                Directory.CreateDirectory(GRILOPath);
            if (!Directory.Exists(GRILOBootablesPath))
                Directory.CreateDirectory(GRILOBootablesPath);
            if (!Directory.Exists(GRILOStylesPath))
                Directory.CreateDirectory(GRILOStylesPath);
        }

        private static string FormPath(string fileName) =>
            Path.Combine(GRILOPath, fileName);
    }
}
