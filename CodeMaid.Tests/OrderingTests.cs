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
	protected const int A = 2;
	protected const int Protected2 = 1;
	protected const int Protected = 0;
	private const int Private = 1;
	public const int Public = 2;    
}
}");

            var orderer = new Orderer();
            var ordered = orderer.OrderConstants(this.GetMembersFromTree(tree));

            Assert.AreEqual("Public", ordered[0].Name);
            Assert.AreEqual("A", ordered[1].Name);
            Assert.AreEqual("Protected", ordered[2].Name);
            Assert.AreEqual("Protected2", ordered[3].Name);
            Assert.AreEqual("Private", ordered[4].Name);
        }

        [Test]
        public void OrderMethodsTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
	class testclass{
	
private int A(){}
private int PrivateMethod(){}
public string Z(){}
public string PublicMethod(){}
protected object ProtectedMethod(){}

}
}            
");

            var orderer = new Orderer();
            var ordered = orderer.OrderMethods(this.GetMembersFromTree(tree));

            Assert.AreEqual("PublicMethod", ordered[0].Name);
            Assert.AreEqual("Z", ordered[1].Name);
            Assert.AreEqual("ProtectedMethod", ordered[2].Name);
            Assert.AreEqual("A", ordered[3].Name);
            Assert.AreEqual("PrivateMethod", ordered[4].Name);
        }

        [Test]
        public void OrderPropertiesTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private int D {get; private set;}
private int C {get;set;}
public string A{get;private set;}
protected object B{get;}

}
}            
");

            var orderer = new Orderer();
            var ordered = orderer.OrderProperties(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
            Assert.AreEqual("C", ordered[2].Name);
            Assert.AreEqual("D", ordered[3].Name);
        }

        [Test]
        public void OrderInstanceVariablesTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private int C = 2;
string B = 1;
public object A;
}
}            
");

            var orderer = new Orderer();
            var ordered = orderer.OrderInstanceVariables(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
            Assert.AreEqual("C", ordered[2].Name);

        }

        [Test]
        public void OrderAllTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private void C(){}
protected const int A = 0;
protected object B{get;}
}
}            	

");

            var orderer = new Orderer();
            var ordered = orderer.OrderAll(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
            Assert.AreEqual("C", ordered[2].Name);
        }

        [Test]
        public void OrderAllTest2()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
   using System;
namespace TestRefacto
{
    public class MyClass
    {
        private int E = 0;


        private const int B = 99;
        public const int A = 88;

        public static int C = 1;

        protected object H()
        {
            return null;
        }

        public string F { get; set; }

        private int I()
        {
            int i = 0;

            return i;
        }

        public MyClass()
        {
            string myConstructorBody = null;
        }

        public static void D()
        {

        }
    }
}
         
");

            var orderer = new Orderer();
            var ordered = orderer.OrderAll(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
            Assert.AreEqual("C", ordered[2].Name);
            Assert.AreEqual("D", ordered[3].Name);
            Assert.AreEqual("E", ordered[4].Name);
            Assert.AreEqual("F", ordered[5].Name);
            Assert.AreEqual(".ctor", ordered[6].Name);
            Assert.AreEqual("H", ordered[7].Name);
            Assert.AreEqual("I", ordered[8].Name);
        }

        [Test]
        public void OrderConstructorsTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private void C(){}
protected const int A = 0;
protected object B{get;}

private testclass(){}
private testclass(int i, int j){}
public testclass(){}


}
}               

");

            var orderer = new Orderer();
            var ordered = orderer.OrderConstructors(this.GetMembersFromTree(tree));

            Assert.AreEqual(0, ((IMethodSymbol)ordered[0]).Parameters.Count());
            Assert.AreEqual(Accessibility.Public, ((IMethodSymbol)ordered[0]).DeclaredAccessibility);

            Assert.AreEqual(0, ((IMethodSymbol)ordered[1]).Parameters.Count());
            Assert.AreEqual(Accessibility.Private, ((IMethodSymbol)ordered[1]).DeclaredAccessibility);

            Assert.AreEqual(2, ((IMethodSymbol)ordered[2]).Parameters.Count());
            Assert.AreEqual(Accessibility.Private, ((IMethodSymbol)ordered[2]).DeclaredAccessibility);
        }

        [Test]
        public void OrderStaticTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private void E(){}
protected const int D = 0;
public static int A = 1;
private static void B(){}
protected object C{get;}
}
}               

");

            var orderer = new Orderer();
            var ordered = orderer.OrderStaticMembers(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
        }

        [Test]
        public void OrderVariablesTest()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace test{
    class testclass{
    
private void E(){}
protected const int D = 0;
private int B = 2;
public static int Z = 1;
private static void X(){}
protected object C{get;}
private int A = 1;
}
}               

");

            var orderer = new Orderer();
            var ordered = orderer.OrderInstanceVariables(this.GetMembersFromTree(tree));

            Assert.AreEqual("A", ordered[0].Name);
            Assert.AreEqual("B", ordered[1].Name);
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
