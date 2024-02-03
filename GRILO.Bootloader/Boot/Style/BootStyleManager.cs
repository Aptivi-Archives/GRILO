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

using GRILO.Bootloader.Boot.Diagnostics;
using GRILO.Bootloader.Boot.Style.Styles;
using GRILO.Bootloader.Common;
using GRILO.Bootloader.Common.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Terminaux.Base.Buffered;
using Terminaux.Inputs;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Boot.Style
{
    /// <summary>
    /// Bootloader style management class
    /// </summary>
    public static class BootStyleManager
    {
        private static readonly Thread timeoutThread = new((timeout) => SelectTimeoutHandler((int)timeout));
        private static readonly Dictionary<string, BaseBootStyle> bootStyles = new()
        {
            { "Default", new DefaultBootStyle() },
            { "Standard", new StandardBootStyle() },
            { "Ntldr", new NtldrBootStyle() },
            { "GRUB", new GrubBootStyle() },
            { "GRUBLegacy", new GrubLegacyBootStyle() },
            { "LILO", new LiloBootStyle() },
            { "BootMgr", new BootMgrBootStyle() },
        };
        private static readonly Dictionary<string, BaseBootStyle> customBootStyles = new();

        /// <summary>
        /// Installs custom boot styles to the bootloader
        /// </summary>
        public static void PopulateCustomBootStyles()
        {
            // Custom boot styles usually have a .DLL extension
            List<string> styles = Directory.EnumerateFiles(ConfigPaths.GRILOStylesPath, "*.dll").ToList();
            List<(string, Exception)> failedStyles = [];
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Custom boot styles: {0}", styles.Count);

            // Iterate through all found possible boot style .dll files
            foreach (var style in styles)
            {
                // Get the file name
                string styleName = Path.GetFileName(style);
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Style name: {0}", styleName);
                try
                {
                    // Load the custom boot style assembly file and check to see if it implements IBootStyle.
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Loading assembly {0}...", style);
                    var asm = Assembly.LoadFrom(style);
                    BaseBootStyle bootStyle;

                    // Get the implemented types of the assembly so that we can check every type found in this assembly for implementation of the
                    // BaseBootStyle class.
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Parsing boot style {0}...", styleName);
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.GetInterface(typeof(BaseBootStyle).Name) != null)
                        {
                            // We found a boot style! Add it to the custom boot styles dictionary.
                            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Boot style is valid! Adding...");
                            bootStyle = (BaseBootStyle)asm.CreateInstance(t.FullName);
                            customBootStyles.Add(styleName, bootStyle);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Either the boot style is invalid or can't be loaded.
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Can't load custom boot style. {0}", ex.Message);
                    DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "Stack trace:\n{0}", ex.StackTrace);
                    failedStyles.Add((styleName, new BootloaderException($"Can't load custom boot style {styleName}: {ex.Message}", ex)));
                }
            }

            // Check to see if there are any errors
            if (failedStyles.Count > 0)
            {
                var message = new StringBuilder();
                message.AppendLine($"GRILO failed to load {failedStyles.Count} custom boot styles.\n");
                foreach (var style in failedStyles)
                    message.AppendLine($"  - {style.Item1}: {style.Item2.Message}\n{style.Item2.StackTrace}");
                throw new BootloaderException(message.ToString());
            }
        }

        /// <summary>
        /// Gets the boot style from the name
        /// </summary>
        /// <param name="name">The name</param>
        /// <returns>The base boot style</returns>
        public static BaseBootStyle GetBootStyle(string name)
        {
            // Use the base boot styles first
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Getting boot style {0} from base boot styles...", name);
            bootStyles.TryGetValue(name, out BaseBootStyle bootStyle);

            // If not found, use the custom one
            if (bootStyle == null)
            {
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Getting boot style {0} from custom boot styles...", name);
                customBootStyles.TryGetValue(name, out bootStyle);
            }

            // If still not found, use Default
            if (bootStyle == null)
            {
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Still nothing. Using the default...");
                bootStyle = bootStyles["Default"];
            }

            // Return it.
            return bootStyle;
        }

        /// <summary>
        /// Renders the boot menu
        /// </summary>
        /// <param name="chosenBootEntry">Chosen boot entry index (from 0)</param>
        public static string RenderMenu(int chosenBootEntry)
        {
            // Render it.
            var bootStyle = GetCurrentBootStyle();
            var rendered = new StringBuilder();
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Rendering menu with chosen boot entry {0}...", chosenBootEntry);
            rendered.Append(
                bootStyle.Render() +
                bootStyle.RenderHighlight(chosenBootEntry)
            );
            return rendered.ToString();
        }

        /// <summary>
        /// Renders the boot message
        /// </summary>
        /// <param name="chosenBootName">Chosen boot name</param>
        public static string RenderBootingMessage(string chosenBootName)
        {
            // Render it.
            var bootStyle = GetCurrentBootStyle();
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Rendering booting message with chosen boot name {0}...", chosenBootName);
            return bootStyle.RenderBootingMessage(chosenBootName);
        }

        /// <summary>
        /// Renders the modal dialog box
        /// </summary>
        /// <param name="content">Message to display in the box</param>
        public static string RenderDialog(string content)
        {
            Console.Clear();

            // Render it.
            var bootStyle = GetCurrentBootStyle();
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Rendering modal dialog with content: {0}...", content);
            return bootStyle.RenderModalDialog(content);
        }

        /// <summary>
        /// Renders the boot failed message
        /// </summary>
        /// <param name="content">Message to display</param>
        public static string RenderBootFailedMessage(string content)
        {
            Console.Clear();

            // Render it.
            var bootStyle = GetCurrentBootStyle();
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Rendering boot failed message with content: {0}...", content);
            return bootStyle.RenderBootFailedMessage(content);
        }

        /// <summary>
        /// Renders the selection timeout
        /// </summary>
        /// <param name="timeout">Timeout interval in seconds</param>
        public static void RenderSelectTimeout(int timeout)
        {
            if (!timeoutThread.IsAlive && timeout > 0 && BootloaderState.WaitingForFirstBootKey)
                timeoutThread.Start(timeout);
        }

        /// <summary>
        /// Gets the current boot style
        /// </summary>
        /// <returns>The current boot style instance</returns>
        public static BaseBootStyle GetCurrentBootStyle()
        {
            string bootStyleStr = Config.Instance.BootStyleName;
            var bootStyle = GetBootStyle(bootStyleStr);
            DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Got boot style from {0}...", bootStyleStr);
            return bootStyle;
        }

        private static void SelectTimeoutHandler(int timeout)
        {
            var style = GetCurrentBootStyle();
            int timeoutElapsed = 0;
            var bufferClear = new ScreenPart();
            try
            {
                while (timeoutElapsed < timeout && BootloaderState.WaitingForFirstBootKey)
                {
                    // Render the timeout area since it isn't elapsed
                    var buffer = new ScreenPart();
                    buffer.AddDynamicText(() => style.RenderSelectTimeout(timeout - timeoutElapsed));
                    BootloaderMain.bootloaderScreen.AddBufferedPart("Timeout", buffer);
                    ScreenTools.Render();
                    Thread.Sleep(1000);
                    BootloaderMain.bootloaderScreen.RemoveBufferedPart("Timeout");
                    timeoutElapsed += 1;
                }
            }
            catch
            {
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Warning, "Failed to render select timeout.");
            }

            // Clear the timeout area
            bufferClear.AddDynamicText(style.ClearSelectTimeout);
            BootloaderMain.bootloaderScreen.AddBufferedPart("Clear timeout", bufferClear);
            ScreenTools.Render();
            BootloaderMain.bootloaderScreen.RemoveBufferedPart("Clear timeout");
        }
    }
}
