using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Diagnostics.SymbolStore;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Editing;

namespace CodeMaid.Common
{
    public class Privater : ICleanerStep, ISyntaxNodeCleanerStep
    {
        public Privater()
        {
        }

        public SyntaxNode Run(SyntaxNode classNode) => this.MakePrivatesVisible(classNode);

        public SyntaxNode MakePrivatesVisible(SyntaxNode classNode)
        {
            var newClassNode = classNode;

            var fields = newClassNode.DescendantNodes()
                .Where(node => node is FieldDeclarationSyntax fds &&
                               !fds.Modifiers.Any(m => HasExplicitAccessibility(m)))
                .Cast<FieldDeclarationSyntax>()
                .ToList();
            newClassNode = newClassNode.TrackNodes(fields);

            // Make variables, const, static, private
            foreach (var field in fields)
            {
                newClassNode = newClassNode.ReplaceNode(newClassNode.GetCurrentNode(field), field.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));
            }

            var methods = newClassNode.DescendantNodes()
                .Where(node => node is MethodDeclarationSyntax mds &&
                               !mds.Modifiers.Any(m => HasExplicitAccessibility(m)))
                .Cast<MethodDeclarationSyntax>()
                .ToList();
            newClassNode = newClassNode.TrackNodes(methods);

            // Make methods private
            foreach (var method in methods)
            {
                newClassNode = newClassNode.ReplaceNode(
                    newClassNode.GetCurrentNode(method),
                    method.AddModifiers(SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));
            }

            return newClassNode;
        }

        private static bool HasExplicitAccessibility(SyntaxToken m)
        {
            return m.Kind() == SyntaxKind.PrivateKeyword ||
                   m.Kind() == SyntaxKind.PublicKeyword ||
                   m.Kind() == SyntaxKind.ProtectedKeyword ||
                   m.Kind() == SyntaxKind.InternalKeyword;
        }
    }
}
