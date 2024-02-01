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
using System.Collections.Generic;
using System;

namespace GRILO.Bootloader.BootStyle
{
    /// <summary>
    /// Boot style interface to customize how the bootloader looks
    /// </summary>
    public interface IBootStyle
    {
        /// <summary>
        /// Custom key assignments
        /// </summary>
        Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }
        /// <summary>
        /// Renders the bootloader style
        /// </summary>
        void Render();
        /// <summary>
        /// Renders the highlighted boot entry
        /// </summary>
        /// <param name="chosenBootEntry">Chosen boot entry index (from 0)</param>
        void RenderHighlight(int chosenBootEntry);
        /// <summary>
        /// Renders the modal dialog box with content
        /// </summary>
        /// <param name="content">Message to display in the box</param>
        void RenderModalDialog(string content);
        /// <summary>
        /// Renders the booting message
        /// </summary>
        /// <param name="chosenBootName">Chosen boot name</param>
        void RenderBootingMessage(string chosenBootName);
        /// <summary>
        /// Renders the boot failed message
        /// </summary>
        /// <param name="content">Message to display</param>
        void RenderBootFailedMessage(string content);
        /// <summary>
        /// Renders the timeout for selection
        /// </summary>
        /// <param name="timeout">Target timeout in seconds to count down from</param>
        void RenderSelectTimeout(int timeout);
        /// <summary>
        /// Clears the timeout for selection
        /// </summary>
        void ClearSelectTimeout();
    }
}
