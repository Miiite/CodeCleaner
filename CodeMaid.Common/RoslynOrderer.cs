using System;
using Microsoft.CodeAnalysis;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CodeMaid.Common
{
    public class RoslynOrderer : ICleanerStep, ISyntaxNodeCleanerStep
    {
        public SyntaxNode Run(SyntaxNode classNode) => this.OrderNode(classNode);

        private SyntaxNode OrderNode(SyntaxNode classNode)
        {
            var result = this.OrderMethods(classNode);

            return result;
        }

        private SyntaxNode OrderMethods(SyntaxNode classNode)
        {
            var methods = classNode.DescendantNodes()
                .OfType<MethodDeclarationSyntax>();

            classNode = classNode.TrackNodes(methods);

            var orderedMethods = methods.OrderBy(m => m.Identifier)
                .ToList();

            foreach (var m in orderedMethods)
            {
                classNode = classNode.RemoveNode(m, SyntaxRemoveOptions.KeepNoTrivia);
            }

            var classRoot = classNode.ChildNodes().First();
            classNode = classNode.InsertNodesAfter(classRoot, orderedMethods);

            return classNode;
        }
    }
}
