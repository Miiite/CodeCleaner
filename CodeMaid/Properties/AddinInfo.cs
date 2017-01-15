using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "CodeMaid",
    Namespace = "CodeMaid",
    Version = "1.0"
)]

[assembly: AddinName("CodeMaid")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("CodeMaid")]
[assembly: AddinAuthor("Adrien")]

[assembly: AddinDependency("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Refactoring", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]
