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

        public void OrderConstantsTest()
        {
            var tree = "mytree";
        }

        internal void IsInternal()
        {
            var i = "internal";
        }

        protected void IsProtected()
        {
            var p = "isProtected";
        }
        void AIsPrivate2()
        {
            var tree = "private2";
        }

        private void IsPrivate()
        {
            var tree = "private";
        }
    }
}