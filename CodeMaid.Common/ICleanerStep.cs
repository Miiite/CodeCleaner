using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
namespace CodeMaid.Common
{
    public interface ICleanerStep
    {
    }

    public interface ISymbolCleanerStep
    {
        IEnumerable<ISymbol> Run(IEnumerable<ISymbol> symbols);
    }

    public interface ISyntaxNodeCleanerStep
    {
        SyntaxNode Run(IEnumerable<ISymbol> symbols, SyntaxNode classNode);
    }
}
