using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace CodeCleaner.Common.Ordering
{
    public class Orderer
    {
        public class LastSymbols
        {
            public SyntaxNode LastConstant { get; set; }
            public SyntaxNode LastStatic { get; set; }
            public SyntaxNode LastVariable { get; set; }
            public SyntaxNode LastProperty { get; set; }
            public SyntaxNode LastConstructor { get; set; }
            public SyntaxNode LastMethod { get; set; }
        }

        public static LastSymbols Symbols { get; set; } = new LastSymbols();

        public Orderer()
        {
        }

        public IList<ISymbol> OrderAll(IEnumerable<ISymbol> symbols)
        {
            var result = new List<ISymbol>();

            this.AddAndSaveLast(result, this.OrderConstants(symbols), (node) => Symbols.LastConstant = node);
            this.AddAndSaveLast(result, this.OrderStaticMembers(symbols), (node) => Symbols.LastStatic = node);
            this.AddAndSaveLast(result, this.OrderInstanceVariables(symbols), (node) => Symbols.LastVariable = node);
            this.AddAndSaveLast(result, this.OrderProperties(symbols), (node) => Symbols.LastProperty = node);
            this.AddAndSaveLast(result, this.OrderConstructors(symbols), (node) => Symbols.LastConstructor = node);
            this.AddAndSaveLast(result, this.OrderMethods(symbols), (node) => Symbols.LastMethod = node);

            return result;
        }

        private IList<ISymbol> AddAndSaveLast(List<ISymbol> sourceList, IEnumerable<ISymbol> symbols, Action<SyntaxNode> saveLastAction)
        {
            saveLastAction(this.GetLastNode(symbols));
            sourceList.AddRange(symbols);

            return sourceList;
        }

        private SyntaxNode GetLastNode(IEnumerable<ISymbol> symbols)
        {
            SyntaxNode result = null;

            if (symbols?.Any() == true)
            {
                result = symbols.Last()
                    .DeclaringSyntaxReferences
                    .First()
                    .GetSyntax();
            }

            return result;
        }

        public IList<ISymbol> OrderConstructors(IEnumerable<ISymbol> symbols)
        {
            var constructors = symbols.Where(s => s is IMethodSymbol &&
                                             ((IMethodSymbol)s).MethodKind == MethodKind.Constructor &&
                                             !((IMethodSymbol)s).IsStatic &&
                                            !((IMethodSymbol)s).IsImplicitlyDeclared)
                                      .ToList();
            var result = new List<ISymbol>();

            result.AddRange(constructors.Where(c => c.DeclaredAccessibility == Accessibility.Public)
                            .OrderBy(c => ((IMethodSymbol)c).Parameters.Count()));
            result.AddRange(constructors.Where(c => c.DeclaredAccessibility == Accessibility.Internal)
                            .OrderBy(c => ((IMethodSymbol)c).Parameters.Count()));
            result.AddRange(constructors.Where(c => c.DeclaredAccessibility == Accessibility.Protected)
                            .OrderBy(c => ((IMethodSymbol)c).Parameters.Count()));
            result.AddRange(constructors.Where(c => c.DeclaredAccessibility == Accessibility.Private)
                            .OrderBy(c => ((IMethodSymbol)c).Parameters.Count()));

            return result;
        }

        public IList<ISymbol> OrderInstanceVariables(IEnumerable<ISymbol> symbols)
        {
            var fields = symbols.Where(s => s is IFieldSymbol &&
                                      !((IFieldSymbol)s).IsStatic &&
                                       !((IFieldSymbol)s).IsImplicitlyDeclared);

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
                                              ((IMethodSymbol)m).IsStatic &&
                                             ((IMethodSymbol)m).MethodKind == MethodKind.Ordinary);

            if (staticMethods.Any())
            {
                result.AddRange(OrderSymbols(staticMethods));
            }

            return result;
        }

        public IList<ISymbol> OrderMethods(IEnumerable<ISymbol> symbols)
        {
            var methods = symbols.Where(m => m is IMethodSymbol &&
                                        ((IMethodSymbol)m).MethodKind == MethodKind.Ordinary &&
                                        !((IMethodSymbol)m).IsStatic);

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
