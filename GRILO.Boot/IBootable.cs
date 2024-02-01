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

namespace GRILO.Boot
{
    /// <summary>
    /// Bootable instance of class to make it bootable by GRILO.
    /// </summary>
    public interface IBootable
    {
        /// <summary>
        /// Title to show on boot menu screen. This will be overridden by the boot metadata json file if specified there. If not assigned by either, GRILO bootloader
        /// tries to get the title from assembly name.
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Detects whether the shutdown has been requested. If set, bootloader exits peacefully. Otherwise, GRILO will return to the boot menu with an error message
        /// indicating that the boot failed.
        /// </summary>
        bool ShutdownRequested { get; set; }
        /// <summary>
        /// Boots the class up (usually executing the entry point of the application)
        /// </summary>
        void Boot(string[] args);
    }
}
