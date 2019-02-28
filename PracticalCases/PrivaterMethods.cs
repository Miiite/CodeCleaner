using NUnit.Framework;
using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Collections.Immutable;
using CodeCleaner.Common.Ordering;

namespace CodeCleaner.Tests
{
    public class Test
    {
        private bool isExplicit;
        bool isImplicit;

        private void IsDeclaredPrivate()
        {
            var tree = "private";
        }

        void IsImplicitelyPrivate()
        {
            var tree = "implicit private";
        }
    }
}