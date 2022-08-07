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

using GRILO.Boot;

namespace GRILO.Bootloader.BootApps
{
    /// <summary>
    /// Bootable application information
    /// </summary>
    public class BootAppInfo
    {

        /// <summary>
        /// Bootable file path
        /// </summary>
        public string BootFile { get; }
        /// <summary>
        /// Boot app title (overrides one found in IBootable)
        /// </summary>
        public string OverriddenTitle { get; }
        /// <summary>
        /// Arguments to inject to the boot file
        /// </summary>
        public string[] Arguments { get; }
        /// <summary>
        /// Gets the bootable
        /// </summary>
        public IBootable Bootable { get; }

        internal BootAppInfo(string bootFile, string overriddenTitle, string[] arguments, IBootable bootable)
        {
            BootFile = bootFile;
            OverriddenTitle = overriddenTitle;
            Arguments = arguments;
            Bootable = bootable;
        }
    }
}
