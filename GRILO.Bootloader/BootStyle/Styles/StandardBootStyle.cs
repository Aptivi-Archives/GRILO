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
using Terminaux.Colors;
using Terminaux.Inputs.Styles.Infobox;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.BootStyle.Styles
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
