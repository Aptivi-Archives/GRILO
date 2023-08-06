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
using System.Collections.Generic;
using System;

namespace GRILO.Bootloader.BootStyle
{
    /// <summary>
    /// Boot style interface to customize how the bootloader looks
    /// </summary>
    public interface IBootStyle
    {
        /// <summary>
        /// Custom key assignments
        /// </summary>
        Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }
        /// <summary>
        /// Renders the bootloader style
        /// </summary>
        void Render();
        /// <summary>
        /// Renders the highlighted boot entry
        /// </summary>
        /// <param name="chosenBootEntry">Chosen boot entry index (from 0)</param>
        void RenderHighlight(int chosenBootEntry);
        /// <summary>
        /// Renders the modal dialog box with content
        /// </summary>
        /// <param name="content">Message to display in the box</param>
        void RenderModalDialog(string content);
        /// <summary>
        /// Renders the booting message
        /// </summary>
        /// <param name="chosenBootName">Chosen boot name</param>
        void RenderBootingMessage(string chosenBootName);
        /// <summary>
        /// Renders the boot failed message
        /// </summary>
        /// <param name="content">Message to display</param>
        void RenderBootFailedMessage(string content);
        /// <summary>
        /// Renders the timeout for selection
        /// </summary>
        /// <param name="timeout">Target timeout in seconds to count down from</param>
        void RenderSelectTimeout(int timeout);
        /// <summary>
        /// Clears the timeout for selection
        /// </summary>
        void ClearSelectTimeout();
    }
}
