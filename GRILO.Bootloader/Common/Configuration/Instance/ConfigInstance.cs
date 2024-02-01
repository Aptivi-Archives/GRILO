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

using Newtonsoft.Json;
using System;

namespace GRILO.Bootloader.Common.Configuration.Instance
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
