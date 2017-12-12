

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ProjectModel;
using Microsoft.VisualStudio.Web.CodeGeneration.DotNet;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Dependency;

namespace XbScaffolder.Model
{
    public class ModelGenerator : CommonGeneratorBase
    {
        protected readonly IProjectContext _projectContext;
        protected readonly ICodeGeneratorActionsService _codeGeneratorActionsService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public ModelGenerator(
            IProjectContext projectDependencyProvider,
            IApplicationInfo applicationInfo,
            ICodeGeneratorActionsService codeGeneratorActionsService,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(applicationInfo)
        {
            this._projectContext = projectDependencyProvider
                ?? throw new ArgumentNullException(nameof(projectDependencyProvider));
            this._codeGeneratorActionsService = codeGeneratorActionsService
                ?? throw new ArgumentNullException(nameof(codeGeneratorActionsService));
            this._serviceProvider = serviceProvider
                ?? throw new ArgumentNullException(nameof(serviceProvider));
            this._logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public virtual IEnumerable<string> TemplateFolders
        {
            get
            {
                return TemplateFoldersUtilities.GetTemplateFolders(
                    containingProject: Constants.ThisAssemblyName,
                    applicationBasePath: ApplicationInfo.ApplicationBasePath,
                    baseFolders: new[] { "ModelGenerator" },
                    projectContext: _projectContext);
            }
        }

        public async Task GenerateCode(ModelGeneratorModel generatorModel)
        {
            if (generatorModel == null)
            {
                throw new ArgumentNullException(nameof(generatorModel));
            }

            if (string.IsNullOrEmpty(generatorModel.ModelName))
            {
                throw new ArgumentException(MessageStrings.ViewNameRequired);
            }

            if (string.IsNullOrEmpty(generatorModel.TemplateName))
            {
                throw new ArgumentException(MessageStrings.TemplateNameRequired);
            }

            var outputPath = ValidateAndGetOutputPath(
                generatorModel, 
                outputFileName: generatorModel.ModelName + Constants.CodeFileExtension
            );

            this._logger.LogMessage("ApplicationBasePath: " + this.ApplicationInfo.ApplicationBasePath);
            this._logger.LogMessage("Model Output Path: " + outputPath);


            var layoutDependencyInstaller 
                = ActivatorUtilities.CreateInstance<MvcLayoutDependencyInstaller>(this._serviceProvider);

            await layoutDependencyInstaller.Execute();

            await this.GenerateModel(generatorModel, null, outputPath);

            await layoutDependencyInstaller.InstallDependencies();
        }

        internal async Task GenerateModel(
            ModelGeneratorModel generatorModel, 
            ModelTypeAndContextModel modelTypeAndContextModel, 
            string outputPath)
        {
            var templateName = generatorModel.TemplateName + Constants.RazorTemplateExtension;
            await _codeGeneratorActionsService.AddFileFromTemplateAsync(
                outputPath, 
                templateName, 
                TemplateFolders,
                generatorModel
            );
            this._logger.LogMessage("Added Model : " 
                                    + outputPath.Substring(ApplicationInfo.ApplicationBasePath.Length));

            await AddRequiredFiles(generatorModel);
        }

        protected async Task AddRequiredFiles(ModelGeneratorModel generatorModel)
        {
            IEnumerable<RequiredFileEntity> requiredFiles = GetRequiredFiles(generatorModel);
            foreach (var file in requiredFiles)
            {
                if (!File.Exists(Path.Combine(ApplicationInfo.ApplicationBasePath, file.OutputPath)))
                {
                    await _codeGeneratorActionsService.AddFileAsync(
                        Path.Combine(ApplicationInfo.ApplicationBasePath, file.OutputPath),
                        Path.Combine(TemplateFolders.First(), file.TemplateName));
                    _logger.LogMessage($"Added additional file :{file.OutputPath}");
                }
            }
        }

        protected IEnumerable<RequiredFileEntity> GetRequiredFiles(ModelGeneratorModel viewGeneratorModel)
        {
            return new List<RequiredFileEntity>();
        }
    }
}
