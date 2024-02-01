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

using GRILO.Bootloader.Common.Configuration.Instance;
using Newtonsoft.Json;
using System.IO;

namespace GRILO.Bootloader.Common.Configuration
{
    /// <summary>
    /// Configuration module
    /// </summary>
    public static class Config
    {
        internal static ConfigInstance instance = new();

        /// <summary>
        /// Configuration instance
        /// </summary>
        public static ConfigInstance Instance =>
            instance;

        /// <summary>
        /// Reads configuration
        /// </summary>
        public static void ReadConfig()
        {
            // If we don't have the config yet, save the file
            if (!File.Exists(ConfigPaths.GRILOConfigPath))
                SaveConfigFile();

            // Now, open the file and save the values from the config to the program
            try
            {
                string fileContents = File.ReadAllText(ConfigPaths.GRILOConfigPath);
                instance = (ConfigInstance)JsonConvert.DeserializeObject(fileContents, typeof(ConfigInstance));
            }
            catch
            {
                // Try to fix configuration
                SaveConfigFile();
            }
        }

        /// <summary>
        /// Saves configuration file
        /// </summary>
        public static void SaveConfigFile()
        {
            // Save the configuration to the GRILO config path
            string serializedConfig = JsonConvert.SerializeObject(Instance, Formatting.Indented);
            File.WriteAllText(ConfigPaths.GRILOConfigPath, serializedConfig);
        }
    }
}
