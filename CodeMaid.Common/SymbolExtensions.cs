using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
namespace CodeMaid.Common
{
    public static class SymbolExtensions
    {
        public static IEnumerable<ISymbol> GetConstructors(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(s => s is IMethodSymbol &&
                     ((IMethodSymbol)s).MethodKind == MethodKind.Constructor &&
                     !((IMethodSymbol)s).IsStatic &&
                    !((IMethodSymbol)s).IsImplicitlyDeclared);
        }

        public static IEnumerable<ISymbol> GetMethods(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(m => m is IMethodSymbol &&
                    ((IMethodSymbol)m).MethodKind == MethodKind.Ordinary &&
                    !((IMethodSymbol)m).IsStatic);
        }

        public static IEnumerable<ISymbol> GetProperties(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(m => m is IPropertySymbol);
        }

        public static IEnumerable<ISymbol> GetStaticFields(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(m => m is IFieldSymbol &&
                         ((IFieldSymbol)m).IsDefinition &&
                         ((IFieldSymbol)m).IsStatic &&
                        !((IFieldSymbol)m).IsConst);
        }

        public static IEnumerable<ISymbol> GetStaticMethods(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(m => m is IMethodSymbol &&
                      ((IMethodSymbol)m).IsStatic &&
                     ((IMethodSymbol)m).MethodKind == MethodKind.Ordinary);
        }

        public static IEnumerable<ISymbol> GetConstants(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(m => m is IFieldSymbol &&
                          ((IFieldSymbol)m).IsConst);
        }

        public static IEnumerable<ISymbol> GetInstanceVariables(this IEnumerable<ISymbol> symbols)
        {
            return symbols.Where(s => s is IFieldSymbol &&
                      !((IFieldSymbol)s).IsStatic &&
                      !((IFieldSymbol)s).IsImplicitlyDeclared);

        }
    }
}
