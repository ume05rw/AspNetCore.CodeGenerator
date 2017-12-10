// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

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
    public class ModelScaffolder : CommonGeneratorBase
    {
        protected readonly IProjectContext _projectContext;
        protected readonly ICodeGeneratorActionsService _codeGeneratorActionsService;
        protected readonly IServiceProvider _serviceProvider;
        protected readonly ILogger _logger;

        public ModelScaffolder(
            IProjectContext projectDependencyProvider,
            IApplicationInfo applicationInfo,
            ICodeGeneratorActionsService codeGeneratorActionsService,
            IServiceProvider serviceProvider,
            ILogger logger)
            : base(applicationInfo)
        {
            if (projectDependencyProvider == null)
            {
                throw new ArgumentNullException(nameof(projectDependencyProvider));
            }

            if (applicationInfo == null)
            {
                throw new ArgumentNullException(nameof(applicationInfo));
            }

            if (codeGeneratorActionsService == null)
            {
                throw new ArgumentNullException(nameof(codeGeneratorActionsService));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            _projectContext = projectDependencyProvider;
            _codeGeneratorActionsService = codeGeneratorActionsService;
            _serviceProvider = serviceProvider;
            _logger = logger;
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

            if (string.IsNullOrEmpty(generatorModel.ViewName))
            {
                throw new ArgumentException(MessageStrings.ViewNameRequired);
            }

            if (string.IsNullOrEmpty(generatorModel.TemplateName))
            {
                throw new ArgumentException(MessageStrings.TemplateNameRequired);
            }

            var outputPath = ValidateAndGetOutputPath(
                generatorModel, 
                outputFileName: generatorModel.ViewName + Constants.CodeFileExtension
            );
            var layoutDependencyInstaller 
                = ActivatorUtilities.CreateInstance<MvcLayoutDependencyInstaller>(_serviceProvider);
            await layoutDependencyInstaller.Execute();

            await GenerateModel(generatorModel, null, outputPath);
            await layoutDependencyInstaller.InstallDependencies();
        }

        internal async Task GenerateModel(
            ModelGeneratorModel generatorModel, 
            ModelTypeAndContextModel modelTypeAndContextModel, 
            string outputPath)
        {
            var templateModel = GetModelGeneratorTemplateModel(
                generatorModel, 
                modelTypeAndContextModel
            );
            var templateName = generatorModel.TemplateName + Constants.RazorTemplateExtension;
            await _codeGeneratorActionsService.AddFileFromTemplateAsync(
                outputPath, 
                templateName, 
                TemplateFolders, 
                templateModel
            );
            _logger.LogMessage("Added Model : " 
                               + outputPath.Substring(ApplicationInfo.ApplicationBasePath.Length));

            await AddRequiredFiles(generatorModel);
        }

        protected ViewGeneratorTemplateModel GetModelGeneratorTemplateModel(
            ModelGeneratorModel generatorModel, 
            ModelTypeAndContextModel modelTypeAndContextModel)
        {
            bool isLayoutSelected = (generatorModel.UseDefaultLayout 
                                     || !String.IsNullOrEmpty(generatorModel.LayoutPage));

            ViewGeneratorTemplateModel templateModel = new ViewGeneratorTemplateModel()
            {
                ViewDataTypeName = modelTypeAndContextModel?.ModelType?.FullName,
                ViewDataTypeShortName = modelTypeAndContextModel?.ModelType?.Name,
                ViewName = generatorModel.ViewName,
                LayoutPageFile = generatorModel.LayoutPage,
                IsLayoutPageSelected = isLayoutSelected,
                IsPartialView = false,
                ReferenceScriptLibraries = generatorModel.ReferenceScriptLibraries,
                ModelMetadata = modelTypeAndContextModel?.ContextProcessingResult?.ModelMetadata,
                JQueryVersion = "1.10.2" //Todo
            };

            return templateModel;
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
