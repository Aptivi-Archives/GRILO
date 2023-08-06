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

using GRILO.Bootloader.Configuration.Instance;
using Newtonsoft.Json;
using System.IO;

namespace GRILO.Bootloader.Configuration
{
    /// <summary>
    /// Configuration module
    /// </summary>
    public static class Config
    {
        internal static ConfigInstance instance = new();

        public static ConfigInstance Instance =>
            instance;

        /// <summary>
        /// Reads configuration
        /// </summary>
        public static void ReadConfig()
        {
            // If we don't have the config yet, save the file
            if (!File.Exists(GRILOPaths.GRILOConfigPath))
                SaveConfigFile();

            // Now, open the file and save the values from the config to the program
            try
            {
                string fileContents = File.ReadAllText(GRILOPaths.GRILOConfigPath);
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
            File.WriteAllText(GRILOPaths.GRILOConfigPath, serializedConfig);
        }
    }
}
