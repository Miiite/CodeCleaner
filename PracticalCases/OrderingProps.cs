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
        private int c;

        private int PropC => c;

        internal string PropB
        {
            get
            {
                return b;
            }
        }

        public object PropA
        {
            get;
            set;
        }
    }
}