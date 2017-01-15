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
                var body = await document.GetSemanticModelAsync();
                var members = body.Compilation
                                  .GlobalNamespace
                                  .GetNamespaceMembers()
                                  .ToList();
                var currentNamespace = members.Where(m => m.IsDefinition)
                                              .FirstOrDefault();

                var classes = currentNamespace.GetTypeMembers();
                var editor = await DocumentEditor.CreateAsync(document);

                foreach (var c in classes)
                {
                    var classMembers = c.GetMembers();

                    this.HandleConstants(editor, classMembers);

                    //var cMembers = classMembers
                    //    .OrderBy(m => m.Kind)
                    //    .Where(m => m.Kind == Microsoft.CodeAnalysis.SymbolKind.Method &&
                    //           m.Kind != Microsoft.CodeAnalysis.SymbolKind.Property &&
                    //           !m.IsStatic)
                    //    .OrderBy(m => m.DeclaredAccessibility);


                    //var membersToAdd = cMembers.Where(m => m.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Public)
                    //                           .ToList();
                    //membersToAdd.AddRange(cMembers.Where(m => m.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Protected));
                    //membersToAdd.AddRange(cMembers.Where(m => m.DeclaredAccessibility == Microsoft.CodeAnalysis.Accessibility.Private));

                    //var methods = membersToAdd.Select(m => m.DeclaringSyntaxReferences.FirstOrDefault()?.GetSyntax())
                    //                .Where(m => m != null)
                    //                .ToList();

                    //// Delete previous nodes
                    //foreach (var method in methods)
                    //{
                    //    editor.RemoveNode(method);
                    //}

                    //SyntaxNode classSyntaxNode = c.DeclaringSyntaxReferences
                    //                       .First()
                    //                       .GetSyntax();

                    //// Add the nodes ordered properly
                    //editor.InsertMembers(classSyntaxNode,
                    //                     0,
                    //                     methods);

                }

                await this.SaveDocument(editor.GetChangedDocument());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void HandleConstants(DocumentEditor editor, ImmutableArray<ISymbol> classMembers)
        {
            var orderer = new Orderer();
            var orderedMembers = orderer.OrderConstants(classMembers);
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