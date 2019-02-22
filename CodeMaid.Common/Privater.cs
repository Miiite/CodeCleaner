using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;

namespace CodeMaid.Common
{
    public class Privater : ICleanerStep
    {
        public Privater()
        {
        }

        public IEnumerable<ISymbol> MakePrivatesVisible(IEnumerable<ISymbol> symbols)
        {
            var methods = symbols.GetMethods();
            foreach (var method in methods)
            {
                if (method.DeclaredAccessibility == Accessibility.Private)
                {
                    //Console.WriteLine(method.Name + " " + ((ISymbolMethod)method).Modifiers.ToString());
                }
            }

            return symbols;
        }

        public IEnumerable<ISymbol> Run(IEnumerable<ISymbol> symbols) => this.MakePrivatesVisible(symbols);
    }
}
