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
using GRILO.Bootloader.Boot.Diagnostics;
using System;
using System.Collections.Generic;
using Terminaux.Colors;
using Terminaux.Inputs;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class NtldrBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override void Render()
        {
            // Prompt the user for selection
            var bootApps = BootManager.GetBootApps();
            TextWriterColor.Write("\n\nPlease select the operating system to start:\n\n");
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                bootEntryPositions.Add((Console.CursorLeft, Console.CursorTop));
                TextWriterColor.Write("    {0}", bootApp);
            }
            TextWriterColor.Write("\nUse the up and down arrow keys to move the highlight to your choice.");
            TextWriterColor.Write("Press ENTER to choose.\n\n\n");
            TextWriterColor.Write("For troubleshooting and advanced startup options for Windows, press F8.");
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            TextWriterWhereColor.WriteWhereColorBack("    {0}", bootEntryPositions[chosenBootEntry].Item1, bootEntryPositions[chosenBootEntry].Item2, new Color(highlightedEntryForeground), new Color(highlightedEntryBackground), bootApp);
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ColorTools.LoadBack(0);
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;

            TextWriterColor.Write($"""
                
                {content}
                
                """);
            TextWriterColor.WriteColorBack("    Continue", true, new Color(highlightedEntryForeground), new Color(highlightedEntryBackground));
            TextWriterColor.Write("\nUse the up and down arrow keys to move the highlight to your choice.");
        }

        public override void RenderBootingMessage(string chosenBootName) { }

        public override void RenderBootFailedMessage(string content)
        {
            bool exiting = false;
            int choiceNum = 7;
            while (!exiting)
            {
                ShowBootFailure(choiceNum);
                var cki = Input.DetectKeypress();
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Key pressed: {0}", cki.Key.ToString());
                switch (cki.Key)
                {
                    case ConsoleKey.Enter:
                        exiting = true;
                        break;
                    case ConsoleKey.UpArrow:
                        choiceNum--;
                        if (choiceNum == 4 || choiceNum == 6)
                            choiceNum--;
                        if (choiceNum == 0)
                            choiceNum = 7;
                        break;
                    case ConsoleKey.DownArrow:
                        choiceNum++;
                        if (choiceNum == 4 || choiceNum == 6)
                            choiceNum++;
                        if (choiceNum == 8)
                            choiceNum = 0;
                        break;
                }
            }
        }

        public override void RenderSelectTimeout(int timeout)
        {
            ConsoleColor hintColor = ConsoleColor.Gray;
            int marginX = 2;
            int optionHelpY = 17;
            TextWriterWhereColor.WriteWhereColor("Seconds until the highlighted choice will be started automatically:", marginX, optionHelpY, true, new Color(hintColor));
            int timeoutX = marginX + "Seconds until the highlighted choice will be started automatically: ".Length;
            TextWriterWhereColor.WriteWhereColor($"{timeout} ", timeoutX, optionHelpY, true, new Color(hintColor));
        }

        public override void ClearSelectTimeout()
        {
            int marginX = 2;
            int timeoutY = 17;
            ConsoleColor hintColor = ConsoleColor.Gray;
            TextWriterWhereColor.WriteWhereColor(new string(' ', Console.WindowWidth - 2), marginX, timeoutY, true, new Color(hintColor));
        }

        private void ShowBootFailure(int choiceNum)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;
            Console.Clear();

            // Populate choices
            string[] choices = new[]
            {
                "Safe Mode",
                "Safe Mode with Networking",
                "Safe Mode with Command Prompt",
                "",
                "Last Known Good Configuration (your most recent settings that worked)",
                "",
                "Start Windows Normally",
            };

            // Print the message
            TextWriterColor.Write("""

                We apologize for the inconvenience, but Windows did not start successfully.  A
                recent hardware or software change might have caused this.

                If your computer stopped responding, restarted unexpectedly, or was
                automatically shut down to protect your files or folders, choose Last Known
                Good Configuration to revert to the most recent settings that worked.

                If a previous startup attempt was interrupted due to a power failure or because
                the Power or Reset button was pressed, or if you aren't sure what caused the
                problem, choose Start Windows Normally.

                """);
            for (int i = 0; i < choices.Length; i++)
            {
                string choice = choices[i];
                if (i == choiceNum - 1)
                    TextWriterColor.WriteColorBack($"    {choice}", false, new Color(highlightedEntryForeground), new Color(highlightedEntryBackground));
                else
                    TextWriterColor.Write($"    {choice}", false);
                TextWriterColor.Write();
            }
            TextWriterColor.Write("\nUse the up and down arrow keys to move the highlight to your choice.");
        }
    }
}
