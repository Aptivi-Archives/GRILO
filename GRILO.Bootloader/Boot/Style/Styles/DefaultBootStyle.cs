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
using GRILO.Bootloader.Common;
using GRILO.Bootloader.Common.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terminaux.Base;
using Terminaux.Colors;
using Terminaux.Inputs.Styles.Infobox;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;
using Textify.General;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class DefaultBootStyle : BaseBootStyle, IBootStyle
    {
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override string Render()
        {
            // Populate colors
            ConsoleColor sectionTitle = ConsoleColor.Green;
            ConsoleColor boxBorderColor = ConsoleColor.Gray;

            // Write the section title
            var builder = new StringBuilder();
            string finalRenderedSection = "-- Select boot entry --";
            int halfX = Console.WindowWidth / 2 - finalRenderedSection.Length / 2;
            builder.Append(
                TextWriterWhereColor.RenderWhere(finalRenderedSection, halfX, 2, new Color(sectionTitle), ColorTools.CurrentBackgroundColor)
            );

            // Now, render a box
            builder.Append(
                BorderColor.RenderBorder(2, 4, Console.WindowWidth - 6, Console.WindowHeight - 9, new Color(boxBorderColor), ColorTools.CurrentBackgroundColor)
            );

            // Offer help for new users
            string help = $"SHIFT + H for help. Version {Entry.griloVersion}";
            builder.Append(
                TextWriterWhereColor.RenderWhere(help, Console.WindowWidth - help.Length - 2, Console.WindowHeight - 2, new Color(ConsoleColor.White), ColorTools.CurrentBackgroundColor)
            );
            return builder.ToString();
        }

        public override string RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.DarkGreen;
            ConsoleColor normalEntry = ConsoleColor.Gray;
            ConsoleColor pageNumberColor = ConsoleColor.Gray;

            // Populate boot entries inside the box
            var builder = new StringBuilder();
            var bootApps = BootManager.GetBootApps();
            (int, int) upperLeftCornerInterior = (4, 6);
            (int, int) lowerLeftCornerInterior = (4, Console.WindowHeight - 6);
            int maxItemsPerPage = lowerLeftCornerInterior.Item2 - upperLeftCornerInterior.Item2;
            int pages = (int)Math.Truncate(bootApps.Count / (double)maxItemsPerPage);
            int currentPage = (int)Math.Truncate(chosenBootEntry / (double)maxItemsPerPage);
            int startIndex = maxItemsPerPage * currentPage;
            int endIndex = maxItemsPerPage * (currentPage + 1) - 1;
            int renderedAnswers = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i + 1 > bootApps.Count)
                    break;
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                string rendered = $" >> {bootApp}";
                var finalColor = i == chosenBootEntry ? highlightedEntry : normalEntry;
                builder.Append(
                    TextWriterWhereColor.RenderWhere(rendered, upperLeftCornerInterior.Item1, upperLeftCornerInterior.Item2 + renderedAnswers, new Color(finalColor), ColorTools.CurrentBackgroundColor)
                );
                renderedAnswers++;
            }

            // Populate page number
            string renderedNumber = $"[{chosenBootEntry + 1}/{bootApps.Count}]═[{currentPage + 1}/{pages}]";
            (int, int) lowerRightCornerToWrite = (Console.WindowWidth - renderedNumber.Length - 3, Console.WindowHeight - 4);
            builder.Append(
                TextWriterWhereColor.RenderWhere(renderedNumber, lowerRightCornerToWrite.Item1, lowerRightCornerToWrite.Item2, new Color(pageNumberColor), ColorTools.CurrentBackgroundColor)
            );
            return builder.ToString();
        }

        public override string RenderModalDialog(string content)
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

        public override string RenderBootingMessage(string chosenBootName) =>
            $"Booting {chosenBootName}...";

        public override string RenderBootFailedMessage(string content) =>
            content;

        public override string RenderSelectTimeout(int timeout) =>
            TextWriterWhereColor.RenderWhere($"{timeout} ", 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White), ColorTools.CurrentBackgroundColor);

        public override string ClearSelectTimeout()
        {
            string spaces = new(' ', GetDigits(Config.Instance.BootSelectTimeoutSeconds));
            return TextWriterWhereColor.RenderWhere(spaces, 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White), ColorTools.CurrentBackgroundColor);
        }

        internal static int GetDigits(int Number) =>
            Number == 0 ? 1 : (int)Math.Log10(Math.Abs(Number)) + 1;
    }
}
