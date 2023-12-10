//
// MIT License
//
// Copyright (c) 2022 Aptivi
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
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
