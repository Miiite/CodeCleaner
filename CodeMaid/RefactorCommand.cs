using MonoDevelop.Components.Commands;
using MonoDevelop.Ide;
using MonoDevelop.Ide.Gui;
using System;
using System.Text;
using MonoDevelop.Ide.Editor;
using System.Collections.Generic;
using System.Linq;
using ICSharpCode.NRefactory.TypeSystem;
using ICSharpCode.NRefactory.CSharp;
using Mono.Collections.Generic;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using ISymbol = Microsoft.CodeAnalysis.ISymbol;
using SymbolKind = Microsoft.CodeAnalysis.SymbolKind;
using Accessibility = Microsoft.CodeAnalysis.Accessibility;
using CodeCleaner.Common.Ordering;
using System.Diagnostics;
using MonoDevelop.Ide.TypeSystem;
using Microsoft.CodeAnalysis.Formatting;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.Options;

namespace CodeCleaner
{
    public class RefactorCommand : CommandHandler
    {
        //Ordre:

        //Constants
        //Static

        //variables

        //properties

        //constructor

        //methods

        protected override async void Run()
        {
            try
            {
                var document = IdeApp.Workbench.ActiveDocument.AnalysisDocument;
                List<INamedTypeSymbol> classes = await GetDocumentClasses(document);
                var editor = await DocumentEditor.CreateAsync(document);

                Microsoft.CodeAnalysis.Document newDocument = null;
                foreach (var c in classes)
                {
                    var classMembers = c.GetMembers();
                    SyntaxNode classSyntaxNode = c.DeclaringSyntaxReferences
                                           .First()
                                           .GetSyntax();

                    this.HandleOrdering(editor, classSyntaxNode, classMembers);
                }

                newDocument = editor.GetChangedDocument();
                var formatedDocument = await Formatter.FormatAsync(newDocument);

                //var fClasses = await GetDocumentClasses(formatedDocument);

                //var firstClassNode = fClasses.First().DeclaringSyntaxReferences.First().GetSyntax();
                //var lastConstantTrivia = Orderer.Symbols.LastConstant.GetTrailingTrivia();
                //lastConstantTrivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
                //var newFirstClassNode = firstClassNode.ReplaceNode(
                //    Orderer.Symbols.LastConstant,
                //    Orderer.Symbols.LastConstant.WithTrailingTrivia(lastConstantTrivia));

                //var root = await formatedDocument.GetSyntaxRootAsync();
                //var newRoot = root.ReplaceNode(firstClassNode, newFirstClassNode);

                //var yetAnotherDoc = document.WithSyntaxRoot(newRoot);
                var root = await formatedDocument.GetSyntaxRootAsync();

                var trivia = Orderer.Symbols.LastConstant.GetTrailingTrivia();
                trivia.Insert(0, SyntaxFactory.CarriageReturnLineFeed);
                var newLastConstant = Orderer.Symbols.LastConstant.WithTrailingTrivia(trivia);
                var newRoot = root.ReplaceNode(Orderer.Symbols.LastConstant, newLastConstant);

                var yetAnotherDoc = document.WithSyntaxRoot(newRoot);

                await this.SaveDocument(yetAnotherDoc);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private static async Task<List<INamedTypeSymbol>> GetDocumentClasses(Microsoft.CodeAnalysis.Document document)
        {
            var body = await document.GetSemanticModelAsync();
            var members = body.Compilation
                              .GlobalNamespace
                              .GetNamespaceMembers()
                              .ToList();
            var currentNamespace = members.Where(m => m.IsDefinition)
                                          .FirstOrDefault();

            var classes = currentNamespace.GetAllTypes().ToList();
            return classes;
        }

        private void HandleOrdering(DocumentEditor editor, SyntaxNode classNode, ImmutableArray<ISymbol> classMembers)
        {
            var orderer = new Orderer();
            var orderedMembers = orderer.OrderAll(classMembers);
            var nodes = orderedMembers.Select(o => o.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax())
                                      .Where(node => node != null)
                                      .ToList();

            foreach (var member in nodes)
            {
                try
                {
                    editor.RemoveNode(member);

                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.ToString());
                }
            }

            editor.InsertMembers(classNode, 0, nodes);

        }

        private async Task<Microsoft.CodeAnalysis.Document> HandleOrdering2(Microsoft.CodeAnalysis.Document document, SyntaxNode classNode, ImmutableArray<ISymbol> classMembers)
        {
            var initialChildren = classNode.ChildNodes();
            var orderer = new Orderer();
            var orderedMembers = orderer.OrderAll(classMembers);
            var nodes = orderedMembers.Select(o => o.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax())
                                      .Where(node => node != null)
                                      .ToList();

            var editor = await DocumentEditor.CreateAsync(document);

            foreach (var member in initialChildren)
            {
                editor.RemoveNode(member);
            }

            editor.InsertMembers(classNode, 0, nodes);
            var newDocument = editor.GetChangedDocument();

            return newDocument;
        }


        private static Task<Microsoft.CodeAnalysis.Document> GetTransformedDocumentAsync(Microsoft.CodeAnalysis.Document document, SyntaxNode syntaxRoot, SyntaxNode node, SyntaxTriviaList leadingTrivia)
        {
            var newTriviaList = leadingTrivia;
            newTriviaList = newTriviaList.Insert(0, SyntaxFactory.CarriageReturnLineFeed);

            var newNode = node.WithTrailingTrivia(newTriviaList);
            var newSyntaxRoot = syntaxRoot.ReplaceNode(node, newNode);
            var newDocument = document.WithSyntaxRoot(newSyntaxRoot);

            return Task.FromResult(newDocument);
        }


        private async System.Threading.Tasks.Task SaveDocument(Microsoft.CodeAnalysis.Document newDocument)
        {
            IdeApp.Workbench.ActiveDocument.Editor.Text = (await newDocument.GetTextAsync()).ToString();
            //await IdeApp.Workbench.ActiveDocument.Save();
        }

        protected override void Update(CommandInfo info)
        {
            // Enable the command only if the current file in editor is a c# file
            info.Enabled = IdeApp.Workbench.ActiveDocument != null &&
                            IdeApp.Workbench.ActiveDocument.Editor != null &&
                            IdeApp.Workbench.ActiveDocument.Editor.MimeType == "text/x-csharp";
            base.Update(info);
        }
    }
}