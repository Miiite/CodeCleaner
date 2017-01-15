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

        public IList<ISymbol> OrderConstants(IEnumerable<ISymbol> symbols)
        {
            var constants = symbols.Where(m => m is IFieldSymbol &&
                                          ((IFieldSymbol)m).IsConst);
            return OrderSymbols(constants);
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
