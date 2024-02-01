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

namespace GRILO.Bootloader.Boot.Diagnostics
{
    /// <summary>
    /// Diagnostics level enumeration
    /// </summary>
    public enum DiagnosticsLevel
    {
        /// <summary>
        /// Informational diagnostic messages
        /// </summary>
        Info,
        /// <summary>
        /// Warning messages
        /// </summary>
        Warning,
        /// <summary>
        /// Error messages, usually indicating that something is wrong
        /// </summary>
        Error
    }
}
