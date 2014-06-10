using System;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Microsoft.CSharp;
using ZippyCoder;
using ZippyCoder.Entity;

namespace Zhwell.ZPPlugins
{
    /// <summary>
    /// 实体映射
    /// </summary>
    [PluginIndicator(Title = "实体源代码")]
    public class EntitySrc : AbstractCoder
    {
        /// <summary>
        /// 构造器
        /// </summary>
        public EntitySrc()
        {
            _Flag = "Entity";
        }

        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();

        /// <summary>
        /// 生成
        /// </summary>
        public override void Create()
        {
            System.Windows.Forms.MessageBox.Show("请选择要输出的目录");
            
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string outputPath = dlgSavePath.SelectedPath;

            List<Table> tables = project.Tables;


            #region Generate Code
            foreach (Table table in tables)
            {


                Stream s = File.Open(outputPath + "\\" + table.Name + ".cs", FileMode.Create);
                StreamWriter sw = new StreamWriter(s, Encoding.Default);

                CSharpCodeProvider cscProvider = new CSharpCodeProvider();
                ICodeGenerator cscg = cscProvider.CreateGenerator(sw);
                CodeGeneratorOptions cop = new CodeGeneratorOptions();
                cop.BlankLinesBetweenMembers = false;
                cop.ElseOnClosing = true;

                //Create Class Using Statements
                cscg.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("using System;"), sw, cop);
                cscg.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("using Zippy.Data.Mapping;"), sw, cop);
                sw.WriteLine();

                //创建命名空间
                CodeNamespace cnsCodeDom = new CodeNamespace(Namespace);

                //创建类
                CodeTypeDeclaration ctd = new CodeTypeDeclaration();
                ctd.IsClass = true;
                ctd.Name = table.Name;
                ctd.TypeAttributes = TypeAttributes.Public;


                //CodeAttributeDeclaration serAttr = new CodeAttributeDeclaration("Serializable");
                //ctd.CustomAttributes.Add(serAttr);

                CodeAttributeDeclaration dataTableAttr = new CodeAttributeDeclaration("DataTable");
                dataTableAttr.Arguments.Add(new CodeAttributeArgument("Name", new CodePrimitiveExpression(table.Name)));
                dataTableAttr.Arguments.Add(new CodeAttributeArgument("Title", new CodePrimitiveExpression(table.Title)));
                ctd.CustomAttributes.Add(dataTableAttr);


                CodeAttributeDeclaration dataTableAttr2 = new CodeAttributeDeclaration("Serializable");
                ctd.CustomAttributes.Add(dataTableAttr2);

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


                //cnsCodeDom.Types.Add(ctd);

                cscg.GenerateCodeFromNamespace(cnsCodeDom, sw, cop);
                sw.Close();
                s.Close();



                //cnsCodeDoms.Types.Add(ctds);
                //cscgs.GenerateCodeFromNamespace(cnsCodeDoms, sws, cops);
                //sws.Close();
                //ss.Close();
            }
            #endregion
            //System.Windows.Forms.MessageBox.Show("生成成功！");

        }
    }
}
