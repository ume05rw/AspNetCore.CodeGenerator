// from: https://github.com/prafullbhosale/CustomScaffolder/blob/master/src/CustomScaffolder/SampleGeneratorModel.cs

using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;

namespace XbScaffolder.Generator.Code
{
    public class XbCodeGeneratorModel
    {
        [Option(Name="model", ShortName="m")]
        public string ModelName { get; set; }

        [Option(Name="dbContext", ShortName="dc")]
        public string DbContextName { get; set; }

        [Option(Name="controllerName", ShortName="name")]
        public string ControllerName { get; set; }

        [Option(Name="areaName", ShortName="area")]
        public string AreaName { get; set; }
    }
}