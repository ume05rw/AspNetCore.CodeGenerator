// from: https://github.com/prafullbhosale/CustomScaffolder/blob/master/src/CustomScaffolder/CustomScaffolder.cs

using System;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using XbScaffolder.Model;
using ControllerWithContextGenerator = XbScaffolder.Controller.ControllerWithContextGenerator;

namespace XbScaffolder.Generator.Code
{
    [Alias("xbcode")]
    public class XbCodeGenerator : ICodeGenerator
    {
        private readonly IApplicationInfo _applicationInfo;
        private readonly IServiceProvider _serviceProvider;
        private readonly IModelTypesLocator _modelTypesLocator;
        private readonly IEntityFrameworkService _entityFrameworkService;
        private readonly ILogger _logger;

        private string _modelName;
        private string _dbContextName;
        private string _controllerName;
        private string _areaName;
        private string _baseName;
        private ModelTypeAndContextModel _modelTypeAndContextModel;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public XbCodeGenerator(
            IApplicationInfo applicationInfo,
            IServiceProvider serviceProvider,
            IModelTypesLocator modelTypesLocator,
            IEntityFrameworkService entityFrameworkService,
            ILogger logger
        )
        {
            try
            {
                this._applicationInfo = applicationInfo
                    ?? throw new ArgumentNullException(nameof(applicationInfo));
                this._serviceProvider = serviceProvider
                    ?? throw new ArgumentNullException(nameof(serviceProvider));
                this._modelTypesLocator = modelTypesLocator
                    ?? throw new ArgumentNullException(nameof(modelTypesLocator));
                this._entityFrameworkService = entityFrameworkService
                    ?? throw new ArgumentNullException(nameof(entityFrameworkService));
                this._logger = logger
                    ?? throw new ArgumentNullException(nameof(logger));
            }
            catch (Exception ex)
            {
                this._logger?.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }
        }

        /// <summary>
        /// Exec code generation
        /// </summary>
        /// <param name="genModel"></param>
        /// <returns></returns>
        public async Task GenerateCode(XbCodeGeneratorModel genModel)
        {
            try
            {
                if (genModel == null)
                {
                    throw new ArgumentNullException(nameof(genModel));
                }

                this._modelName = genModel.ModelName
                    ?? throw new ArgumentException(MessageStrings.ModelNameRequired);
                this._dbContextName = genModel.DbContextName
                    ?? throw new ArgumentException(MessageStrings.DbContextNameRequired);
                this._controllerName = genModel.ControllerName
                    ?? throw new ArgumentException(MessageStrings.ControllerNameRequired);
                this._areaName = genModel.AreaName;

                // When multiple this NuGet packages are installed,
                // depending on the situation, older versions may be used.
                var version = this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
                this._logger.LogMessage($"Generator version: {version}");

                if (!string.IsNullOrEmpty(genModel.ControllerName)
                    && (Xb.Str.Right(genModel.ControllerName, 10).ToLower() == "controller"))
                {
                    this._controllerName = genModel.ControllerName;
                    this._baseName = Xb.Str.SliceReverse(genModel.ControllerName, -10);
                }
                else
                {
                    this._controllerName = genModel.ControllerName + "Controller";
                    this._baseName = genModel.ControllerName;
                }

                var cgModel = new CommandLineGeneratorModel()
                {
                    ModelClass = this._modelName,
                    DataContextClass = this._dbContextName,
                    ControllerName = this._controllerName,
                    RelativeFolderPath = string.IsNullOrEmpty(this._areaName)
                        ? "Controllers"
                        : Path.Combine("Areas", this._areaName, "Controllers")
                };

                this._modelTypeAndContextModel = await ModelMetadataUtilities.ValidateModelAndGetEFMetadata(
                    cgModel,
                    this._entityFrameworkService,
                    this._modelTypesLocator,
                    this._areaName
                );

                this._logger.LogMessage($"Model name: {this._modelName}");
                this._logger.LogMessage($"DbContext name: {this._dbContextName}");
                this._logger.LogMessage($"Controller name: {this._controllerName}"
                    + ((this._controllerName == genModel.ControllerName)
                        ? ""
                        : $" (passing: {genModel.ControllerName})"));
                this._logger.LogMessage($"Area name: {this._areaName}");

                await this.GenerateControllerAndViews(cgModel);

                await this.GenerateStore();

                await this.GenerateViewModel();
            }
            catch (Exception ex)
            {
                this._logger?.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }
        }

        private string GetNamespace(string relativeFolderPath)
        {
            return NameSpaceUtilities.GetSafeNameSpaceFromPath(
                relativeFolderPath, 
                this._applicationInfo.ApplicationName
            );
        }

        private async Task GenerateControllerAndViews(CommandLineGeneratorModel cgModel)
        {
            try
            {
                // Generate Controller & 3-Views(Index, Details, Edit)
                var generator = ActivatorUtilities.CreateInstance<ControllerWithContextGenerator>(
                    this._serviceProvider
                );
                await generator.Generate(cgModel);
            }
            catch (Exception ex)
            {
                this._logger.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }
        }

        private async Task GenerateStore()
        {
            try
            {
                var mgModel = new ModelGeneratorModel(
                    this._modelTypeAndContextModel.ModelType,
                    this._modelTypeAndContextModel.DbContextFullName
                )
                {
                    ModelClass = this._modelName,
                    DataContextClass = this._dbContextName,
                    ModelName = $"{this._modelName}Store",
                    TemplateName = "Store",
                    RelativeFolderPath = @"Models\Stores",
                    DbSetName = this._baseName,
                    ModelMetadata = this._modelTypeAndContextModel.ContextProcessingResult.ModelMetadata,
                    AppRootNamespace = this.GetNamespace(string.Empty),
                    AreaRootNamespace = string.IsNullOrEmpty(this._areaName)
                        ? this.GetNamespace(string.Empty)
                        : this.GetNamespace(Path.Combine("Areas", this._areaName))
                };
                mgModel.ClassNamespace = this.GetNamespace(mgModel.RelativeFolderPath);

                var scaffolder = ActivatorUtilities.CreateInstance<ModelGenerator>(this._serviceProvider);
                await scaffolder.GenerateCode(mgModel);
            }
            catch (Exception ex)
            {
                this._logger.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }
        }

        private async Task GenerateViewModel()
        {
            try
            {
                var mgModel = new ModelGeneratorModel(
                    this._modelTypeAndContextModel.ModelType,
                    this._modelTypeAndContextModel.DbContextFullName
                )
                {
                    ModelClass = this._modelName,
                    DataContextClass = this._dbContextName,
                    ModelName = $"{this._modelName}ViewModel",
                    TemplateName = "ViewModel",
                    RelativeFolderPath = string.IsNullOrEmpty(this._areaName)
                        ? @"Models\ViewModels"
                        : Path.Combine("Areas", this._areaName, "Models", "ViewModels"),
                    DbSetName = this._baseName,
                    ModelMetadata = this._modelTypeAndContextModel.ContextProcessingResult.ModelMetadata,
                    AppRootNamespace = this.GetNamespace(string.Empty),
                    AreaRootNamespace = string.IsNullOrEmpty(this._areaName)
                        ? this.GetNamespace(string.Empty)
                        : this.GetNamespace(Path.Combine("Areas", this._areaName))
                };
                mgModel.ClassNamespace = this.GetNamespace(mgModel.RelativeFolderPath);

                var scaffolder = ActivatorUtilities.CreateInstance<ModelGenerator>(this._serviceProvider);
                await scaffolder.GenerateCode(mgModel);
            }
            catch (Exception ex)
            {
                this._logger.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }
        }
    }
}
