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
        int c = 0;
        private object a = null;
        int d;
        string b = "b";

        internal string PropB
        {
            get
            {
                return b;
            }
        }

        private int PropC => c;

        public int PropD
        {
            get => return d;
            set => d = value;
        }

        private static object StaticA = null;
        public static string StaticZ = "";
        static object StaticBB = null;

        const int bconst = 42;

        public int PropA
        {
            get => return a;
            set => a = value;
        }

        public const string a = "const";

        private void IsPrivate()
        {
            var tree = "private";
        }

        void AIsPrivate3()
        {
            var tree = "private3";
        }

        protected void IsProtected()
        {
            var p = "isProtected";
        }

        public void OrderConstantsTest()
        {
            var tree = "mytree";
        }

        void AIsPrivate2()
        {
            var tree = "private2";
        }

        internal void IsInternal()
        {
            var i = "internal";
        }
    }
}