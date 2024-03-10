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

using GRILO.Boot;
using GRILO.Bootloader.Boot.Apps.Marshaling;
using GRILO.Bootloader.Boot.Diagnostics;

namespace GRILO.Bootloader.Boot.Apps
{
    /// <summary>
    /// Bootable application information
    /// </summary>
    public class BootAppInfo
    {
#if NET6_0_OR_GREATER
        internal BootLoadContext context;
#endif

        /// <summary>
        /// Bootable file path
        /// </summary>
        public string BootFile { get; }
        /// <summary>
        /// Boot app title (overrides one found in IBootable)
        /// </summary>
        public string OverriddenTitle { get; }
        /// <summary>
        /// Arguments to inject to the boot file
        /// </summary>
        public string[] Arguments { get; }
        /// <summary>
        /// Gets the bootable
        /// </summary>
        public IBootable Bootable { get; }

        internal BootAppInfo(string bootFile, string overriddenTitle, string[] arguments, IBootable bootable)
        {
            BootFile = bootFile;
            OverriddenTitle = overriddenTitle;
            Arguments = arguments;
            Bootable = bootable;

            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot file: {0}", BootFile);
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Overridden title: {0}", OverriddenTitle);
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Arguments: [ {0} ]", string.Join(", ", Arguments));
        }
    }
}
