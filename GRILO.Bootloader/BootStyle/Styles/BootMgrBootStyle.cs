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
using System;
using System.Collections.Generic;
using Terminaux.Colors;
using Terminaux.Reader.Inputs;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.BootStyle.Styles
{
    internal class BootMgrBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Render the header and footer
            int marginX = 2;
            int headerY = 0;
            int footerY = Console.WindowHeight - 1;
            int barLength = Console.WindowWidth - 4;
            string header = "Windows Boot Manager";
            string footer = "ENTER=Choose";
            int headerTextX = (Console.WindowWidth / 2) - (header.Length / 2);
            ConsoleColor barColor = ConsoleColor.Gray;
            ConsoleColor barForeground = ConsoleColor.Black;
            TextWriterWhereColor.WriteWhereColorBack(new string(' ', barLength), marginX, headerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(new string(' ', barLength), marginX, footerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(header, headerTextX, headerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(footer, 3, footerY, new Color(barForeground), new Color(barColor));

            // Render the hints first
            ConsoleColor promptColor = ConsoleColor.White;
            ConsoleColor hintColor = ConsoleColor.Gray;
            int chooseHelpY = 2;
            int optionHelpY = 12;
            TextWriterWhereColor.WriteWhereColor("Choose an operating system to start, or press TAB to select a tool:", marginX, chooseHelpY, new Color(promptColor));
            TextWriterWhereColor.WriteWhereColor("(Use the arrow keys to highlight your choice, then press ENTER.)", marginX, chooseHelpY + 1, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor("To specify an advanced option for this choice, press F8.", marginX, optionHelpY, new Color(promptColor));
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor normalEntryFg = ConsoleColor.Gray;
            ConsoleColor normalEntryBg = ConsoleColor.Black;
            ConsoleColor selectedEntryFg = normalEntryBg;
            ConsoleColor selectedEntryBg = normalEntryFg;

            // Populate boot entries
            var bootApps = BootManager.GetBootApps();
            int maxItemsPerPage = 6;
            int currentPage = (int)Math.Truncate(chosenBootEntry / (double)maxItemsPerPage);
            int startIndex = maxItemsPerPage * currentPage;
            int endIndex = maxItemsPerPage * (currentPage + 1);
            int renderedAnswers = 0;
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i + 1 > bootApps.Count)
                    break;
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                string rendered = $"{bootApp}";
                var finalColorBg = i == chosenBootEntry ? selectedEntryBg : normalEntryBg;
                var finalColorFg = i == chosenBootEntry ? selectedEntryFg : normalEntryFg;
                TextWriterWhereColor.WriteWhereColorBack(rendered + new string(' ', Console.WindowWidth - 15 - rendered.Length) + (i == chosenBootEntry ? '>' : ' '), 6, 5 + renderedAnswers, false, new Color(finalColorFg), new Color(finalColorBg));
                renderedAnswers++;
            }
        }

        public override void RenderBootFailedMessage(string content)
        {
            // Render the header and footer
            int marginX = 2;
            int headerY = 0;
            int footerY = Console.WindowHeight - 1;
            int barLength = Console.WindowWidth - 4;
            string header = "Windows Boot Manager";
            string footer = "ENTER=Continue";
            int headerTextX = (Console.WindowWidth / 2) - (header.Length / 2);
            ConsoleColor barColor = ConsoleColor.Gray;
            ConsoleColor barForeground = ConsoleColor.Black;
            TextWriterWhereColor.WriteWhereColorBack(new string(' ', barLength), marginX, headerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(new string(' ', barLength), marginX, footerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(header, headerTextX, headerY, new Color(barForeground), new Color(barColor));
            TextWriterWhereColor.WriteWhereColorBack(footer, 3, footerY, new Color(barForeground), new Color(barColor));

            // Render the hints first
            ConsoleColor promptColor = ConsoleColor.White;
            ConsoleColor hintColor = ConsoleColor.Gray;
            int failedHelpY = 2;
            TextWriterWhereColor.WriteWhereColor("Windows failed to start. A recent hardware or software change might be the\ncause. To fix the problem:", marginX, failedHelpY, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor("1. Insert your Windows installation disc and restart your computer.", marginX + 2, failedHelpY + 3, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor("2. Choose your language settings, and then click \"Next.\"", marginX + 2, failedHelpY + 4, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor("3. Click \"Repair your computer.\"", marginX + 2, failedHelpY + 5, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor("If you do not have this disc, contact your system administrator or computer\nmanufacturer for assistance.", marginX, failedHelpY + 7, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor($"File: {new Color(promptColor).VTSequenceForeground}\\Boot\\BCD", marginX + 4, failedHelpY + 10, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor($"Status: {new Color(promptColor).VTSequenceForeground}0xc000000f", marginX + 4, failedHelpY + 12, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor($"Info:", marginX + 4, failedHelpY + 14, new Color(hintColor));
            TextWriterWhereColor.WriteWhereColor(content, marginX + 10, failedHelpY + 14, new Color(promptColor));
            Input.DetectKeypress();
            Console.Clear();
        }

        public override void RenderBootingMessage(string chosenBootName) { }

        public override void RenderModalDialog(string content) =>
            TextWriterColor.Write(content);

        public override void RenderSelectTimeout(int timeout)
        {
            ConsoleColor hintColor = ConsoleColor.Gray;
            int marginX = 2;
            int optionHelpY = 12;
            TextWriterWhereColor.WriteWhereColor("Seconds until the highlighted choice will be started automatically:", marginX, optionHelpY + 1, true, new Color(hintColor));
            int timeoutX = marginX + "Seconds until the highlighted choice will be started automatically: ".Length;
            int timeoutY = 13;
            TextWriterWhereColor.WriteWhereColor($"{timeout} ", timeoutX, timeoutY, true, new Color(hintColor));
        }

        public override void ClearSelectTimeout()
        {
            int marginX = 2;
            int timeoutY = 13;
            ConsoleColor hintColor = ConsoleColor.Gray;
            TextWriterWhereColor.WriteWhereColor(new string(' ', Console.WindowWidth - 2), marginX, timeoutY, true, new Color(hintColor));
        }
    }
}
