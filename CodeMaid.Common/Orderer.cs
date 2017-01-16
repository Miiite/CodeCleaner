using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeCleaner.Common.Ordering
{
    public class Orderer
    {
        public Orderer()
        {
        }

        public IList<ISymbol> OrderAll(IEnumerable<ISymbol> symbols)
        {
            var result = new List<ISymbol>();

            result.AddRange(this.OrderStaticMembers(symbols));
            result.AddRange(this.OrderConstants(symbols));
            //result.AddRange(this.OrderInstanceVariables(symbols));
            //result.AddRange(this.OrderConstructor(symbols));
            result.AddRange(this.OrderProperties(symbols));
            result.AddRange(this.OrderMethods(symbols));

            return result;
        }

        public IList<ISymbol> OrderInstanceVariables(IEnumerable<ISymbol> symbols)
        {
            var fields = symbols.Where(s => s is IFieldSymbol);

            return OrderSymbols(fields);
        }

        public IList<ISymbol> OrderConstants(IEnumerable<ISymbol> symbols)
        {
            var constants = symbols.Where(m => m is IFieldSymbol &&
                                          ((IFieldSymbol)m).IsConst);
            return OrderSymbols(constants);
        }

        public IList<ISymbol> OrderStaticMembers(IEnumerable<ISymbol> symbols)
        {
            List<ISymbol> result = new List<ISymbol>();
            var staticFields = symbols.Where(m => m is IFieldSymbol &&
                                             ((IFieldSymbol)m).IsDefinition &&
                                             ((IFieldSymbol)m).IsStatic &&
                                            !((IFieldSymbol)m).IsConst);

            if (staticFields.Any())
            {
                result.AddRange(OrderSymbols(staticFields));
            }

            var staticMethods = symbols.Where(m => m is IMethodSymbol &&
                                              ((IMethodSymbol)m).IsStatic);

            if (staticMethods.Any())
            {
                result.AddRange(OrderSymbols(staticMethods));
            }

            return result;
        }

        public IList<ISymbol> OrderMethods(IEnumerable<ISymbol> symbols)
        {
            var methods = symbols.Where(m => m is IMethodSymbol &&
                                        ((IMethodSymbol)m).MethodKind == MethodKind.Ordinary);

            return OrderSymbols(methods);
        }

        public IList<ISymbol> OrderProperties(IEnumerable<ISymbol> symbols)
        {
            var methods = symbols.Where(m => m is IPropertySymbol);

            return OrderSymbols(methods);
        }

        private IList<ISymbol> OrderSymbols(IEnumerable<ISymbol> constants)
        {
            List<ISymbol> result = new List<ISymbol>();

            result.AddRange(constants.Where(c => c.DeclaredAccessibility == Accessibility.Public)
                            .OrderBy(c => c.Name));
            result.AddRange(constants.Where(c => c.DeclaredAccessibility == Accessibility.Internal)
                            .OrderBy(c => c.Name));
            result.AddRange(constants.Where(c => c.DeclaredAccessibility == Accessibility.Protected)
                            .OrderBy(c => c.Name));
            result.AddRange(constants.Where(c => c.DeclaredAccessibility == Accessibility.Private)
                            .OrderBy(c => c.Name));

            return result;
        }
    }
}
