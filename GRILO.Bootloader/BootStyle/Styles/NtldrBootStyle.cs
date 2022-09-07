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
using System.Text;

namespace GRILO.Bootloader.BootStyle.Styles
{
    internal class NtldrBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();

        public override void Render()
        {
            // Prompt the user for selection
            var bootApps = BootManager.GetBootApps();
            Console.WriteLine("\n\nPlease select the operating system to start:\n\n");
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                bootEntryPositions.Add((Console.CursorLeft, Console.CursorTop));
                Console.WriteLine("    {0}", bootApp);
            }
            Console.WriteLine("\nUse the up and down arrow keys to move the highlight to your choice.");
            Console.WriteLine("Press ENTER to choose.\n\n\n");
            Console.WriteLine("For troubleshooting and advanced startup options for Windows, press F8.");
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            Console.ForegroundColor = highlightedEntryForeground;
            Console.BackgroundColor = highlightedEntryBackground;
            Console.CursorLeft = bootEntryPositions[chosenBootEntry].Item1;
            Console.CursorTop =  bootEntryPositions[chosenBootEntry].Item2;
            Console.WriteLine("    {0}", bootApp);
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor highlightedEntryForeground = ConsoleColor.Black;
            ConsoleColor highlightedEntryBackground = ConsoleColor.Gray;

            Console.WriteLine(@"
We apologize for the inconvenience, but Windows did not start successfully.  A
recent hardware or software change might have caused this.

If your computer stopped responding, restarted unexpectedly, or was
automatically shut down to protect your files or folders, choose Last Known
Good Configuration to revert to the most recent settings that worked.

If a previous startup attempt was interrupted due to a power failure or because
the Power or Reset button was pressed, or if you aren't sure what caused the
problem, choose Start Windows Normally.

    Safe Mode
    Safe Mode with Networking
    Safe Mode with Command Prompt

    Last Known Good Configuration (your most recent settings that worked)
");
            Console.ForegroundColor = highlightedEntryForeground;
            Console.BackgroundColor = highlightedEntryBackground;
            Console.WriteLine("    Start Windows Normally");
            Console.ResetColor();
            Console.WriteLine("\nUse the up and down arrow keys to move the highlight to your choice.");
        }
    }
}
