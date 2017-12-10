using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;

namespace XbScaffolder
{
    public class XbGeneratorModel
    {
        [Option(Name="model", ShortName="m")]
        public string Model { get; set; }

        [Option(Name="dataContext", ShortName="dc")]
        public string DataContextClass { get; set; }

        [Option(Name="controllerName", ShortName="name")]
        public string ControllerName { get; set; }
    }
}