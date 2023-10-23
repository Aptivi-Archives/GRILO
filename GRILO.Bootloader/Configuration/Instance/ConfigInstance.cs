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

using Newtonsoft.Json;
using System;

namespace GRILO.Bootloader.Configuration.Instance
{
    /// <summary>
    /// Configuration instance
    /// </summary>
    public class ConfigInstance
    {
        /// <summary>
        /// The boot style name
        /// </summary>
        [JsonProperty("Boot style", Required = Required.Always)]
        public string BootStyleName { get; set; } = "Default";

        /// <summary>
        /// Whether to create a file containing diagnostic messages
        /// </summary>
        [JsonProperty("Diagnostic messages", Required = Required.Always)]
        public bool DiagnosticMessages { get; set; }

        /// <summary>
        /// Whether to also print diagnostic messages to the console
        /// </summary>
        [JsonProperty("Print diagnostic messages to console", Required = Required.Always)]
        public bool PrintDiagnosticMessages { get; set; }

        /// <summary>
        /// Additional bootable folders to scan for bootable applications
        /// </summary>
        [JsonProperty("Additional bootable folders", Required = Required.Always)]
        public string[] AdditionalScanFolders { get; set; } = Array.Empty<string>();

        /// <summary>
        /// Timeout to boot to the default selection
        /// </summary>
        [JsonProperty("Timeout to boot to selection", Required = Required.Always)]
        public int BootSelectTimeoutSeconds { get; set; } = 10;

        /// <summary>
        /// The default boot entry selection. This number is zero-based, so the first element is index 0, and so on.
        /// </summary>
        [JsonProperty("Default boot entry selection", Required = Required.Always)]
        public int BootSelect { get; set; } = 0;
    }
}
