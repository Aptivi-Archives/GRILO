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

namespace GRILO.Bootloader.BootStyle.Styles
{
    internal class DefaultBootStyle : BaseBootStyle, IBootStyle
    {
        internal List<(int, int)> bootEntryPositions = new();

        public override void Render()
        {
            // Populate colors
            ConsoleColor bootEntry = ConsoleColor.Blue;

            // Prompt the user for selection
            var bootApps = BootManager.GetBootApps();
            Console.WriteLine("\n  Select boot entry:\n\n");
            for (int i = 0; i < bootApps.Count; i++)
            {
                string bootApp = BootManager.GetBootAppNameByIndex(i);
                Console.ForegroundColor = bootEntry;
                bootEntryPositions.Add((Console.CursorLeft, Console.CursorTop));
                Console.WriteLine(" [{0}] {1}", i + 1, bootApp);
            }
        }

        public override void RenderHighlight(int chosenBootEntry)
        {
            // Populate colors
            ConsoleColor highlightedEntry = ConsoleColor.Cyan;

            // Highlight the chosen entry
            string bootApp = BootManager.GetBootAppNameByIndex(chosenBootEntry);
            Console.ForegroundColor = highlightedEntry;
            Console.CursorLeft = bootEntryPositions[chosenBootEntry].Item1;
            Console.CursorTop =  bootEntryPositions[chosenBootEntry].Item2;
            Console.WriteLine(" [{0}] {1}", chosenBootEntry + 1, bootApp);
        }

        public override void RenderModalDialog(string content)
        {
            // Populate colors
            ConsoleColor dialogBG = ConsoleColor.Red;
            ConsoleColor dialogFG = ConsoleColor.White;

            // Get the corner positions
            (int, int) modalDialogBorderTopLeft =       (5, 5);
            (int, int) modalDialogBorderTopRight =      (Console.WindowWidth - 5, 5);
            (int, int) modalDialogBorderBottomLeft =    (5, Console.WindowHeight - 5);
            (int, int) modalDialogBorderBottomRight =   (Console.WindowWidth - 5, Console.WindowHeight - 5);

            // Get the difference so we can draw the border
            int modalDialogBorderFromTopToBottom = modalDialogBorderBottomLeft.Item2 - modalDialogBorderTopLeft.Item2;
            int modalDialogBorderFromRightToLeft = modalDialogBorderBottomRight.Item1 - modalDialogBorderBottomLeft.Item1;

            // Get the border characters
            char modalDialogBorderCornerTopLeft =     Convert.ToChar(0x250C);
            char modalDialogBorderCornerTopRight =    Convert.ToChar(0x2510);
            char modalDialogBorderCornerBottomLeft =  Convert.ToChar(0x2514);
            char modalDialogBorderCornerBottomRight = Convert.ToChar(0x2518);
            char modalDialogBorderTopBottom =         Convert.ToChar(0x2500);
            char modalDialogBorderLeftRight =         Convert.ToChar(0x2502);

            // Change colors as necessary
            Console.BackgroundColor = dialogBG;
            Console.ForegroundColor = dialogFG;

            // Draw the corners
            Console.SetCursorPosition(modalDialogBorderTopLeft.Item1, modalDialogBorderTopLeft.Item2);
            Console.Write(modalDialogBorderCornerTopLeft);
            Console.SetCursorPosition(modalDialogBorderTopRight.Item1, modalDialogBorderTopRight.Item2);
            Console.Write(modalDialogBorderCornerTopRight);
            Console.SetCursorPosition(modalDialogBorderBottomLeft.Item1, modalDialogBorderBottomLeft.Item2);
            Console.Write(modalDialogBorderCornerBottomLeft);
            Console.SetCursorPosition(modalDialogBorderBottomRight.Item1, modalDialogBorderBottomRight.Item2);
            Console.Write(modalDialogBorderCornerBottomRight);

            // Now, draw the borders starting from top left to bottom left
            for (int i = 1; i < modalDialogBorderFromTopToBottom; i++)
            {
                int modalDialogBorderPosX = modalDialogBorderTopLeft.Item1;
                int modalDialogBorderPosY = modalDialogBorderTopLeft.Item2 + i;
                Console.SetCursorPosition(modalDialogBorderPosX, modalDialogBorderPosY);
                Console.Write(modalDialogBorderLeftRight);
            }

            // Top right to bottom right
            for (int i = 1; i < modalDialogBorderFromTopToBottom; i++)
            {
                int modalDialogBorderPosX = modalDialogBorderTopRight.Item1;
                int modalDialogBorderPosY = modalDialogBorderTopRight.Item2 + i;
                Console.SetCursorPosition(modalDialogBorderPosX, modalDialogBorderPosY);
                Console.Write(modalDialogBorderLeftRight);
            }

            // Top left to top right
            for (int i = 1; i < modalDialogBorderFromRightToLeft; i++)
            {
                int modalDialogBorderPosX = modalDialogBorderTopLeft.Item1 + i;
                int modalDialogBorderPosY = modalDialogBorderTopLeft.Item2;
                Console.SetCursorPosition(modalDialogBorderPosX, modalDialogBorderPosY);
                Console.Write(modalDialogBorderTopBottom);
            }

            // Bottom right to bottom left
            for (int i = 1; i < modalDialogBorderFromRightToLeft; i++)
            {
                int modalDialogBorderPosX = modalDialogBorderBottomLeft.Item1 + i;
                int modalDialogBorderPosY = modalDialogBorderBottomLeft.Item2;
                Console.SetCursorPosition(modalDialogBorderPosX, modalDialogBorderPosY);
                Console.Write(modalDialogBorderTopBottom);
            }

            // Fill the entire box
            for (int i = modalDialogBorderTopLeft.Item2 + 1; i < modalDialogBorderBottomLeft.Item2; i++)
            {
                for (int j = modalDialogBorderTopLeft.Item1 + 1; j < modalDialogBorderTopRight.Item1; j++)
                {
                    Console.SetCursorPosition(j, i);
                    Console.Write(" ");
                }
            }

            // Print the message
            string[] contents = content.Replace(Convert.ToChar(13), default).Split(Convert.ToChar(10));
            for (int i = 0; i < contents.Length; i++)
            {
                Console.SetCursorPosition(modalDialogBorderTopLeft.Item1 + 3, modalDialogBorderTopLeft.Item2 + 2 + i);
                Console.Write(contents[i]);
            }

            // Wait for input
            Console.ReadKey(true);
        }
    }
}
