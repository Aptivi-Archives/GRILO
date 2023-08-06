/*
 * MIT License
 * 
 * Copyright (c) 2022 Aptivi
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

using GRILO.Bootloader.BootApps;
using GRILO.Bootloader.Configuration;
using System;
using System.Collections.Generic;
using Terminaux.Colors;
using Terminaux.Writer.ConsoleWriters;
using Terminaux.Writer.FancyWriters;

namespace GRILO.Bootloader.BootStyle.Styles
{
    internal class DefaultBootStyle : BaseBootStyle, IBootStyle
    {
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Populate colors
            ConsoleColor sectionTitle = ConsoleColor.Green;
            ConsoleColor boxBorderColor = ConsoleColor.DarkGray;

            // Write the section title
            string finalRenderedSection = "-- Select boot entry --";
            int halfX = (Console.WindowWidth / 2) - (finalRenderedSection.Length / 2);
            TextWriterWhereColor.WriteWhere(finalRenderedSection, halfX, 2, new Color(sectionTitle));

            // Now, render a box
            BorderColor.WriteBorder(2, 4, Console.WindowWidth - 6, Console.WindowHeight - 9, new Color(boxBorderColor));

            // Offer help for new users
            string help = $"SHIFT + H for help. Version {GRILO.griloVersion}";
            TextWriterWhereColor.WriteWhere(help, Console.WindowWidth - help.Length - 2, Console.WindowHeight - 2, new Color(ConsoleColor.White));
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.DarkGreen;
            ConsoleColor normalEntry = ConsoleColor.Gray;
            ConsoleColor pageNumberColor = ConsoleColor.Gray;

            // Populate boot entries inside the box
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
                TextWriterWhereColor.WriteWhere(rendered, upperLeftCornerInterior.Item1, upperLeftCornerInterior.Item2 + renderedAnswers, new Color(finalColor));
                renderedAnswers++;
            }

            // Populate page number
            string renderedNumber = $"[{chosenBootEntry + 1}/{bootApps.Count}]═[{currentPage + 1}/{pages}]";
            (int, int) lowerRightCornerToWrite = (Console.WindowWidth - renderedNumber.Length - 3, Console.WindowHeight - 4);
            TextWriterWhereColor.WriteWhere(renderedNumber, lowerRightCornerToWrite.Item1, lowerRightCornerToWrite.Item2, new Color(pageNumberColor));
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor dialogBG = ConsoleColor.Black;
            ConsoleColor dialogFG = ConsoleColor.DarkGray;
            InfoBoxColor.WriteInfoBox(content, new Color(dialogFG), new Color(dialogBG));
            Console.Clear();
        }

        public override void RenderBootingMessage(string chosenBootName) =>
            TextWriterColor.Write("Booting {0}...", chosenBootName);

        public override void RenderBootFailedMessage(string content) =>
            TextWriterColor.Write(content);

        public override void RenderSelectTimeout(int timeout) =>
            TextWriterWhereColor.WriteWhere($"{timeout} ", 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));

        public override void ClearSelectTimeout()
        {
            string spaces = new(' ', GetDigits(GetDigits(Config.Instance.BootSelectTimeoutSeconds)));
            TextWriterWhereColor.WriteWhere(spaces, 2, Console.WindowHeight - 2, true, new Color(ConsoleColor.White));
        }

        internal static int GetDigits(int Number) =>
            Number == 0 ? 1 : (int)Math.Log10(Math.Abs(Number)) + 1;
    }
}
