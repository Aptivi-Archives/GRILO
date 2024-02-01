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
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class LiloBootStyle : BaseBootStyle, IBootStyle
    {
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Populate colors
            ConsoleColor sectionTitle = ConsoleColor.Yellow;
            ConsoleColor boxBorderColor = ConsoleColor.Gray;
            ConsoleColor boxBorderBackgroundColor = ConsoleColor.DarkRed;
            ConsoleColor promptColor = ConsoleColor.Gray;

            // Write the box
            int interiorWidth = 41;
            int interiorHeight = 12;
            int bootPrompt = 16;
            int halfX = Console.WindowWidth / 2 - (interiorWidth + 2) / 2;
            int extraStartX = Console.WindowWidth / 2 - (interiorWidth + 4) / 2;
            int extraEndX = Console.WindowWidth / 2 + (interiorWidth + 4) / 2;
            int startY = 1;
            int endY = bootPrompt + startY - 2;
            BorderColor.WriteBorder(halfX, startY, interiorWidth, interiorHeight,
                '╓', '╙', '╖', '╜', '─', '─', '║', '║', new Color(boxBorderColor), new Color(boxBorderBackgroundColor));
            for (int y = startY; y < endY; y++)
            {
                TextWriterWhereColor.WriteWhereColorBack(" ", extraStartX, y, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));
                TextWriterWhereColor.WriteWhereColorBack(" ", extraEndX, y, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));
            }

            // Offer the boot prompt
            TextWriterWhereColor.WriteWhereColor("boot: ", 0, bootPrompt, new Color(promptColor));

            // Now, fill the box with usual things, starting from the title
            string title = "LILO 22.7  Boot Menu";
            int titleX = Console.WindowWidth / 2 - title.Length / 2;
            int titleY = 3;
            TextWriterWhereColor.WriteWhereColorBack(title, titleX, titleY, new Color(sectionTitle), new Color(boxBorderBackgroundColor));

            // The two separators
            int separator1Y = 5;
            int separator2Y = 10;
            string separator1 = "╟──┬─────────────────╥──┬─────────────────╢";
            string separator2 = "╟──┴─────────────────╨──┴─────────────────╢";
            TextWriterWhereColor.WriteWhereColorBack(separator1, halfX, separator1Y, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));
            TextWriterWhereColor.WriteWhereColorBack(separator2, halfX, separator2Y, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));

            // Connecting the separators
            int startSepY = 6;
            int endSepY = 9;
            int connectX = halfX + 1;
            string separator = "  │                 ║  │                 ";
            for (int y = startSepY; y <= endSepY; y++)
                TextWriterWhereColor.WriteWhereColorBack(separator, connectX, y, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));

            // Write the help text
            int textY = 11;
            int textX = halfX + 1;
            string help =
                "Hit any key to cancel timeout\n" +
                "Use arrow keys to make selection\n" +
                "Enter choice & options, hit CR to boot";
            TextWriterWhereColor.WriteWhereColorBack(help, textX, textY, new Color(boxBorderColor), new Color(boxBorderBackgroundColor));
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor normalEntryFg = ConsoleColor.Gray;
            ConsoleColor normalEntryBg = ConsoleColor.DarkRed;
            ConsoleColor selectedEntryFg = ConsoleColor.DarkBlue;
            ConsoleColor selectedEntryBg = normalEntryFg;

            // Populate boot entries inside the box
            var bootApps = BootManager.GetBootApps();
            int interiorWidth = 41;
            int halfX = Console.WindowWidth / 2 - (interiorWidth + 2) / 2 + 4;
            (int, int) upperLeftCornerInterior = (halfX, 6);
            int maxItemsPerPage = 4;
            int currentPage = (int)Math.Truncate(chosenBootEntry / (double)maxItemsPerPage);
            int startIndex = maxItemsPerPage * currentPage;
            int endIndex = maxItemsPerPage * (currentPage + 1);
            int renderedAnswers = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i + 1 > bootApps.Count)
                    break;
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                string rendered = $" {bootApp} ";
                rendered = rendered.Length > 16 ? $" {rendered.Substring(1, 15)} " : rendered;
                var finalColorBg = i == chosenBootEntry ? selectedEntryBg : normalEntryBg;
                var finalColorFg = i == chosenBootEntry ? selectedEntryFg : normalEntryFg;
                TextWriterWhereColor.WriteWhereColorBack(rendered, upperLeftCornerInterior.Item1, upperLeftCornerInterior.Item2 + renderedAnswers, false, new Color(finalColorFg), new Color(finalColorBg));
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

        public override void RenderBootingMessage(string chosenBootName) { }

        public override void RenderBootFailedMessage(string content) =>
            RenderModalDialog(content);

        public override void RenderSelectTimeout(int timeout)
        {
            string help = $"{TimeSpan.FromSeconds(timeout):mm}:{TimeSpan.FromSeconds(timeout):ss}";
            int textY = 11;
            int interiorWidth = 41;
            int extraEndX = Console.WindowWidth / 2 + interiorWidth / 2 - help.Length;
            TextWriterWhereColor.WriteWhereColorBack(help, extraEndX, textY, true, new Color(ConsoleColor.Gray), new Color(ConsoleColor.DarkRed));
        }

        public override void ClearSelectTimeout()
        {
            string help = $"{TimeSpan.FromSeconds(Config.Instance.BootSelectTimeoutSeconds):mm}:{TimeSpan.FromSeconds(Config.Instance.BootSelectTimeoutSeconds):ss}";
            int textY = 11;
            int interiorWidth = 41;
            int extraEndX = Console.WindowWidth / 2 + interiorWidth / 2 - help.Length;
            TextWriterWhereColor.WriteWhereColorBack(new string(' ', help.Length), extraEndX, textY, true, new Color(ConsoleColor.Gray), new Color(ConsoleColor.DarkRed));
        }
    }
}
