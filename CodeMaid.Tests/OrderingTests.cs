using NUnit.Framework;
using System;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis;
using System.Linq;
using System.Collections.Immutable;
using CodeCleaner.Common.Ordering;

namespace CodeCleaner.Tests
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void OrderConstantsTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace testnm
{
class TestClass 
{
	protected const int Protected = 0;
	private const int Private = 1;
	public const int Public = 2;    
}
}");

            var orderer = new Orderer();
            var ordered = orderer.OrderConstants(this.GetMembersFromTree(tree));

            Assert.AreEqual("Public", ordered[0].Name);
            Assert.AreEqual("Protected", ordered[1].Name);
            Assert.AreEqual("Private", ordered[2].Name);
        }

        private ImmutableArray<ISymbol> GetMembersFromTree(SyntaxTree csharpCode)
        {
            var Mscorlib = MetadataReference.CreateFromFile(typeof(object).Assembly.Location);
            var compilation = CSharpCompilation.Create("MyCompilation",
                syntaxTrees: new[] { csharpCode }, references: new[] { Mscorlib });
            var body = compilation.GetSemanticModel(csharpCode);
            var members = body.Compilation
                                  .GlobalNamespace
                                  .GetNamespaceMembers()
                                  .ToList();
            var currentNamespace = members.Where(m => m.IsDefinition)
                                          .FirstOrDefault();
            var classes = currentNamespace.GetTypeMembers();
            var classMembers = classes.First().GetMembers();

            return classMembers;
        }
    }
}
