using System;
using System.IO;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TextTemplating;

namespace ZippyWPFForm.T4Engine
{
    [Serializable]
    public class ZippyT4Host : Microsoft.VisualStudio.TextTemplating.ITextTemplatingEngineHost
    {
        /// <summary>
        /// Tag 可能是 Project ，可能是 Table
        /// </summary>
        public object Tag
        {
            get;
            set;
        }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace
        {
            get;
            set;
        }
        /// <summary>
        /// 项目结构
        /// </summary>
        public ZippyCoder.Entity.Project Project { get; set; }
        /// <summary>
        /// 表结构
        /// </summary>
        public ZippyCoder.Entity.Table Table { get; set; }

        #region ITextTemplatingEngineHost 成员

        private string TemplateFileValue;
        public string TemplateFile
        {
            get { return TemplateFileValue; }
            set { TemplateFileValue=value; }
        }


        public object GetHostOption(string optionName)
        {
            object returnObject;
            switch (optionName)
            {
                case "CacheAssemblies":
                    returnObject = true;
                    break;
                default:
                    returnObject = null;
                    break;
            }
            return returnObject;
        }

        public bool LoadIncludeText(string requestFileName, out string content, out string location)
        {
            content = System.String.Empty;
            location = System.String.Empty;

            if (File.Exists(requestFileName))
            {
                content = File.ReadAllText(requestFileName);
                return true;
            }
            else
            {
                return false;
            }
        }


        private CompilerErrorCollection errorsValue;
        public CompilerErrorCollection Errors
        {
            get { return errorsValue; }
        }
        public void LogErrors(System.CodeDom.Compiler.CompilerErrorCollection errors)
        {
            errorsValue = errors;
        }

        public AppDomain ProvideTemplatingAppDomain(string content)
        {
            return AppDomain.CreateDomain("Generation App Domain");
        }

        public string ResolveAssemblyReference(string assemblyReference)
        {
            if (File.Exists(assemblyReference))
            {
                return assemblyReference;
            }

            string candidate = Path.Combine(Path.GetDirectoryName(this.TemplateFile), assemblyReference);
            if (File.Exists(candidate))
            {
                return candidate;
            }

            return "";
        }

        public Type ResolveDirectiveProcessor(string processorName)
        {
            if (string.Compare(processorName, "XYZ", StringComparison.OrdinalIgnoreCase) == 0)
            {
            }
            throw new Exception("Directive Processor not found");
        }

        public string ResolveParameterValue(string directiveId, string processorName, string parameterName)
        {
            if (directiveId == null)
            {
                throw new ArgumentNullException("the directiveId cannot be null");
            }
            if (processorName == null)
            {
                throw new ArgumentNullException("the processorName cannot be null");
            }
            if (parameterName == null)
            {
                throw new ArgumentNullException("the parameterName cannot be null");
            }

            //Code to provide "hard-coded" parameter values goes here.
            //This code depends on the directive processors this host will interact with.

            //If we cannot do better, return the empty string.
            return String.Empty;
        }

        public string ResolvePath(string path)
        {
            throw new NotImplementedException();
        }
        private string fileExtensionValue = ".cs";
        public string FileExtension
        {
            get { return fileExtensionValue; }
        }

        public void SetFileExtension(string extension)
        {
            fileExtensionValue = extension;
        }
        private Encoding fileEncodingValue = Encoding.UTF8;
        public Encoding FileEncoding
        {
            get { return fileEncodingValue; }
        }
        public void SetOutputEncoding(Encoding encoding, bool fromOutputDirective)
        {
            fileEncodingValue = encoding;
        }

        public IList<string> StandardAssemblyReferences
        {
            get
            {
                return new string[]
                {
                    typeof(System.Uri).Assembly.Location,
                    typeof(ZippyCoder.Entity.Project).Assembly.Location,
                    typeof(ZippyWPFForm.T4Engine.ZippyT4Host).Assembly.Location,
                    typeof(System.Data.DbType).Assembly.Location
                    
                };
            }
        }

        public IList<string> StandardImports
        {
            get
            {
                return new string[]
                {
                    "System",
                    "System.Data",
                    "System.Collections.Generic",
                    "ZippyCoder.Entity",
                    "ZippyWPFForm.T4Engine"
                };
            }
        }



        #endregion
    }
}
