using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Diagnostics.SymbolStore;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeMaid.Common
{
    public class Privater : ICleanerStep, ISyntaxNodeCleanerStep
    {
        public Privater()
        {
        }

        public SyntaxNode MakePrivatesVisible(IEnumerable<ISymbol> symbols, SyntaxNode classNode)
        {
            var newClassNode = classNode;

            foreach (var symbol in symbols)
            {
                if (symbol.DeclaredAccessibility == Accessibility.Private)
                {
                    var syntax = symbol.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax();
                    var symbolToken = syntax as MethodDeclarationSyntax;

                    if (symbolToken?.Modifiers != null &&
                        !symbolToken.Modifiers.Any(m => m.Kind() == SyntaxKind.PrivateKeyword))
                    {
                        var newMethodToken = symbolToken.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword));

                        newClassNode = newClassNode.ReplaceNode(symbolToken, newMethodToken);
                    }
                }
            }

            return newClassNode;
        }

        public SyntaxNode Run(IEnumerable<ISymbol> symbols, SyntaxNode classNode) => this.MakePrivatesVisible(symbols, classNode);
    }
}
