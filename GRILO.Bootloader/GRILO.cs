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
using GRILO.Bootloader.Diagnostics;
using GRILO.Bootloader.KeyHandler;
using System.Linq;
using Terminaux.Sequences.Builder;
using Terminaux.Reader.Inputs;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader
{
    internal class GRILO
    {
        internal static bool shutdownRequested = false;
        internal static bool waitingForBootKey = true;
        internal static bool waitingForFirstBootKey = true;
        internal static string griloVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
        internal static bool isOnAlternativeBuffer = false;

        static void Main()
        {
            try
            {
                // Preload bootloader
                Console.CursorVisible = false;
                TextWriterColor.Write("Starting GRILO v{0}...", Assembly.GetExecutingAssembly().GetName().Version.ToString());

                // Populate GRILO folders (if any)
                GRILOPaths.MakePaths();

                // Read the configuration (or make one if not found)
                Config.ReadConfig();
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Config read successfully.");

                // Populate custom boot styles
                BootStyleManager.PopulateCustomBootStyles();
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Custom boot styles read successfully.");

                // Populate the bootable apps list
                BootManager.PopulateBootApps();
                var bootApps = BootManager.GetBootApps();
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Bootable apps read successfully.");

                // Switch to alternative buffer
                if (!GRILOPlatform.IsOnWindows())
                {
                    TextWriterColor.WritePlain($"{VtSequenceBasicChars.EscapeChar}7{VtSequenceBasicChars.EscapeChar}[?47h", false);
                    isOnAlternativeBuffer = true;
                }

                // Now, draw the boot menu. Note that the chosen boot entry counts from zero.
                int chosenBootEntry = Config.Instance.BootSelect;
                while (!shutdownRequested)
                {
                    while (waitingForBootKey)
                    {
                        // Reset console colors in case app or boot style didn't reset them
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.ForegroundColor = ConsoleColor.White;

                        // Render the menu
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Rendering menu...");
                        Console.Clear();
                        BootStyleManager.RenderMenu(chosenBootEntry);

                        // Wait for a key and parse it
                        int timeout = Config.Instance.BootSelectTimeoutSeconds;
                        BootStyleManager.RenderSelectTimeout(timeout);
                        ConsoleKeyInfo cki;
                        if (timeout > 0 && waitingForFirstBootKey)
                        {
                            try
                            {
                                cki = Input.ReadKeyTimeout(true, TimeSpan.FromSeconds(Config.Instance.BootSelectTimeoutSeconds));
                            }
                            catch
                            {
                                cki = new('\x0A', ConsoleKey.Enter, false, false, false);
                            }
                        }
                        else
                            cki = Input.DetectKeypress();
                        waitingForFirstBootKey = false;
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Key pressed: {0}", cki.Key.ToString());
                        switch (cki.Key)
                        {
                            case ConsoleKey.UpArrow:
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Decrementing boot entry...");
                                chosenBootEntry--;

                                // If we reached the beginning of the boot menu, go to the ending
                                if (chosenBootEntry < 0)
                                {
                                    chosenBootEntry = bootApps.Count - 1;
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "We're at the beginning! Chosen boot entry is now {0}", chosenBootEntry);
                                }
                                break;
                            case ConsoleKey.DownArrow:
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Incrementing boot entry...");
                                chosenBootEntry++;

                                // If we reached the ending of the boot menu, go to the beginning
                                if (chosenBootEntry > bootApps.Count - 1)
                                {
                                    chosenBootEntry = 0;
                                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "We're at the ending! Chosen boot entry is now {0}", chosenBootEntry);
                                }
                                break;
                            case ConsoleKey.Home:
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Decrementing boot entry to the first entry...");
                                chosenBootEntry = 0;
                                break;
                            case ConsoleKey.End:
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Decrementing boot entry to the last entry...");
                                chosenBootEntry = bootApps.Count - 1;
                                break;
                            case ConsoleKey.H:
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Opening controls page...");
                                var style = BootStyleManager.GetCurrentBootStyle();
                                if (cki.Modifiers.HasFlag(ConsoleModifiers.Shift))
                                    BootStyleManager.RenderDialog(
                                        $"""
                                        Standard controls
                                        -----------------

                                        [UP ARROW]   | Selects the previous boot entry
                                        [DOWN ARROW] | Selects the next boot entry
                                        [HOME]       | Selects the first boot entry
                                        [END]        | Selects the last boot entry
                                        [SHIFT + H]  | Opens this help page
                                        [ENTER]      | Boots the selected entry

                                        Controls defined by custom boot style
                                        -------------------------------------

                                        {(style.CustomKeys is not null && style.CustomKeys.Count > 0 ?
                                          string.Join("\n", style.CustomKeys
                                              .Select((cki) => $"[{string.Join(" + ", cki.Key.Modifiers)} + {cki.Key.Key}]")) :
                                          "No controls defined by custom boot style")}
                                        """
                                    );
                                break;
                            case ConsoleKey.Enter:
                                // We're no longer waiting for boot key
                                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Booting...");
                                waitingForBootKey = false;
                                break;
                            default:
                                string chosenBootName = BootManager.GetBootAppNameByIndex(chosenBootEntry);
                                var chosenBootApp = BootManager.GetBootApp(chosenBootName);
                                Handler.HandleKey(cki, chosenBootApp);
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
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot name {0} at index {1}", chosenBootName, chosenBootEntry);

                        BootStyleManager.RenderBootingMessage(chosenBootName);
                        chosenBootApp.Bootable.Boot(chosenBootApp.Arguments);

                        shutdownRequested = chosenBootApp.Bootable.ShutdownRequested;
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot app done and shutdown requested is {0}", shutdownRequested);
                    }
                    catch (Exception ex)
                    {
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Unknown boot failure: {0}", ex.Message);
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Stack trace:\n{0}", ex.StackTrace);
                        bootFailureException = ex;
                    }

                    // Check to see if we experienced boot failure
                    if (!shutdownRequested)
                    {
                        DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Boot failed: {0}", bootFailureException.Message);
                        BootStyleManager.RenderBootFailedMessage($"Encountered boot failure.\nReason: {bootFailureException.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                // Failed trying to preload the bootloader
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Preload bootloader failed: {0}", ex.Message);
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Stack trace:\n{0}", ex.StackTrace);
                TextWriterColor.Write("Failed to preload bootloader: {0}", ex.Message);
                TextWriterColor.Write(ex.StackTrace);
                TextWriterColor.Write("Press any key to exit.");
                Input.DetectKeypress();
            }
            finally
            {
                if (isOnAlternativeBuffer)
                    TextWriterColor.WritePlain($"{VtSequenceBasicChars.EscapeChar}[2J{VtSequenceBasicChars.EscapeChar}[?47l{VtSequenceBasicChars.EscapeChar}8", false);
                Console.CursorVisible = true;
            }
        }
    }
}
