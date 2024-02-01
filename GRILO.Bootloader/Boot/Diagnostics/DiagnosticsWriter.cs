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

using GRILO.Bootloader.Common.Configuration;
using System.IO;
using Terminaux.Writer.ConsoleWriters;

namespace GRILO.Bootloader.Boot.Diagnostics
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
                var writer = File.AppendText(ConfigPaths.GRILODebugPath);
                writer.WriteLine(string.Format($"[{level.ToString()[0]}] {content}", args));
                writer.Flush();
                writer.Close();
            }
        }
    }
}
