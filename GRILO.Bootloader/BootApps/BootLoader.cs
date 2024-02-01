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

#if !NETCOREAPP
using GRILO.Boot;
using GRILO.Bootloader.Diagnostics;
using System;
using System.IO;
using System.Reflection;

namespace GRILO.Bootloader.BootApps
{
    internal class BootLoader : MarshalByRefObject
    {
        public string lookupPath = "";
        public bool shutting = false;
        internal byte[] bytes = Array.Empty<byte>();

        internal void ProxyExecuteBootable(params string[] args)
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.ResolveDependency;
            var asm = Assembly.Load(bytes);
            IBootable proxy = null;
            foreach (Type t in asm.GetTypes())
            {
                if (t.GetInterface(typeof(IBootable).Name) != null)
                {
                    proxy = (IBootable)asm.CreateInstance(t.FullName);
                    break;
                }
            }
            proxy.Boot(args);
            AppDomain.CurrentDomain.AssemblyResolve -= this.ResolveDependency;
            shutting = proxy.ShutdownRequested;
        }

        internal bool VerifyBoot(byte[] bootBytes)
        {
            AppDomain.CurrentDomain.AssemblyResolve += this.ResolveDependency;
            var asm = Assembly.Load(bootBytes);
            bool validBoot = false;
            foreach (Type t in asm.GetTypes())
            {
                if (t.GetInterface(typeof(IBootable).Name) != null)
                    validBoot = true;
            }
            if (validBoot)
                bytes = bootBytes;
            AppDomain.CurrentDomain.AssemblyResolve -= this.ResolveDependency;
            return validBoot;
        }

        internal Assembly ResolveDependency(object sender, ResolveEventArgs args)
        {
            string depAssemblyName = new AssemblyName(args.Name).Name;
            string depAssemblyFilePath = Path.Combine(lookupPath, depAssemblyName + ".dll");
            try
            {
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Info, "Loading {0}", depAssemblyFilePath);
                return Assembly.LoadFrom(depAssemblyFilePath);
            }
            catch (Exception ex)
            {
                DiagnosticsWriter.WriteDiag(DiagnosticsLevel.Error, "{0}: {1}", ex.GetType().Name, ex.Message);
                return null;
            }
        }
    }
}
#endif
