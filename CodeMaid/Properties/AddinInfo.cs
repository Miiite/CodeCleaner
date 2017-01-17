using System;
using Mono.Addins;
using Mono.Addins.Description;

[assembly: Addin(
    "CodeCleaner",
    Namespace = "CodeCleaner",
    Version = "1.0"
)]

[assembly: AddinName("CodeCleaner")]
[assembly: AddinCategory("IDE extensions")]
[assembly: AddinDescription("CodeCleaner")]
[assembly: AddinAuthor("Adrien")]

[assembly: AddinDependency("::MonoDevelop.Core", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Ide", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.Refactoring", MonoDevelop.BuildInfo.Version)]
[assembly: AddinDependency("::MonoDevelop.SourceEditor2", MonoDevelop.BuildInfo.Version)]
