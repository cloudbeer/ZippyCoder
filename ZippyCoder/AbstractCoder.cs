using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZippyCoder.Entity;

namespace ZippyCoder
{
    public abstract class AbstractCoder
    {
        protected string Namespace;

        /// <summary>
        /// 是标志（次字符串将指定目录名称，下级命名空间等）
        /// </summary>
        protected string _Flag;

        protected Project project;
        public virtual Project DataSourse
        {
            get { return project; }
            set
            {
                project = value;
                if (!string.IsNullOrEmpty(_Flag))
                    Namespace = project.Namespace + "." + _Flag;
                else
                    Namespace = project.Namespace;

            }
        }


        protected void AddCode(string codeString, System.CodeDom.CodeMemberMethod cmm)
        {
            System.CodeDom.CodeStatement code = new System.CodeDom.CodeSnippetStatement(codeString);
            cmm.Statements.Add(code);
        }
        protected void AddGetCode(string codeString, System.CodeDom.CodeMemberProperty cmp)
        {
            System.CodeDom.CodeStatement code = new System.CodeDom.CodeSnippetStatement(codeString);
            cmp.GetStatements.Add(code);
        }

        /// <summary>
        /// 抽象方法，实现此方法输出代码
        /// </summary>
        public abstract void Create();
    }
}
