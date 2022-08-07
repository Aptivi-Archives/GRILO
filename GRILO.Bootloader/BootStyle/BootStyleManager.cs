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

using GRILO.Bootloader;
using GRILO.Bootloader.BootStyle.Styles;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace GRILO.Bootloader.BootStyle
{
    /// <summary>
    /// Bootloader style management class
    /// </summary>
    public static class BootStyleManager
    {
        internal static string bootStyleStr = "Default";
        private static readonly Dictionary<string, BaseBootStyle> bootStyles = new()
        {
            { "Default", new DefaultBootStyle() }
        };
        private static readonly Dictionary<string, BaseBootStyle> customBootStyles = new();

        /// <summary>
        /// Installs custom boot styles to the bootloader
        /// </summary>
        public static void PopulateCustomBootStyles()
        {
            // Custom boot styles usually have a .DLL extension
            var styles = Directory.EnumerateFiles(GRILOPaths.GRILOStylesPath, "*.dll");
            foreach (var style in styles)
            {
                string styleName = Path.GetFileName(style);
                try
                {
                    // Load the custom boot style assembly file and check to see if it implements IBootStyle.
                    var asm = Assembly.LoadFrom(style);
                    BaseBootStyle bootStyle;

                    // Get the implemented types of the assembly so that we can check every type found in this assembly for implementation of the
                    // BaseBootStyle class.
                    foreach (Type t in asm.GetTypes())
                    {
                        if (t.GetInterface(typeof(BaseBootStyle).Name) != null)
                        {
                            // We found a boot style! Add it to the custom boot styles dictionary.
                            bootStyle = (BaseBootStyle)asm.CreateInstance(t.FullName);
                            customBootStyles.Add(styleName, bootStyle);
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Either the boot style is invalid or can't be loaded.
                    throw new GRILOException($"Can't load custom boot style {styleName}: {ex.Message}", ex);
                }
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
            bootStyles.TryGetValue(name, out BaseBootStyle bootStyle);

            // If not found, use the custom one
            if (bootStyle == null)
                customBootStyles.TryGetValue(name, out bootStyle);

            // If still not found, use Default
            if (bootStyle == null)
                customBootStyles.TryGetValue("Default", out bootStyle);

            // Return it.
            return bootStyle;
        }

        /// <summary>
        /// Renders the boot menu
        /// </summary>
        public static void RenderMenu(int chosenBootEntry)
        {
            // Get the base boot style from the current boot style name
            var bootStyle = GetBootStyle(bootStyleStr);

            // Render it.
            bootStyle.Render();
            bootStyle.RenderHighlight(chosenBootEntry);
        }
    }
}
