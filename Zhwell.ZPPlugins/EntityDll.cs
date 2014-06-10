using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.CSharp;
using ZippyCoder;
using ZippyCoder.Entity;

namespace Zhwell.ZPPlugins
{
    [PluginIndicator(Title = "实体Dll")]
    public class EntityDll : AbstractCoder
    {
        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();

        public EntityDll()
        {
            _Flag = "Entity";
        }
        public override void Create()
        {
            System.Windows.Forms.OpenFileDialog mappingRefrence = new System.Windows.Forms.OpenFileDialog();
            mappingRefrence.Title = "请指定参考项 Zippy.Data.dll";
            mappingRefrence.Filter = "Zippy Mapping|Zippy.Data.dll";
            if (mappingRefrence.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string zippyMappingPath = mappingRefrence.FileName;


            System.Windows.Forms.MessageBox.Show("请选择要输出的目录");
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string outputPath = dlgSavePath.SelectedPath;



            List<Table> tables = project.Tables;

            #region Generate DLL
            CSharpCodeProvider xProvider = new CSharpCodeProvider();
            CompilerParameters paras = new CompilerParameters();
            paras.GenerateExecutable = false;
            paras.GenerateInMemory = false;
            paras.OutputAssembly = outputPath + "\\" + project.Namespace + ".dll";
            paras.ReferencedAssemblies.Add(zippyMappingPath);
            paras.ReferencedAssemblies.Add("System.Data.dll");



            CodeCompileUnit[] units = new CodeCompileUnit[tables.Count];
            //xProvider.CompileAssemblyFromDom(
            for (int i = 0; i < tables.Count; i++)
            {
                Table table = tables[i];

                CodeCompileUnit compileUnit = new CodeCompileUnit();
                units[i] = compileUnit;

                CodeNamespace cnsCodeDom = new CodeNamespace(Namespace);
                compileUnit.Namespaces.Add(cnsCodeDom);

                cnsCodeDom.Imports.Add(new CodeNamespaceImport("System"));
                cnsCodeDom.Imports.Add(new CodeNamespaceImport("Zippy.Data.Mapping"));


                CodeTypeDeclaration ctd = new CodeTypeDeclaration();
                ctd.IsClass = true;
                ctd.Name = table.Name;
                ctd.TypeAttributes = TypeAttributes.Public;

                //CodeAttributeDeclaration dataTableAttr2 = new CodeAttributeDeclaration("Serializable");
                //ctd.CustomAttributes.Add(dataTableAttr2);

                CodeAttributeDeclaration dataTableAttr = new CodeAttributeDeclaration("DataTable");
                dataTableAttr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(table.Name)));
                dataTableAttr.Arguments.Add(new CodeAttributeArgument("Title", new CodePrimitiveExpression(table.Title)));
                ctd.CustomAttributes.Add(dataTableAttr);

                cnsCodeDom.Types.Add(ctd);

                List<Col> cols = table.Cols;
                for (int j = 0; j < cols.Count; j++)
                {

                    Col col = cols[j];

                    
                    System.Data.DbType xtyppe = TypeConverter.ToDbType(col.DataType);
                    Type colType = ZippyCoder.TypeConverter.ToNetType(col.DataType);

                   

                    CodeMemberField cmf = new CodeMemberField(colType.FullName + (colType.IsValueType ? "?" : ""), "_" + col.Name);
                    cmf.Attributes = MemberAttributes.Family;
                    ctd.Members.Add(cmf);

                    CodeMemberProperty cmp = new CodeMemberProperty();
                    cmp.Name = col.Name;
                    cmp.HasGet = true;
                    cmp.GetStatements.Add(new CodeSnippetExpression("return _" + col.Name));
                    cmp.HasSet = true;
                    cmp.SetStatements.Add(new CodeSnippetExpression("_" + col.Name + " = value"));
                    cmp.Type = new CodeTypeReference(colType.FullName + (colType.IsValueType ? "?" : ""));
                    cmp.Attributes = MemberAttributes.Public;

                    CodeAttributeDeclaration fieldAttr = new CodeAttributeDeclaration("DataField");
                    fieldAttr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(col.Name)));
                    if (!string.IsNullOrEmpty(col.Title))
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("Title", new CodePrimitiveExpression(col.Title)));
                    if (!string.IsNullOrEmpty(col.Length))
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("Length", new CodePrimitiveExpression(col.Length)));
                    if (!string.IsNullOrEmpty(col.Remark))
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("Description", new CodePrimitiveExpression(col.Remark)));
                    if (col.RenderType == RenderTypes.Html)
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("RenderAsHtml", new CodePrimitiveExpression(true)));
                    if (col.IsPK)
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("IsPrimaryKey", new CodePrimitiveExpression(col.IsPK)));
                    if (col.AutoIncrease)
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("AutoIncrement", new CodePrimitiveExpression(col.AutoIncrease)));
                    if (!col.IsNull)
                        fieldAttr.Arguments.Add(new CodeAttributeArgument("AllowNull", new CodePrimitiveExpression(col.IsNull)));
                    fieldAttr.Arguments.Add(new CodeAttributeArgument("DbType", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Data.DbType"), xtyppe.ToString())));
                    fieldAttr.Arguments.Add(new CodeAttributeArgument("SqlDbType", new CodeFieldReferenceExpression(new CodeTypeReferenceExpression("System.Data.SqlDbType"), col.DataType.ToString())));

                    cmp.CustomAttributes.Add(fieldAttr);

                    ctd.Members.Add(cmp);

                }

            }
            CompilerResults cr = xProvider.CompileAssemblyFromDom(paras, units);
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            if (cr.Errors.Count > 0)
            {
                // Display compilation errors.
                sb.Append("Errors building " + cr.PathToAssembly + "\r\n");
                foreach (CompilerError ce in cr.Errors)
                {
                    sb.Append(ce.ToString() + "\r\n");
                }
            }
            else
            {
                sb.Append("编译成功。\r\n");
                sb.Append("调用的命名空间为：" + Namespace);

            }

            System.Windows.Forms.MessageBox.Show(sb.ToString(), "提示信息", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information);

            #endregion
        }
    }
}
