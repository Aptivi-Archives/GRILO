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

namespace GRILO.Boot
{
    /// <summary>
    /// Bootable instance of class to make it bootable by GRILO.
    /// </summary>
    public interface IBootable
    {
        /// <summary>
        /// Title to show on boot menu screen. This will be overridden by the boot metadata json file if specified there. If not assigned by either, GRILO bootloader
        /// tries to get the title from assembly name.
        /// </summary>
        string Title { get; }
        /// <summary>
        /// Detects whether the shutdown has been requested. If set, bootloader exits peacefully. Otherwise, GRILO will return to the boot menu with an error message
        /// indicating that the boot failed.
        /// </summary>
        bool ShutdownRequested { get; set; }
        /// <summary>
        /// Boots the class up (usually executing the entry point of the application)
        /// </summary>
        void Boot(string[] args);
    }
}