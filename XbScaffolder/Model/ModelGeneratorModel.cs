

using Microsoft.VisualStudio.Web.CodeGeneration;
using Microsoft.VisualStudio.Web.CodeGeneration.CommandLine;
using Microsoft.VisualStudio.Web.CodeGeneration.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc;
using System;
using System.Collections.Generic;
using ClassNameModel = Microsoft.VisualStudio.Web.CodeGenerators.Mvc.ClassNameModel;

namespace XbScaffolder.Model
{
    public class ModelGeneratorModel : CommonCommandLineModel
    {
        public ModelGeneratorModel(
            ModelType modelType,
            string dbContextFullTypeName
        )
        {
            if (modelType == null)
            {
                throw new ArgumentNullException(nameof(modelType));
            }

            if (dbContextFullTypeName == null)
            {
                throw new ArgumentNullException(nameof(dbContextFullTypeName));
            }

            this.ModelType = modelType;

            var classNameModel = new ClassNameModel(dbContextFullTypeName);

            this.DbContextTypeName = classNameModel.ClassName;
            this.DbContextNamespace = classNameModel.NamespaceName;
        }

        /// <summary>
        /// Entity name
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// DbContext.DbSet name
        /// </summary>
        public string DbSetName { get; set; }

        public string TemplateName { get; set; }


        public string AppRootNamespace { get; set; }

        public string AreaRootNamespace { get; set; }

        public string ClassNamespace { get; set; }


        public string DbContextTypeName { get; set; }

        public string DbContextNamespace { get; private set; }

        public IModelMetadata ModelMetadata { get; set; }

        public ModelType ModelType { get; private set; }

        public HashSet<string> RequiredNamespaces
        {
            get
            {
                var requiredNamespaces = new SortedSet<string>(StringComparer.Ordinal);
                // We add ControllerNamespace first to make other entries not added to the set if they match.
                requiredNamespaces.Add(this.ClassNamespace);

                var modelTypeNamespace = this.ModelType.Namespace;

                if (!string.IsNullOrWhiteSpace(modelTypeNamespace))
                {
                    requiredNamespaces.Add(modelTypeNamespace);
                }

                if (!string.IsNullOrWhiteSpace(this.DbContextNamespace))
                {
                    requiredNamespaces.Add(this.DbContextNamespace);
                }

                foreach (var item in new[] { "DbContexts", "Entities", "Stores", "ViewModels" })
                {
                    requiredNamespaces.Add($"{this.AppRootNamespace}.Models.{item}");
                }

                // Finally we remove the ControllerNamespace as it's not required.
                requiredNamespaces.Remove(this.ClassNamespace);
                return new HashSet<string>(requiredNamespaces);
            }
        }
    }
}