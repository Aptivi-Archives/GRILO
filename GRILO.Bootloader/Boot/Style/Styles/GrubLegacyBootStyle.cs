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
using Terminaux.Colors;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class GrubLegacyBootStyle : BaseBootStyle, IBootStyle
    {
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Populate colors
            ConsoleColor sectionTitle = ConsoleColor.Gray;
            ConsoleColor boxBorderColor = ConsoleColor.DarkGray;

            // Write the section title
            string finalRenderedSection = "GNU GRUB  version 0.97  (638K lower / 1046784K upper memory)";
            int halfX = Console.WindowWidth / 2 - finalRenderedSection.Length / 2;
            TextWriterWhereColor.WriteWhereColor(finalRenderedSection, halfX, 2, new Color(sectionTitle));

            // Now, render a box
            BorderColor.WriteBorder(2, 4, Console.WindowWidth - 6, Console.WindowHeight - 15, new Color(boxBorderColor));

            // Offer help for new users
            string help =
                $"Use the ↑ and ↓ keys to select which entry is highlighted.\n" +
                $"Press enter to boot the selected OS, `e' to edit the\n" +
                $"commands before booting or `c' for a command line.";
            int longest = help.Split(new[] { '\n' }).Max((text) => text.Length);
            TextWriterWhereColor.WriteWhereColor(help, Console.WindowWidth / 2 - longest / 2 - 2, Console.WindowHeight - 8, new Color(sectionTitle));
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.Gray;
            ConsoleColor normalEntry = ConsoleColor.Black;

            // Populate boot entries inside the box
            var bootApps = BootManager.GetBootApps();
            (int, int) upperLeftCornerInterior = (3, 5);
            (int, int) lowerLeftCornerInterior = (3, Console.WindowHeight - 9);
            int maxItemsPerPage = lowerLeftCornerInterior.Item2 - upperLeftCornerInterior.Item2 - 1;
            int currentPage = (int)Math.Truncate(chosenBootEntry / (double)maxItemsPerPage);
            int startIndex = maxItemsPerPage * currentPage;
            int endIndex = maxItemsPerPage * (currentPage + 1) - 1;
            int renderedAnswers = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i + 1 > bootApps.Count)
                    break;
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                string rendered = $" {bootApp}";
                var finalColorBg = i == chosenBootEntry ? highlightedEntry : normalEntry;
                var finalColorFg = i == chosenBootEntry ? normalEntry : highlightedEntry;
                TextWriterWhereColor.WriteWhereColorBack(rendered + new string(' ', Console.WindowWidth - 6 - rendered.Length), upperLeftCornerInterior.Item1, upperLeftCornerInterior.Item2 + renderedAnswers, false, new Color(finalColorFg), new Color(finalColorBg));
                renderedAnswers++;
            }
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ColorTools.LoadBack(0);
            ConsoleColor dialogFG = ConsoleColor.Gray;
            TextWriterColor.WriteColor(content, true, new Color(dialogFG));
        }

        public override void RenderBootingMessage(string chosenBootName) =>
            TextWriterColor.Write(
                $"  Booting '{chosenBootName}'\n\n" +
                $" Filesystem type is fat, partition type 0x0C"
            );

        public override void RenderBootFailedMessage(string content) =>
            RenderModalDialog(content);

        public override void RenderSelectTimeout(int timeout)
        {
            string help = $"The highlighted entry will be executed automatically in {timeout}s. ";
            TextWriterWhereColor.WriteWhereColor(help, 4, Console.WindowHeight - 5, true, new Color(ConsoleColor.White));
        }

        public override void ClearSelectTimeout()
        {
            string help = $"The highlighted entry will be executed automatically in {Config.Instance.BootSelectTimeoutSeconds}s. ";
            TextWriterWhereColor.WriteWhereColor(new string(' ', help.Length), 4, Console.WindowHeight - 5, true, new Color(ConsoleColor.White));
        }
    }
}
