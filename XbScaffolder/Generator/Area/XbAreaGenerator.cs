// from: https://github.com/aspnet/Scaffolding/blob/1.1.3/src/Microsoft.VisualStudio.Web.CodeGenerators.Mvc/Areas/AreaGenerator.cs

using System;
using System.Reflection;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;

namespace XbScaffolder.Generator.Area
{
    [Alias("xbarea")]
    public class XbAreaGenerator : ICodeGenerator
    {
        private static readonly string[] AreaFolders = new string[]
        {
            "Controllers",
            "Models",
            "Views"
        };

        private static readonly string[] ModelSubFolders = new string[]
        {
            "ViewModels",
        };

        private IServiceProvider _serviceProvider { get; set; }
        private IApplicationInfo _appInfo { get; set; }
        private ILogger _logger { get; set; }
        private IModelTypesLocator _modelTypesLocator { get; set; }

        public XbAreaGenerator(
            IApplicationInfo applicationInfo,
            IServiceProvider serviceProvider,
            IModelTypesLocator modelTypesLocator,
            ILogger logger
        )
        {
            if(serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if(applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            if(logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            if(modelTypesLocator == null)
            {
                throw new ArgumentNullException(nameof(modelTypesLocator));
            }

            this._serviceProvider = serviceProvider;
            this._logger = logger;
            this._appInfo = applicationInfo;
            this._modelTypesLocator = modelTypesLocator;
        }

        public async Task GenerateCode(XbAreaGeneratorModel genModel)
        {
            // When multiple this NuGet packages are installed,
            // depending on the situation, older versions may be used.
            var version = this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
            this._logger.LogMessage($"Generator version: {version}");
            this._logger.LogMessage($"Area name: {genModel.AreaName}");

            if (genModel == null)
            {
                throw new ArgumentNullException(nameof(genModel));
            }

            if (string.IsNullOrEmpty(genModel.AreaName))
            {
                throw new ArgumentException(MessageStrings.AreaNameRequired);
            }

            this.EnsureFolderLayout(genModel);

            var readmeGenerator = ActivatorUtilities.CreateInstance<ReadMeGenerator>(this._serviceProvider);
            try
            {
                await readmeGenerator.GenerateReadmeForArea();
            }
            catch (Exception ex)
            {
                _logger.LogMessage($"Failed to generate a readme: {ex.Message}");
                throw ex.Unwrap(_logger);
            }
        }

        /// <summary>
        /// Creates a folder hierarchy:
        ///     ProjectDir
        ///        \ Areas
        ///            \ AreaName
        ///                \ Controllers
        ///                \ Models
        ///                    \ ViewModels
        ///                \ Views
        /// </summary>
        private void EnsureFolderLayout(XbAreaGeneratorModel model)
        {
            var areaBasePath = Path.Combine(_appInfo.ApplicationBasePath, "Areas");
            if (!Directory.Exists(areaBasePath))
            {
                Directory.CreateDirectory(areaBasePath);
            }

            var areaPath = Path.Combine(areaBasePath, model.AreaName);
            if (!Directory.Exists(areaPath))
            {
                Directory.CreateDirectory(areaPath);
            }

            foreach (var areaFolder in AreaFolders)
            {
                var path = Path.Combine(areaPath, areaFolder);
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }

                if (areaFolder == "Models")
                {
                    foreach (var subFolder in ModelSubFolders)
                    {
                        var subPath = Path.Combine(areaPath, areaFolder, subFolder);
                        if (!Directory.Exists(subPath))
                        {
                            Directory.CreateDirectory(subPath);
                        }
                    }
                }
            }
        }
    }
}
