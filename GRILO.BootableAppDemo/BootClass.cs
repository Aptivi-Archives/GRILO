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

using GRILO.Boot;
using System;

namespace GRILO.BootableAppDemo
{
    public class BootClass : IBootable
    {
        public string Title => "Bootable app demo";

        public bool ShutdownRequested { get; set; }

        public void Boot(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Hello world! From BootableAppDemo!");
            Console.WriteLine("Arguments passed: [{0}]", string.Join(", ", args));
            Console.WriteLine("Press any key to shutdown");
            Console.ReadKey(true);
            if ((args.Length > 0 && args[0] != "fail") || args.Length == 0)
                ShutdownRequested = true;
        }
    }
}
