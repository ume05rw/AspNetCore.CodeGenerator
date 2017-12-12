// from: https://github.com/aspnet/Scaffolding/blob/1.1.3/src/Microsoft.VisualStudio.Web.CodeGenerators.Mvc/Areas/AreaGeneratorCommandLine.cs

using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace XbScaffolder.Generator.Area
{
    public class XbAreaGeneratorModel
    {
        [Option(Name = "areaName", ShortName = "name")]
        public string AreaName { get; set; }
    }
}