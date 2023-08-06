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

using GRILO.Bootloader.Configuration;
using System.IO;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Diagnostics
{
    /// <summary>
    /// Stores functions needed to write diagnostic messages to the bootloader
    /// </summary>
    public static class DiagnosticsWriter
    {
        /// <summary>
        /// Writes diagnostic messages
        /// </summary>
        /// <param name="level">Diagnostics level</param>
        /// <param name="content">Content to write</param>
        /// <param name="args">Arguments</param>
        public static void WriteDiag(DiagnosticsLevel level, string content, params object[] args)
        {
            if (Config.Instance.DiagnosticMessages)
            {
                // Print diagnostic messages to the console
                if (Config.Instance.PrintDiagnosticMessages)
                    TextWriterColor.Write($"[{level.ToString()[0]}] {content}", args);

                // Print diagnostic messages to the debug file
                var writer = File.AppendText(GRILOPaths.GRILODebugPath);
                writer.WriteLine(string.Format($"[{level.ToString()[0]}] {content}", args));
                writer.Flush();
                writer.Close();
            }
        }
    }
}
