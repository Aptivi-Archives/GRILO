/*
 * MIT License
 * 
 * Copyright (c) 2022 Aptivi
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using GRILO.Bootloader.BootApps;
using GRILO.Bootloader.BootStyle;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;

namespace GRILO.Bootloader.Configuration
{
    /// <summary>
    /// Configuration module
    /// </summary>
    public static class Config
    {
        public static JObject GeneratePristineConfig(bool @default = false)
        {
            JObject pristineConfig = new();

            // Populate these configuration entries
            JArray bootFolders = new()
            {
                @default ? Array.Empty<string>() : BootManager.additionalScanFolders
            };
            JObject entries = new()
            {
                { "Boot style",                             @default ? "Default" : BootStyleManager.bootStyleStr },
                { "Diagnostic messages",                    !@default && GRILO.diagMessages },
                { "Print diagnostic messages to console",   !@default && GRILO.printDiagMessages },
                { "Additional bootable folders",            bootFolders }
            };

            // Add the entries to the master object
            pristineConfig.Add("GRILO", entries);
            return pristineConfig;
        }

        /// <summary>
        /// Reads configuration
        /// </summary>
        public static void ReadConfig()
        {
            // If we don't have the config yet, save the file
            if (!File.Exists(GRILOPaths.GRILOConfigPath))
                SaveConfigFile();

            // Now, open the file and save the values from the config to the program
            JToken config = JObject.Parse(File.ReadAllText(GRILOPaths.GRILOConfigPath)).SelectToken("GRILO");
            BootStyleManager.bootStyleStr =     config["Boot style"].ToString();
            GRILO.diagMessages =                (bool)config["Diagnostic messages"].ToObject(typeof(bool));
            GRILO.printDiagMessages =           (bool)config["Print diagnostic messages to console"].ToObject(typeof(bool));
            BootManager.additionalScanFolders = config["Additional bootable folders"].ToObject<string[]>();
        }

        /// <summary>
        /// Saves configuration file
        /// </summary>
        public static void SaveConfigFile()
        {
            // Get the pristine config
            JObject pristineConfig = GeneratePristineConfig();

            // Save the configuration to the GRILO config path
            string serializedConfig = JsonConvert.SerializeObject(pristineConfig, Formatting.Indented);
            File.WriteAllText(GRILOPaths.GRILOConfigPath, serializedConfig);
        }
    }
}
