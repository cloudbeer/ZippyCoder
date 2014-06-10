using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.VisualStudio.TextTemplating;
using System.Windows;

namespace ZippyWPFForm.T4Engine
{
    public class T4Creator
    {
        public static CompilerErrorCollection T4Errors = null;
        /// <summary>
        /// 生成代码
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="ttFile">tt模板路径</param>
        /// <param name="outputFileName">生成的文件的名称</param>
        /// <param name="fileNamePattern">需要替换的文件名</param>
        /// <param name="fileNamesDel">需要去掉的文件名前缀</param>
        /// <param name="outputPath">输出路径</param>
        /// <param name="nameSpace">命名空间</param>
        public static void CreateCode(ZippyCoder.Entity.Project project, ZippyCoder.Entity.Table table, string ttFile, string outputFileName, string fileNamePattern, string fileNamesDel, string outputPath, bool multiDir)
        {
            if (T4Errors != null)
            {
                T4Errors.Clear();
            }
            ZippyT4Host host = new ZippyT4Host();
            host.Project = project;
            host.Table = table;

            Engine engine = new Engine();
            host.TemplateFile = ttFile;


            string input = File.ReadAllText(ttFile);
            string output = engine.ProcessTemplate(input, host);

            string[] fileNameDels = fileNamesDel.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string fileNameDel in fileNameDels)
            {
                if (outputFileName.StartsWith(fileNameDel))
                {
                    outputFileName = outputFileName.Remove(0, fileNameDel.Length);
                }
            }
            //大写文件名，java 会敏感，与类对应。
            outputFileName = ZippyCoder.Helper.ToJavaClassName(outputFileName);

            if (multiDir)
            {
                if (table != null)
                {
                    outputPath = Path.Combine(outputPath, table.Name);
                    if (!System.IO.Directory.Exists(outputPath))
                    {
                        System.IO.Directory.CreateDirectory(outputPath);
                    }
                }
            }

            outputFileName = string.Format(fileNamePattern, outputFileName);
            var utf8WithoutBom = new System.Text.UTF8Encoding(false);

            File.WriteAllText(Path.Combine(outputPath, outputFileName + host.FileExtension), output, utf8WithoutBom);

            T4Errors = host.Errors;

        }

    }
}
