//
// MIT License
//
// Copyright (c) 2022 Aptivi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//

using GRILO.Bootloader.BootApps;
using GRILO.Bootloader.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using Terminaux.Colors;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;

namespace GRILO.Bootloader.BootStyle.Styles
{
    internal class GrubBootStyle : BaseBootStyle, IBootStyle
    {
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Populate colors
            ConsoleColor sectionTitle = ConsoleColor.Gray;
            ConsoleColor boxBorderColor = ConsoleColor.DarkGray;

            // Write the section title
            string finalRenderedSection = "GNU GRUB  version 2.06";
            int halfX = (Console.WindowWidth / 2) - (finalRenderedSection.Length / 2);
            TextWriterWhereColor.WriteWhereColor(finalRenderedSection, halfX, 2, new Color(sectionTitle));

            // Now, render a box
            BorderColor.WriteBorder(2, 4, Console.WindowWidth - 6, Console.WindowHeight - 15, new Color(boxBorderColor));

            // Offer help for new users
            string help =
                $"Use the ↑ and ↓ keys to select which entry is highlighted.\n" +
                $"Press enter to boot the selected OS, `e' to edit the commands\n" +
                $"before booting or `c' for a command line.";
            int longest = help.Split(new[] { '\n' }).Max((text) => text.Length);
            TextWriterWhereColor.WriteWhereColor(help, (Console.WindowWidth / 2) - (longest / 2) - 2, Console.WindowHeight - 8, new Color(sectionTitle));
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
                string rendered = i == chosenBootEntry ? $"*{bootApp}" : $" {bootApp}";
                var finalColorBg = i == chosenBootEntry ? highlightedEntry : normalEntry;
                var finalColorFg = i == chosenBootEntry ? normalEntry : highlightedEntry;
                TextWriterWhereColor.WriteWhereColorBack(rendered + new string(' ', Console.WindowWidth - 6 - rendered.Length), upperLeftCornerInterior.Item1, upperLeftCornerInterior.Item2 + renderedAnswers, false, new Color(finalColorFg), new Color(finalColorBg));
                renderedAnswers++;
            }
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor dialogFG = ConsoleColor.Gray;
            TextWriterColor.WriteColor(content, true, new Color(dialogFG));
        }

        public override void RenderBootingMessage(string chosenBootName) { }

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
