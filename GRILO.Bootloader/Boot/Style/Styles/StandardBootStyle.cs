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
using System.Linq;
using System.Text;
using Terminaux.Base;
using Terminaux.Colors;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;
using Textify.General;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class StandardBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();

        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override string Render()
        {
            // Populate colors
            ConsoleColor bootEntry = ConsoleColor.Blue;

            // Prompt the user for selection
            var builder = new StringBuilder();
            var bootApps = BootManager.GetBootApps();
            builder.AppendLine("\n  Select boot entry:\n");
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                bootEntryPositions.Add((0, 3 + i));
                builder.AppendLine($"{ColorTools.RenderSetConsoleColor(new Color(bootEntry))} [{i + 1}] {bootApp}");
            }
            return builder.ToString();
        }

        public override string RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.Cyan;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            return TextWriterWhereColor.RenderWhereColor(" [{0}] {1}", bootEntryPositions[chosenBootEntry].Item1, bootEntryPositions[chosenBootEntry].Item2, true, new Color(highlightedEntry), chosenBootEntry + 1, bootApp);
        }

        public override string RenderBootingMessage(string chosenBootName) =>
            $"Booting {chosenBootName}...";

        public override string RenderBootFailedMessage(string content) =>
            RenderModalDialog(content);

        public override string RenderSelectTimeout(int timeout) =>
            TextWriterWhereColor.RenderWhereColor($" {timeout}", Console.WindowWidth - $" {timeout}".Length - 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));

        public override string ClearSelectTimeout()
        {
            string spaces = new(' ', GetDigits(Config.Instance.BootSelectTimeoutSeconds));
            return TextWriterWhereColor.RenderWhereColor(spaces, Console.WindowWidth - spaces.Length - 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));
        }
    }
}
