﻿//
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

using GRILO.Bootloader.Boot.Apps;
using System;
using System.Collections.Generic;

namespace GRILO.Bootloader.Boot.Style
{
    /// <summary>
    /// Base boot style
    /// </summary>
    public abstract class BaseBootStyle : IBootStyle
    {
        /// <inheritdoc/>
        public virtual Dictionary<ConsoleKeyInfo, Action<BootAppInfo>> CustomKeys { get; }

        /// <inheritdoc/>
        public virtual string Render() =>
            "";

        /// <inheritdoc/>
        public virtual string RenderHighlight(int chosenBootEntry) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderModalDialog(string content) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderBootingMessage(string chosenBootName) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderBootFailedMessage(string content) =>
            "";

        /// <inheritdoc/>
        public virtual string RenderSelectTimeout(int timeout) =>
            "";

        /// <inheritdoc/>
        public virtual string ClearSelectTimeout() =>
            "";
    }
}