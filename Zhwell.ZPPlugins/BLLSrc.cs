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
    [PluginIndicator(Title = "商业逻辑源码")]
    public class BLLSrc : AbstractCoder
    {
        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();
        public BLLSrc()
        {
            _Flag = "BLL";
        }
        public override void Create()
        {
            System.Windows.Forms.MessageBox.Show("请选择要输出的目录");
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string outputPath = dlgSavePath.SelectedPath;
            Dictionary<string, List<RefKeyMap>> refEntities = GetFKMapping();

            foreach (Table table in project.Tables)
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
                cscg.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("using System.Collections.Generic;"), sw, cop);
                cscg.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("using System.Data;"), sw, cop);
                cscg.GenerateCodeFromCompileUnit(new CodeSnippetCompileUnit("using System.Text;"), sw, cop);

                sw.WriteLine();

                //创建命名空间
                CodeNamespace cnsCodeDom = new CodeNamespace(Namespace);

                //创建类
                CodeTypeDeclaration ctd = new CodeTypeDeclaration();
                cnsCodeDom.Types.Add(ctd);
                ctd.IsClass = true;
                ctd.Name = table.Name;
                ctd.BaseTypes.Add(project.Namespace + ".Entity." + table.Name); //继承 实体
                ctd.TypeAttributes = TypeAttributes.Public;


                foreach (RefKeyMap wmap in refEntities[table.Name])  //增加外键子元素的集合
                {
                    string fieldName = wmap.Col.Name + "s" + wmap.Table.Name + "s";

                    ctd.Members.Add(new CodeMemberField("List<" + wmap.Table.Name + ">", "_" + fieldName));

                    CodeMemberProperty cmp = new CodeMemberProperty();
                    cmp.Type = new CodeTypeReference("List<" + wmap.Table.Name + ">");
                    cmp.Attributes = MemberAttributes.Public;
                    cmp.Name = fieldName;
                    cmp.Comments.Add(new CodeCommentStatement("<summary>", true));
                    cmp.Comments.Add(new CodeCommentStatement("表示 [" + wmap.Col.Title + "] 的 [" + wmap.Table.Title + "] 集合", true));
                    cmp.Comments.Add(new CodeCommentStatement("</summary>", true));
                    cmp.HasGet = true;
                    cmp.GetStatements.Add(new CodeSnippetExpression(@"if (_" + fieldName + @" == null) {
                    if (this." + wmap.Col.Name + @" == null)
					    _" + fieldName + @" = new List<" + wmap.Table.Name + @">();
                    else
                        _" + fieldName + @" = db.Take<" + wmap.Table.Name + @">(""" + wmap.Col.Name + @"=@" + wmap.Col.Name + @""", Zippy.Helper.ZData.CreateParameter(""" + wmap.Col.Name + @""", this." + wmap.Col.RefCol + @"));
                }
                return _" + fieldName));
                    //cmp.HasSet = true;
                    //cmp.SetStatements.Add(new CodeSnippetExpression("_" + fieldName + " = value"));
                    ctd.Members.Add(cmp);
                }

                foreach (Col col in table.Cols) //找到外键映射的实体
                {
                    if (!string.IsNullOrEmpty(col.RefTable))
                    {
                        string fieldName = col.Name + "s" + col.RefTable + "";

                        ctd.Members.Add(new CodeMemberField(col.RefTable, "_" + fieldName));

                        CodeMemberProperty cmp = new CodeMemberProperty();
                        cmp.Type = new CodeTypeReference(col.RefTable);
                        cmp.Attributes = MemberAttributes.Public;
                        cmp.Name = fieldName;
                        cmp.Comments.Add(new CodeCommentStatement("<summary>", true));
                        cmp.Comments.Add(new CodeCommentStatement("表示 [" + col.Title + "] 对应的实体", true));
                        cmp.Comments.Add(new CodeCommentStatement("</summary>", true));
                        cmp.HasGet = true;
                        cmp.GetStatements.Add(new CodeSnippetExpression(@"if (_" + fieldName + @" == null) 
                    _" + fieldName + @" = db.FindUnique<" + col.RefTable + @">(""" + col.RefCol + @"=@" + col.RefCol + @""", Zippy.Helper.ZData.CreateParameter(""" + col.RefCol + @""", this." + col.Name + @"));
                return _" + fieldName));
                        //cmp.HasSet = true;
                        //cmp.SetStatements.Add(new CodeSnippetExpression("_" + fieldName + " = value"));
                        ctd.Members.Add(cmp);

                    }
                }

                CodeMemberField dbField = new CodeMemberField("Zippy.Data.IDalProvider", "db");
                dbField.Attributes = MemberAttributes.Public;
                ctd.Members.Add(dbField);

                CodeConstructor ctor0 = new CodeConstructor();
                ctd.Members.Add(ctor0);
                ctor0.Attributes = MemberAttributes.Public;
                ctor0.Statements.Add(new CodeSnippetExpression("db = Zippy.Data.DalFactory.CreateProvider()"));

                CodeConstructor ctor1 = new CodeConstructor();
                ctd.Members.Add(ctor1);
                ctor1.Parameters.Add(new CodeParameterDeclarationExpression("Zippy.Data.IDalProvider", "_db"));
                ctor1.Attributes = MemberAttributes.Public;
                ctor1.Statements.Add(new CodeSnippetExpression("db = _db"));

                Col colPK = FindPKCol(table);
                if (colPK != null)
                {
                    System.Data.DbType xtyppe = TypeConverter.ToDbType(colPK.DataType);
                    Type colType = ZippyCoder.TypeConverter.ToNetType(colPK.DataType);

                    ctd.Members.Add(CreateStaticMethod("FindUnique", table.Name, @"Zippy.Data.IDalProvider db = Zippy.Data.DalFactory.CreateProvider();
            return db.FindUnique<" + table.Name + ">(pkValue)", new CodeParameterDeclarationExpression(colType, "pkValue")));
                    ctd.Members.Add(CreateStaticMethod("FindUnique", table.Name, @"return db.FindUnique<" + table.Name + ">(pkValue)", new CodeParameterDeclarationExpression(colType, "pkValue"), new CodeParameterDeclarationExpression("Zippy.Data.IDalProvider", "db")));

                    ctd.Members.Add(CreateMethod("Delete", typeof(int), "return db.Delete<" + table.Name + ">(this." + colPK.Name + ")"));
                    ctd.Members.Add(CreateMethod("Insert", typeof(int), "int rtn = db.Insert<" + table.Name + @">(this); 
            this." + colPK.Name + @" = rtn;
            return rtn"));
                    ctd.Members.Add(CreateMethod("Update", typeof(int), "return db.Update<" + table.Name + ">(this)"));
                    ctd.Members.Add(CreateMethod("Save", typeof(bool), @"int rtn = 0;
            if (this." + colPK.Name + @" != null) 
                rtn = db.Update<" + table.Name + @">(this);
            else {
                rtn = db.Insert<" + table.Name + @">(this);
                this." + colPK.Name + @" = rtn;
            }
            return rtn > 0"));

                }


                ctd.Members.Add(CreateMethod("Take", "List<" + table.Name + ">", "return db.Take<" + table.Name + ">(true)"));
                ctd.Members.Add(CreateMethod("Take", "List<" + table.Name + ">", "return db.Take<" + table.Name + ">(count, true)", new CodeParameterDeclarationExpression(typeof(int), "count")));
                ctd.Members.Add(CreateMethod("Take", "List<" + table.Name + ">", "return db.Take<" + table.Name + ">(sqlEntry, cmdParameters)",
                    new CodeParameterDeclarationExpression(typeof(string), "sqlEntry"),
                    new CodeParameterDeclarationExpression("params System.Data.Common.DbParameter[]", "cmdParameters")));
                ctd.Members.Add(CreateMethod("Take", "Zippy.Data.Collections.PaginatedList<" + table.Name + ">", @"Zippy.Data.Collections.PaginatedList<" + table.Name + @"> rtn = new Zippy.Data.Collections.PaginatedList<" + table.Name + @">();           
            List<" + table.Name + @"> records = db.Take<" + table.Name + @">(where + "" order by "" + orderby, pageSize, pageNumber, cmdParameters);
            rtn.AddRange(records);
            rtn.PageIndex = pageNumber;
            rtn.PageSize = pageSize;
            rtn.TotalCount = db.Count<" + table.Name + @">(where, cmdParameters);
            return rtn",
                    new CodeParameterDeclarationExpression(typeof(string), "where"),
                    new CodeParameterDeclarationExpression(typeof(string), "orderby"),
                    new CodeParameterDeclarationExpression(typeof(int), "pageSize"),
                    new CodeParameterDeclarationExpression(typeof(int), "pageNumber"),
                    new CodeParameterDeclarationExpression("params System.Data.Common.DbParameter[]", "cmdParameters")));


                cscg.GenerateCodeFromNamespace(cnsCodeDom, sw, cop);
                sw.Close();
                s.Close();

            }

        }


        private Col FindPKCol(Table table)
        {
            foreach (Col col in table.Cols)
            {
                if (col.IsPK) return col;
            }
            return null;
        }

        protected CodeMemberMethod CreateStaticMethod(string name, Type returnType, string statements, params CodeParameterDeclarationExpression[] paras)
        {
            CodeMemberMethod rtn = new CodeMemberMethod();
            rtn.Name = name;
            foreach (CodeParameterDeclarationExpression para in paras)
            {
                rtn.Parameters.Add(para);
            }
            rtn.ReturnType = new CodeTypeReference(returnType);
            rtn.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            rtn.Statements.Add(new CodeSnippetExpression(statements));
            return rtn;
        }
        protected CodeMemberMethod CreateStaticMethod(string name, string returnType, string statements, params CodeParameterDeclarationExpression[] paras)
        {
            CodeMemberMethod rtn = new CodeMemberMethod();
            rtn.Name = name;
            foreach (CodeParameterDeclarationExpression para in paras)
            {
                rtn.Parameters.Add(para);
            }
            rtn.ReturnType = new CodeTypeReference(returnType);
            rtn.Attributes = MemberAttributes.Public | MemberAttributes.Static;
            rtn.Statements.Add(new CodeSnippetExpression(statements));
            return rtn;
        }
        protected CodeMemberMethod CreateMethod(string name, string returnType, string statements, params CodeParameterDeclarationExpression[] paras)
        {
            CodeMemberMethod rtn = new CodeMemberMethod();
            rtn.Name = name;
            foreach (CodeParameterDeclarationExpression para in paras)
            {
                rtn.Parameters.Add(para);
            }
            rtn.ReturnType = new CodeTypeReference(returnType);
            rtn.Attributes = MemberAttributes.Public;
            rtn.Statements.Add(new CodeSnippetExpression(statements));
            return rtn;
        }
        protected CodeMemberMethod CreateMethod(string name, Type returnType, string statements, params CodeParameterDeclarationExpression[] paras)
        {
            CodeMemberMethod rtn = new CodeMemberMethod();
            rtn.Name = name;
            foreach (CodeParameterDeclarationExpression para in paras)
            {
                rtn.Parameters.Add(para);
            }
            rtn.ReturnType = new CodeTypeReference(returnType);
            rtn.Attributes = MemberAttributes.Public;
            rtn.Statements.Add(new CodeSnippetExpression(statements));
            return rtn;
        }

        private Dictionary<string, List<RefKeyMap>> GetFKMapping()
        {
            Dictionary<string, List<RefKeyMap>> rtn = new Dictionary<string, List<RefKeyMap>>();
            foreach (Table table in project.Tables)
            {
                List<RefKeyMap> wmap = new List<RefKeyMap>();
                foreach (Table reftable in project.Tables)
                {
                    foreach (Col refCol in reftable.Cols)
                    {
                        if (refCol.RefTable == table.Name)
                            wmap.Add(new RefKeyMap(reftable, refCol));
                    }
                }
                rtn.Add(table.Name, wmap);
            }
            return rtn;
        }

        class RefKeyMap
        {
            public RefKeyMap(Table nTable, Col nCol)
            {
                Table = nTable;
                Col = nCol;
            }
            public Table Table;
            public Col Col;
        }
    }


}
