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
using CodeMaid.Common;

namespace CodeCleaner
{
    public class RefactorCommand : CommandHandler
    {
        private static ICleanerStep[] Steps = new ICleanerStep[] { new Orderer(), new Privater() };

        protected override async void Run()
        {
            foreach (var step in Steps)
            {
                var document = IdeApp.Workbench.ActiveDocument.AnalysisDocument;
                var body = await document.GetSemanticModelAsync();
                var members = body.Compilation
                                  .GlobalNamespace
                                  .GetNamespaceMembers()
                                  .ToList();
                var currentNamespace = members.Where(m => m.IsDefinition)
                                              .FirstOrDefault();

                var classes = currentNamespace.GetAllTypes().ToList();
                var editor = await DocumentEditor.CreateAsync(document);

                foreach (var c in classes)
                {
                    var classMembers = c.GetMembers();
                    SyntaxNode classSyntaxNode = c.DeclaringSyntaxReferences
                                           .First()
                                           .GetSyntax();

                    if (step is ISymbolCleanerStep)
                    {
                        this.HandleStep(((ISymbolCleanerStep)step), editor, classSyntaxNode, classMembers);
                    }
                    else if (step is ISyntaxNodeCleanerStep)
                    {
                        editor.ReplaceNode(classSyntaxNode, ((ISyntaxNodeCleanerStep)step).Run(classMembers, classSyntaxNode));
                    }
                }

                var newDocument = editor.GetChangedDocument();
                var formatedDocument = await Formatter.FormatAsync(newDocument);
                await this.SaveDocument(formatedDocument);
            }
        }

        private void HandleStep(ISymbolCleanerStep cleanerStep, DocumentEditor editor, SyntaxNode classNode, ImmutableArray<ISymbol> classMembers)
        {
            var cleanedMembers = cleanerStep.Run(classMembers);

            var nodes = cleanedMembers.Where(o => o.DeclaringSyntaxReferences.Any())
                                      .Select(o => o.DeclaringSyntaxReferences.FirstOrDefault().GetSyntax())
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