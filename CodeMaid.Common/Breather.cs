using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;

namespace CodeMaid.Common
{
    public class Breather : ICleanerStep, ISyntaxNodeCleanerStep
    {
        public Breather()
        {
        }

        public SyntaxNode Run(SyntaxNode classNode) => this.MakeCodeBreathe(classNode);

        public SyntaxNode MakeCodeBreathe(SyntaxNode classNode)
        {
            SyntaxNode resultNode = classNode;

            var lastVariable = resultNode.DescendantNodes()
              .Where(node => node is FieldDeclarationSyntax fds)
              .Cast<FieldDeclarationSyntax>()
              .LastOrDefault();

            var currentTrivia = lastVariable.GetTrailingTrivia();

            resultNode = classNode.TrackNodes(lastVariable);
            resultNode = resultNode.ReplaceNode(lastVariable, lastVariable.WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed, SyntaxFactory.CarriageReturnLineFeed));

            return resultNode;
        }
    }
}
