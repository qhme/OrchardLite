﻿using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Compilation;

namespace Orchard.Environment
{
    public interface IAssemblyBuilder
    {
        void AddCodeCompileUnit(CodeCompileUnit compileUnit);
        void AddAssemblyReference(Assembly assembly);
    }

    public class AspNetAssemblyBuilder : IAssemblyBuilder
    {
        private readonly AssemblyBuilder _assemblyBuilder;
        private readonly BuildProvider _buildProvider;

        public AspNetAssemblyBuilder(AssemblyBuilder assemblyBuilder, BuildProvider buildProvider)
        {
            _assemblyBuilder = assemblyBuilder;
            _buildProvider = buildProvider;
        }

        public void AddCodeCompileUnit(CodeCompileUnit compileUnit)
        {
            _assemblyBuilder.AddCodeCompileUnit(_buildProvider, compileUnit);
        }

        public void AddAssemblyReference(Assembly assembly)
        {
            _assemblyBuilder.AddAssemblyReference(assembly);
        }
    }

}
