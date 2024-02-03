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

using GRILO.Bootloader.Boot.Apps;
using System;
using System.Collections.Generic;
using System.Linq;
using Terminaux.Base;
using Terminaux.Colors;
using Terminaux.Writer.FancyWriters;
using Textify.General;

namespace GRILO.Bootloader.Boot.Style
{
    /// <summary>
    /// Base boot style
    /// </summary>
    public abstract class BaseBootStyle : IBootStyle
    {
        /// <inheritdoc/>
        public virtual Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        /// <inheritdoc/>
        public virtual string Render() =>
            "";

        /// <inheritdoc/>
        public virtual string RenderHighlight(int chosenBootEntry) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor dialogBG = ConsoleColor.Black;
            ConsoleColor dialogFG = ConsoleColor.Gray;
            ColorTools.LoadBack();

            var splitLines = content.SplitNewLines();
            int maxWidth = splitLines.Max((str) => str.Length);
            int maxHeight = splitLines.Length;
            if (maxWidth >= ConsoleWrapper.WindowWidth)
                maxWidth = ConsoleWrapper.WindowWidth - 4;
            if (maxHeight >= ConsoleWrapper.WindowHeight)
                maxHeight = ConsoleWrapper.WindowHeight - 4;
            int borderX = ConsoleWrapper.WindowWidth / 2 - maxWidth / 2 - 1;
            int borderY = ConsoleWrapper.WindowHeight / 2 - maxHeight / 2 - 1;
            return BorderTextColor.RenderBorder(content, borderX, borderY, maxWidth, maxHeight, new Color(dialogFG), new Color(dialogBG));
        }

        /// <inheritdoc/>
        public virtual string RenderBootingMessage(string chosenBootName) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderBootFailedMessage(string content) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderSelectTimeout(int timeout) =>
            "";

        /// <inheritdoc/>
        public virtual string ClearSelectTimeout() =>
            "";
    }
}
