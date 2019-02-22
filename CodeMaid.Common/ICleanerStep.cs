using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
namespace CodeMaid.Common
{
    public interface ICleanerStep
    {
        IEnumerable<ISymbol> Run(IEnumerable<ISymbol> symbols);
    }
}
