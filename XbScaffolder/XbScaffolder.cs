using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Controller;
using ControllerWithContextGenerator = XbScaffolder.Controller.ControllerWithContextGenerator;
using XbScaffolder.Model;

namespace XbScaffolder
{
    [Alias("xb")]
    public class XbScaffolder : ICodeGenerator
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        public XbScaffolder(
            IServiceProvider serviceProvider,
            ILogger logger
        )
        {
            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._serviceProvider = serviceProvider;
            this._logger = logger;
        }

        /// <summary>
        /// Exec code generation
        /// </summary>
        /// <param name="genModel"></param>
        /// <returns></returns>
        public async Task GenerateCode(XbGeneratorModel genModel)
        {
            var version = this.GetType().GetTypeInfo().Assembly.GetName().Version.ToString();
            this._logger.LogMessage($"version: {version}");

            this._logger.LogMessage($"Model name: {genModel.Model}");
            this._logger.LogMessage($"DbContext name: {genModel.DataContextClass}");
            this._logger.LogMessage($"Controller name: {genModel.ControllerName}");

            var controlName = "";
            var baseName = "";

            if (!string.IsNullOrEmpty(genModel.ControllerName)
                && (Xb.Str.Right(genModel.ControllerName, 10).ToLower() == "controller"))
            {
                controlName = genModel.ControllerName;
                baseName = Xb.Str.SliceReverse(genModel.ControllerName, -10);
            }
            else
            {
                controlName = genModel.ControllerName + "Controller";
                baseName = genModel.ControllerName;
            }

            try
            {
                var cgModel = new CommandLineGeneratorModel();
                cgModel.ModelClass = genModel.Model;
                cgModel.DataContextClass = genModel.DataContextClass;
                cgModel.ControllerName = controlName;
                cgModel.RelativeFolderPath = ".\\Controllers";

                // Generate Controller & 3-Views(Index, Details, Edit)
                var generator = ActivatorUtilities.CreateInstance<ControllerWithContextGenerator>(this._serviceProvider);
                await generator.Generate(cgModel);
            }
            catch (Exception ex)
            {
                this._logger.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }

            // Generate Store class
            try
            {
                var mgModel = new ModelGeneratorModel();
                mgModel.ModelClass = genModel.Model;
                mgModel.DataContextClass = genModel.DataContextClass;
                mgModel.ViewName = baseName;
                mgModel.TemplateName = "Empty";
                mgModel.RelativeFolderPath = ".\\Models\\Stores";

                var scaffolder = ActivatorUtilities.CreateInstance<ModelScaffolder>(this._serviceProvider);
                await scaffolder.GenerateCode(mgModel);
            }
            catch (Exception ex)
            {
                this._logger.LogMessage(Xb.Util.GetErrorHighlighted(ex));
                throw ex;
            }

            await Task.CompletedTask;
        }

        ///// <summary>
        ///// Exec code generation
        ///// </summary>
        ///// <param name="generatorModel"></param>
        ///// <returns></returns>
        //public async Task GenerateCode(XbGeneratorModel generatorModel)
        //{
        //    // View生成テスト
        //    if (generatorModel == null)
        //    {
        //        throw new ArgumentNullException(nameof(generatorModel));
        //    }

        //    if (string.IsNullOrEmpty(generatorModel.Model))
        //    {
        //        throw new ArgumentException("model not setted");
        //    }

        //    this._logger.LogMessage($"Model name: {generatorModel.Model}");

        //    // Viewの生成
        //    var vgModel = new ViewGeneratorModel();
        //    vgModel.ViewName = "test";
        //    vgModel.TemplateName = "Empty";
        //    vgModel.RelativeFolderPath = "Views/Hello";

        //    // 下記二つをセットすると、恐らく ModelBasedViewScaffolder を生成できると思う。
        //    //vgModel.DataContextClass
        //    //vgModel.ModelClass

        //    var scaffolder = ActivatorUtilities.CreateInstance<EmptyViewScaffolder>(this._serviceProvider);

        //    await scaffolder.GenerateCode(vgModel);

        //    await Task.CompletedTask;
        //}
    }
}