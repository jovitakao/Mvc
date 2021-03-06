﻿// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using Microsoft.AspNetCore.Mvc.ApplicationParts;

namespace Microsoft.AspNetCore.Mvc.Razor.Compilation
{
    public static class CompiledViewManfiest
    {
        public static readonly string PrecompiledViewsAssemblySuffix = ".PrecompiledViews";

        public static Type GetManifestType(AssemblyPart assemblyPart, string typeName)
        {
            EnsureFeatureAssembly(assemblyPart);

            var precompiledAssemblyName = new AssemblyName(assemblyPart.Assembly.FullName);
            precompiledAssemblyName.Name = precompiledAssemblyName.Name + PrecompiledViewsAssemblySuffix;

            return Type.GetType($"{typeName},{precompiledAssemblyName}");
        }

        private static void EnsureFeatureAssembly(AssemblyPart assemblyPart)
        {
            if (assemblyPart.Assembly.IsDynamic || string.IsNullOrEmpty(assemblyPart.Assembly.Location))
            {
                return;
            }

            var precompiledAssemblyFileName = assemblyPart.Assembly.GetName().Name
                + PrecompiledViewsAssemblySuffix
                + ".dll";
            var precompiledAssemblyFilePath = Path.Combine(
                Path.GetDirectoryName(assemblyPart.Assembly.Location),
                precompiledAssemblyFileName);

            if (File.Exists(precompiledAssemblyFilePath))
            {
                try
                {
                    Assembly.LoadFile(precompiledAssemblyFilePath);
                }
                catch (FileLoadException)
                {
                    // Don't throw if assembly cannot be loaded. This can happen if the file is not a managed assembly.
                }
            }
        }
    }
}
