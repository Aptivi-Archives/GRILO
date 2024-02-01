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

using GRILO.Bootloader.BootApps;
using GRILO.Bootloader.BootStyle;
using GRILO.Bootloader.Configuration;
using System;

namespace GRILO.Bootloader.KeyHandler
{
    /// <summary>
    /// Key handling module
    /// </summary>
    internal static class Handler
    {
        /// <summary>
        /// Handles the key
        /// </summary>
        /// <param name="cki">Key information</param>
        /// <param name="chosenBootApp">Chosen boot application information class</param>
        internal static void HandleKey(ConsoleKeyInfo cki, BootAppInfo chosenBootApp)
        {
            var bootStyle = BootStyleManager.GetBootStyle(Config.Instance.BootStyleName);
            if (bootStyle.CustomKeys is null || bootStyle.CustomKeys.Count == 0)
                return;
            if (bootStyle.CustomKeys.TryGetValue(cki, out var action))
                action.Invoke(chosenBootApp);
        }
    }
}
