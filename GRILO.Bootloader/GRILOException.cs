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

using System;

namespace GRILO.Bootloader
{
    /// <summary>
    /// Internal bootloader error
    /// </summary>
    public class GRILOException : Exception
    {
        /// <summary>
        /// Makes a new instance of GRILO exception
        /// </summary>
        public GRILOException() :
            base("Unknown bootloader error!")
        { }

        /// <summary>
        /// Makes a new instance of GRILO exception
        /// </summary>
        /// <param name="message">Error message to use while creating a new instance of the exception</param>
        public GRILOException(string message) :
            base("Internal bootloader error: " + message)
        { }

        /// <summary>
        /// Makes a new instance of GRILO exception
        /// </summary>
        /// <param name="message">Error message to use while creating a new instance of the exception</param>
        /// <param name="innerException">Inner exception to use</param>
        public GRILOException(string message, Exception innerException) :
            base("Internal bootloader error: " + message, innerException)
        { }
    }
}
