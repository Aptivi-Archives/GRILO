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
using GRILO.Bootloader.Common.Configuration;
using System;
using System.Collections.Generic;
using Terminaux.Colors;
using Terminaux.Inputs.Styles.Infobox;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class StandardBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();

        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Populate colors
            ConsoleColor bootEntry = ConsoleColor.Blue;

            // Prompt the user for selection
            var bootApps = BootManager.GetBootApps();
            TextWriterColor.Write("\n  Select boot entry:\n", true);
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                bootEntryPositions.Add((Console.CursorLeft, Console.CursorTop));
                TextWriterColor.WriteColor(" [{0}] {1}", true, new Color(bootEntry), i + 1, bootApp);
            }
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.Cyan;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            TextWriterWhereColor.WriteWhereColor(" [{0}] {1}", bootEntryPositions[chosenBootEntry].Item1, bootEntryPositions[chosenBootEntry].Item2, new Color(highlightedEntry), chosenBootEntry + 1, bootApp);
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor dialogBG = ConsoleColor.Black;
            ConsoleColor dialogFG = ConsoleColor.Gray;
            InfoBoxColor.WriteInfoBoxColorBack(content, new Color(dialogFG), new Color(dialogBG));
            Console.Clear();
        }

        public override void RenderBootingMessage(string chosenBootName) =>
            TextWriterColor.Write("Booting {0}...", chosenBootName);

        public override void RenderBootFailedMessage(string content) =>
            RenderModalDialog(content);

        public override void RenderSelectTimeout(int timeout) =>
            TextWriterWhereColor.WriteWhereColor($" {timeout}", Console.WindowWidth - $" {timeout}".Length - 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));

        public override void ClearSelectTimeout()
        {
            string spaces = new(' ', DefaultBootStyle.GetDigits(Config.Instance.BootSelectTimeoutSeconds));
            TextWriterWhereColor.WriteWhereColor(spaces, Console.WindowWidth - spaces.Length - 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));
        }
    }
}
