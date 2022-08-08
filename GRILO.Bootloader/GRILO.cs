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

using GRILO.Bootloader.BootStyle;
using GRILO.Bootloader.BootApps;
using GRILO.Bootloader.Configuration;
using System;
using System.Reflection;

namespace GRILO.Bootloader
{
    internal class GRILO
    {
        internal static bool diagMessages = false;
        internal static bool printDiagMessages = false;
        internal static bool shutdownRequested = false;
        internal static bool waitingForBootKey = true;

        static void Main()
        {
            try
            {
                // Preload bootloader
                Console.CursorVisible = false;
                Console.WriteLine("Starting GRILO v{0}...", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // Populate GRILO folders (if any)
                GRILOPaths.MakePaths();

                // Populate custom boot styles
                BootStyleManager.PopulateCustomBootStyles();

                // Populate the bootable apps list
                BootManager.PopulateBootApps();

                // Read the configuration (or make one if not found)
                Config.ReadConfig();

                // Now, draw the boot menu. Note that the chosen boot entry counts from zero.
                int chosenBootEntry = 0;
                while (!shutdownRequested)
                {
                    while (waitingForBootKey)
                    {
                        // Reset console colors in case app or boot style didn't reset them
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;

                        // Render the menu
                        Console.Clear();
                        BootStyleManager.RenderMenu(chosenBootEntry);

                        // Wait for a key and parse it
                        var cki = Console.ReadKey(true);
                        switch (cki.Key)
                        {
                            case ConsoleKey.UpArrow:
                                chosenBootEntry--;

                                // If we reached the beginning of the boot menu, go to the ending
                                if (chosenBootEntry < 0)
                                    chosenBootEntry = BootManager.GetBootApps().Count - 1;
                                break;
                            case ConsoleKey.DownArrow:
                                chosenBootEntry++;

                                // If we reached the ending of the boot menu, go to the beginning
                                if (chosenBootEntry > BootManager.GetBootApps().Count - 1)
                                    chosenBootEntry = 0;
                                break;
                            case ConsoleKey.Enter:
                            case ConsoleKey.RightArrow: // to simulate GRUB
                                // We're no longer waiting for boot key
                                waitingForBootKey = false;
                                break;
                            default:
                                break;
                        }
                    }
                    // Reset console colors
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.Clear();
                    waitingForBootKey = true;

                    // Boot the system
                    Exception bootFailureException = new GRILOException("Boot program failed.");
                    try
                    {
                        string chosenBootName = BootManager.GetBootAppNameByIndex(chosenBootEntry);
                        var chosenBootApp = BootManager.GetBootApp(chosenBootName);
                        Console.WriteLine("Booting {0}", chosenBootName);
                        chosenBootApp.Bootable.Boot(chosenBootApp.Arguments);
                        shutdownRequested = chosenBootApp.Bootable.ShutdownRequested;
                    }
                    catch (Exception ex)
                    {
                        bootFailureException = ex;
                    }

                    // Check to see if we experienced boot failure
                    if (!shutdownRequested)
                        BootStyleManager.RenderDialog($"Encountered boot failure.\nReason: {bootFailureException.Message}");
                }
            }
            catch (Exception ex)
            {
                // Failed trying to preload the bootloader
                Console.WriteLine("Failed to preload bootloader: {0}", ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.WriteLine("Press any key to exit.");
                Console.ReadKey(true);
            }
        }
    }
}