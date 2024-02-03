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
using System.Text;
using Terminaux.Colors;
using Terminaux.Inputs;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Boot.Style.Styles
{
    internal class NtldrBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();
        public override Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        public override string Render()
        {
            // Prompt the user for selection
            var bootApps = BootManager.GetBootApps();
            var builder = new StringBuilder();
            builder.AppendLine("\n\nPlease select the operating system to start:\n\n");
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                bootEntryPositions.Add((0, 5 + i));
                builder.AppendLine($"    {bootApp}");
            }
            builder.AppendLine("\nUse the up and down arrow keys to move the highlight to your choice.");
            builder.AppendLine("Press ENTER to choose.\n\n\n");
            builder.AppendLine("For troubleshooting and advanced startup options for Windows, press F8.");
            return builder.ToString();
        }

        public override string RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            return TextWriterWhereColor.RenderWhere("    {0}", bootEntryPositions[chosenBootEntry].Item1, bootEntryPositions[chosenBootEntry].Item2, true, new Color(highlightedEntryForeground), new Color(highlightedEntryBackground), bootApp);
        }

        public override string RenderModalDialog(string content)
        {
            // Populate colors
            ColorTools.LoadBack(0);
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;
            var builder = new StringBuilder();

            builder.AppendLine(
               $"""
                
                {content}
                
                """
            );
            builder.AppendLine(
                $"{new Color(highlightedEntryForeground).VTSequenceForeground}" +
                $"{new Color(highlightedEntryBackground).VTSequenceBackground}" +
                 "    Continue" +
                $"{ColorTools.CurrentForegroundColor.VTSequenceForeground}" +
                $"{ColorTools.CurrentBackgroundColor.VTSequenceBackground}"
            );
            builder.AppendLine("\nUse the up and down arrow keys to move the highlight to your choice.");
            return builder.ToString();
        }

        public override string RenderBootFailedMessage(string content)
        {
            bool exiting = false;
            int choiceNum = 7;
            while (!exiting)
            {
                TextWriterColor.WritePlain(ShowBootFailure(choiceNum));
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
            return "";
        }

        public override string RenderSelectTimeout(int timeout)
        {
            ConsoleColor hintColor = ConsoleColor.Gray;
            var builder = new StringBuilder();
            int marginX = 2;
            int optionHelpY =
                bootEntryPositions.Count > 0 ?
                bootEntryPositions[bootEntryPositions.Count - 1].Item2 + 9 :
                17;
            int timeoutX = marginX + "Seconds until the highlighted choice will be started automatically: ".Length;
            builder.Append(
                TextWriterWhereColor.RenderWhere("Seconds until the highlighted choice will be started automatically:", marginX, optionHelpY, true, new Color(hintColor), ColorTools.CurrentBackgroundColor) +
                TextWriterWhereColor.RenderWhere($"{timeout} ", timeoutX, optionHelpY, true, new Color(hintColor), ColorTools.CurrentBackgroundColor)
            );
            return builder.ToString();
        }

        public override string ClearSelectTimeout()
        {
            int marginX = 2;
            int timeoutY =
                bootEntryPositions.Count > 0 ?
                bootEntryPositions[bootEntryPositions.Count - 1].Item2 + 9 :
                17;
            ConsoleColor hintColor = ConsoleColor.Gray;
            return TextWriterWhereColor.RenderWhere(new string(' ', Console.WindowWidth - 2), marginX, timeoutY, true, new Color(hintColor), ColorTools.CurrentBackgroundColor);
        }

        private string ShowBootFailure(int choiceNum)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;
            ColorTools.LoadBack();

            // Populate choices
            string[] choices =
            [
                "Safe Mode",
                "Safe Mode with Networking",
                "Safe Mode with Command Prompt",
                "",
                "Last Known Good Configuration (your most recent settings that worked)",
                "",
                "Start Windows Normally",
            ];

            // Print the message
            var builder = new StringBuilder();
            builder.AppendLine(
                """

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
                {
                    builder.Append(
                        $"{new Color(highlightedEntryForeground).VTSequenceForeground}" +
                        $"{new Color(highlightedEntryBackground).VTSequenceBackground}" +
                        $"    {choice}" +
                        $"{ColorTools.CurrentForegroundColor.VTSequenceForeground}" +
                        $"{ColorTools.CurrentBackgroundColor.VTSequenceBackground}"
                    );
                }
                else
                    builder.Append($"    {choice}");
                builder.AppendLine();
            }
            builder.AppendLine("\nUse the up and down arrow keys to move the highlight to your choice.");
            return builder.ToString();
        }
    }
}
