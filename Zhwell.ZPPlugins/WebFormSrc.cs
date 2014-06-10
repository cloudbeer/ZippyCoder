using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZippyCoder;
using ZippyCoder.Entity;
using System.IO;

namespace Zhwell.ZPPlugins
{
    [PluginIndicator(Title = "WebForm 后台管理")]
    public class WebFormSrc : ZippyCoder.AbstractCoder
    {

        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();
        private string outputPath = "c:";

        public WebFormSrc()
        {
            _Flag = "Admin";
        }

        public override void Create()
        {
            System.Windows.Forms.MessageBox.Show("请选择要输出的目录", "选择目录", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Question);
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            outputPath = dlgSavePath.SelectedPath;


            //CreateCreate();
            CreateList();
            CreateEdit();
            CreateMenuPage();

        }

        #region WebForm List

        private void CreateList()
        {
            string resourceString0 = "<%$Resources:" + project.Namespace + "Entity,{0}%>";
            string resourceString1 = "Resources." + project.Namespace + "Entity.";
            foreach (Table table in project.Tables)
            {
                string aspxFile = table.Name + ".aspx";
                string csFile = table.Name + ".aspx.cs";
                string csClassName = "Admin_" + table.Name;

                Col colPK = FindPKCol(table);
                //System.Data.DbType xtyppe = TypeConverter.ToDbType(col.DataType);
                Type pkType = ZippyCoder.TypeConverter.ToNetType(colPK.DataType);

                StringBuilder sbStartJS = new StringBuilder();
                StringBuilder sbFunctionJS = new StringBuilder();
                //StringBuilder sbHeader = new StringBuilder();
                StringBuilder sbDataList = new StringBuilder();
                StringBuilder sbQueryList = new StringBuilder();
                StringBuilder sbCsGetMethods = new StringBuilder();
                StringBuilder sbQueryFiledAspx = new StringBuilder();
                StringBuilder sbQueryFiledCsDBPara = new StringBuilder();
                StringBuilder sbQueryFiledCsBinder = new StringBuilder();
                StringBuilder sbQueryFiledCsRequest = new StringBuilder();
                StringBuilder sbQueryFiledRedirectUrl = new StringBuilder();
                StringBuilder sbQueryFieldPagerHsParam = new StringBuilder();
                StringBuilder sbCsRegisterJs = new StringBuilder();
                //StringBuilder sbCsStar = new StringBuilder();


                Stream sAspx = File.Open(outputPath + "\\" + aspxFile, FileMode.Create);
                StreamWriter swAspx = new StreamWriter(sAspx, Encoding.Default);

                Stream sCs = File.Open(outputPath + "\\" + csFile, FileMode.Create);
                StreamWriter swCs = new StreamWriter(sCs, Encoding.Default);

                int queryIndex = 0;
                foreach (Col col in table.Cols)
                {
                    Type colType = ZippyCoder.TypeConverter.ToNetType(col.DataType);
                    if ((col.UIColType & UIColTypes.Listable) == UIColTypes.Listable)
                    {

                        if (!string.IsNullOrEmpty(col.RefTable) || !string.IsNullOrEmpty(col.EnumType))
                        {
                            sbDataList.AppendLine("                    <asp:TemplateColumn HeaderText=\"" + string.Format(resourceString0, table.Name + "_" + col.Name) + "\">");
                            sbDataList.AppendLine("                        <ItemTemplate>");
                            sbDataList.AppendLine("                            <%#Get" + col.Name + "(Eval(\"" + colPK.Name + "\"), Eval(\"" + col.Name + "\"))%></ItemTemplate>");
                            sbDataList.AppendLine("                    </asp:TemplateColumn>");

                            sbCsGetMethods.AppendLine("    protected string Get" + col.Name + "(object n" + colPK.Name + ", object n" + col.Name + ")");
                            sbCsGetMethods.AppendLine("    {");
                            if (!string.IsNullOrEmpty(col.EnumType))
                                sbCsGetMethods.AppendLine("        return Zippy.Helper.ZData.EnumToString((int)n" + col.Name + ", typeof(" + col.EnumType + "), typeof(" + col.ResourceType + "));");
                            else if (!string.IsNullOrEmpty(col.RefTable))
                            {
                                sbCsGetMethods.AppendLine("        object rtn = data.db.FindUnique<" + project.Namespace + ".BLL." + col.RefTable + ">(\"" + col.RefCol + "=@" + col.RefCol + "\", \"" + col.RefColTextField + "\", Zippy.Helper.ZData.CreateParameter(\"" + col.RefCol + "\", n" + col.Name + "));");
                                sbCsGetMethods.AppendLine("        if (rtn!=null)");
                                sbCsGetMethods.AppendLine("            return rtn.ToString();");
                                sbCsGetMethods.AppendLine("        return string.Empty;");
                            }
                            sbCsGetMethods.AppendLine("    }");
                            sbCsGetMethods.AppendLine();
                        }
                        else
                        {
                            sbDataList.AppendLine("                    <asp:BoundColumn DataField=\"" + col.Name + "\" HeaderText=\"" + string.Format(resourceString0, table.Name + "_" + col.Name) + "\"></asp:BoundColumn>");
                        }
                    }
                    if ((col.UIColType & UIColTypes.Queryable) == UIColTypes.Queryable)
                    {
                        if (queryIndex % 2 == 0)
                        {
                            sbQueryFiledAspx.AppendLine("                <tr>");
                        }
                        sbQueryFiledAspx.AppendLine(@"                    <td class='label'>
                        <%=" + resourceString1 + table.Name + "_" + col.Name + @"%>: 
                    </td>
                    <td class='value'>");

                        if (col.RenderType == RenderTypes.TextBox && colType == typeof(DateTime)) //时间类型的查询会出现区间，并帮定 jquery
                        {
                            sbQueryFiledAspx.AppendLine(@"                        <asp:TextBox runat='server' ID='qtbx" + col.Name + @"Start' CssClass='textBox w3'></asp:TextBox>
                        -");
                            sbQueryFiledAspx.AppendLine("                        <asp:TextBox runat='server' ID='qtbx" + col.Name + "End' CssClass='textBox w3'></asp:TextBox>");

                            sbStartJS.AppendLine("        $(\"#<%=qtbx" + col.Name + "Start.ClientID %>\").datepicker();");
                            sbStartJS.AppendLine("        $(\"#<%=qtbx" + col.Name + "End.ClientID %>\").datepicker();");

                            sbCsRegisterJs.AppendLine(@"
            includeScript = ""\r\n<link type='text/css' rel='stylesheet' href='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker.css' />"";
            includeScript += ""\r\n<script src='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker.js' type='text/javascript'></script>"";
            includeScript += ""\r\n<script src='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker-zh_CN.js' type='text/javascript'></script>\r\n"";
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), ""ui.datepicker.include"", includeScript, false);
");

                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "Start=\" + HttpUtility.UrlEncode(qtbx" + col.Name + "Start.Text)");
                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "End=\" + HttpUtility.UrlEncode(qtbx" + col.Name + "End.Text)");


                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "Start\", q" + col.Name + "Start);");
                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "End\", q" + col.Name + "End);");

                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + "Start = Request[\"q" + col.Name + "Start\"];");
                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + "End = Request[\"q" + col.Name + "End\"];");

                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "Start))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                try");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                    dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "Start\", DateTime.Parse(q" + col.Name + "Start)));");
                            sbQueryFiledCsDBPara.AppendLine("                    sql += \" and " + col.Name + " >= @" + col.Name + "Start\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("                catch { }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "End))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                try");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                    dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "End\", DateTime.Parse(q" + col.Name + "End).AddDays(1)));");
                            sbQueryFiledCsDBPara.AppendLine("                    sql += \" and " + col.Name + " < @" + col.Name + "End\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("                catch { }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                        }
                        else if (colType == typeof(string))
                        {
                            sbQueryFiledAspx.AppendLine("                        <asp:TextBox runat='server' ID='qtbx" + col.Name + "' CssClass='textBox w6'></asp:TextBox>");
                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "=\" + HttpUtility.UrlEncode(qtbx" + col.Name + ".Text)");
                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "\", q" + col.Name + ");");
                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + " = Request[\"q" + col.Name + "\"];");
                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("               dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "\", \"%\" + q" + col.Name + " + \"%\"));");
                            sbQueryFiledCsDBPara.AppendLine("               sql += \" and " + col.Name + " like @" + col.Name + "\";");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                        }
                        else if (col.RenderType == RenderTypes.RadioButtonList || col.RenderType == RenderTypes.DropDownList)
                        {
                            if (!string.IsNullOrEmpty(col.EnumType))
                            {
                                if (!string.IsNullOrEmpty(col.ResourceType))
                                    sbQueryFiledCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), qddl" + col.Name + ", typeof(" + col.ResourceType + "), Resources.Common.Label_PlsChoose);");
                                else
                                    sbQueryFiledCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), qddl" + col.Name + ", Resources.Common.Label_PlsChoose);");
                            }
                            else if (!string.IsNullOrEmpty(col.RefTable))
                            {
                                sbQueryFiledCsBinder.AppendLine(@"
            List<" + project.Namespace + @".BLL." + col.RefTable + @"> " + col.RefTable + @"s = data.db.Take<" + project.Namespace + @".BLL." + col.RefTable + @">();
            qddl" + col.Name + @".DataSource = " + col.RefTable + @"s;
            qddl" + col.Name + @".DataTextField = """ + col.RefColTextField + @""";
            qddl" + col.Name + @".DataValueField = """ + col.RefCol + @""";
            qddl" + col.Name + @".DataBind();
            qddl" + col.Name + @".Items.Insert(0, new ListItem(Resources.Common.Label_PlsChoose, ""0""));
");
                            }
                            sbQueryFiledAspx.AppendLine("                        <asp:DropDownList runat='server' ID='qddl" + col.Name + "' CssClass='textBox w6'></asp:DropDownList>");
                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "=\" + HttpUtility.UrlEncode(qddl" + col.Name + ".SelectedValue)");
                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "\", q" + col.Name + ");");
                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + " = Request[\"q" + col.Name + "\"];");
                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                " + colType.Name + " i" + col.Name + " = (" + colType.Name + ")Convert.ChangeType(q" + col.Name + ", typeof(" + colType.Name + "));");
                            sbQueryFiledCsDBPara.AppendLine("                if (i" + col.Name + " > 0)");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                   dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "\", i" + col.Name + "));");
                            sbQueryFiledCsDBPara.AppendLine("                   sql += \" and " + col.Name + " = @" + col.Name + "\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                        }
                        else if (col.RenderType == RenderTypes.CheckBoxList)
                        {
                            if (!string.IsNullOrEmpty(col.EnumType))
                            {
                                if (!string.IsNullOrEmpty(col.ResourceType))
                                    sbQueryFiledCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), qddl" + col.Name + ", typeof(" + col.ResourceType + "), Resources.Common.Label_PlsChoose);");
                                else
                                    sbQueryFiledCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), qddl" + col.Name + ", Resources.Common.Label_PlsChoose);");
                            }
                            else if (!string.IsNullOrEmpty(col.RefTable))
                            {
                            }
                            sbQueryFiledAspx.AppendLine("                        <asp:DropDownList runat='server' ID='qddl" + col.Name + "' CssClass='textBox w6'></asp:DropDownList>");
                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "=\" + HttpUtility.UrlEncode(qddl" + col.Name + ".SelectedValue)");
                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "\", q" + col.Name + ");");
                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + " = Request[\"q" + col.Name + "\"];");
                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                " + colType.Name + " i" + col.Name + " = (" + colType.Name + ")Convert.ChangeType(q" + col.Name + ", typeof(" + colType.Name + "));");
                            sbQueryFiledCsDBPara.AppendLine("                if (i" + col.Name + " > 0)");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                   dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "\", i" + col.Name + "));");
                            sbQueryFiledCsDBPara.AppendLine("                   sql += \" and " + col.Name + " & @" + col.Name + "= @" + col.Name + "\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                        }
                        else if (col.RenderType == RenderTypes.MassiveSelector)
                        {
                        }
                        else if (colType.IsValueType) //值类型的查询也会出现区间
                        {
                            sbQueryFiledAspx.AppendLine(@"                        <asp:TextBox runat='server' ID='qtbx" + col.Name + @"Start' CssClass='textBox w3'></asp:TextBox> 
                        -");
                            sbQueryFiledAspx.AppendLine("                        <asp:TextBox runat='server' ID='qtbx" + col.Name + "End' CssClass='textBox w3'></asp:TextBox>");

                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "Start=\" + HttpUtility.UrlEncode(qtbx" + col.Name + "Start.Text)");
                            sbQueryFiledRedirectUrl.Append(" + \"&q" + col.Name + "End=\" + HttpUtility.UrlEncode(qtbx" + col.Name + "End.Text)");

                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "Start\", q" + col.Name + "Start);");
                            sbQueryFieldPagerHsParam.AppendLine("            xpager.HsParam.Add(\"q" + col.Name + "End\", q" + col.Name + "End);");

                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + "Start = Request[\"q" + col.Name + "Start\"];");
                            sbQueryFiledCsRequest.AppendLine("        string q" + col.Name + "End = Request[\"q" + col.Name + "End\"];");

                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "Start))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                try");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                    dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "Start\", Convert.ChangeType(q" + col.Name + "Start, typeof(" + colType.Name + "))));");
                            sbQueryFiledCsDBPara.AppendLine("                    sql += \" and " + col.Name + " >= @" + col.Name + "Start\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("                catch { }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                            sbQueryFiledCsDBPara.AppendLine("            if (!string.IsNullOrEmpty(q" + col.Name + "End))");
                            sbQueryFiledCsDBPara.AppendLine("            {");
                            sbQueryFiledCsDBPara.AppendLine("                try");
                            sbQueryFiledCsDBPara.AppendLine("                {");
                            sbQueryFiledCsDBPara.AppendLine("                    dbParams.Add(Zippy.Helper.ZData.CreateParameter(\"" + col.Name + "End\", Convert.ChangeType(q" + col.Name + "End, typeof(" + colType.Name + "))));");
                            sbQueryFiledCsDBPara.AppendLine("                    sql += \" and " + col.Name + " <= @" + col.Name + "End\";");
                            sbQueryFiledCsDBPara.AppendLine("                }");
                            sbQueryFiledCsDBPara.AppendLine("                catch { }");
                            sbQueryFiledCsDBPara.AppendLine("            }");
                        }
                        sbQueryFiledAspx.AppendLine("                    </td>");
                        if (queryIndex % 2 == 1)
                        {
                            sbQueryFiledAspx.AppendLine("                </tr>");
                        }
                        queryIndex++;
                    }

                }
                sbDataList.AppendLine("                    <asp:TemplateColumn HeaderText=\"<%$ Resources:Common,Label_Manage %>\">");
                sbDataList.AppendLine("                        <ItemTemplate>");
                sbDataList.AppendLine("                            <a href='" + table.Name + "_Edit.aspx?pkid=<%# Eval(\"" + colPK.Name + "\")%>&ReturnUrl=<%# HttpUtility.UrlEncode(PageSelfAndQuery)%>'><%# Resources.Common.Label_Modify%></a> | ");
                sbDataList.AppendLine("                            <a href='javascript:;' onclick='del(\"<%# Eval(\"" + colPK.Name + "\")%>\", this)'><%# Resources.Common.Label_Delete%></a>");
                sbDataList.AppendLine("                        </ItemTemplate>");
                sbDataList.AppendLine("                    </asp:TemplateColumn>");


                sbFunctionJS.AppendLine(@"

    function del(pkid, sender){
        if (!confirm('<%=Resources.Common.Tip_ConfirmDelete %>')) return;
        url = '<%=PageSelf %>';
        para = 'act=del&pkid=' + pkid;
        $.post(url, para, function(res){
            if (res=='1'){
                $(sender).parent().parent().remove();
            }else{
                alert('<%=Resources.Common.Error_Operation %>');
            }
            
        });
    }
");

                //对查询列进行检测
                if (queryIndex == 0) //没有查询列
                {
                    sbQueryFiledAspx.AppendLine("<tr><td></td></tr>");
                }
                else if (queryIndex > 1 && queryIndex % 2 == 1)
                {
                    sbQueryFiledAspx.AppendLine(@"                    <td>
                    </td>
                    <td>
                    </td>
                </tr>");
                }

                swAspx.WriteLine(@"<%@ Page Language=""C#"" MasterPageFile=""~/MasterPage.master"" AutoEventWireup=""true"" CodeFile=""" + csFile + @""" Inherits=""" + csClassName + @""" Title=""" + string.Format(resourceString0, table.Name) + @""" %>");
                swAspx.WriteLine("<asp:Content ID=\"Content1\" ContentPlaceHolderID=\"head\" runat=\"Server\">");
                swAspx.WriteLine("</asp:Content>");
                swAspx.WriteLine("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"content\" runat=\"Server\">");
                swAspx.WriteLine("    <zxp:XPanel runat=\"server\" id=\"panel\">");
                swAspx.WriteLine("        <zxp:XQueryBox ID='queryBox' runat='server'>");
                swAspx.WriteLine("            <table class='queryTable'>");
                swAspx.WriteLine(sbQueryFiledAspx);
                swAspx.WriteLine("            </table>");
                swAspx.WriteLine("            <zxp:XActionBox ID='qactionBox' runat='server'>");
                swAspx.WriteLine("                <asp:Button ID='btnQuery' runat='server' Text='<%$Resources:Common,Label_Query%>' CssClass='button btnQuery' OnClick='btnQuery_Click' />");
                swAspx.WriteLine("            </zxp:XActionBox>");
                swAspx.WriteLine("        </zxp:XQueryBox>");
                swAspx.WriteLine("        <zxp:XListBox ID=\"listBox\" runat=\"server\">");
                swAspx.WriteLine("            <asp:DataGrid ID=\"dgList\" runat=\"server\" AutoGenerateColumns=\"False\" CssClass=\"table\" EnableViewState=\"false\">");
                swAspx.WriteLine("                <HeaderStyle CssClass=\"header\" />");
                swAspx.WriteLine("                <ItemStyle CssClass=\"odd\" />");
                swAspx.WriteLine("                <AlternatingItemStyle CssClass=\"even\" />");
                swAspx.WriteLine("                <Columns>");
                swAspx.WriteLine(sbDataList);
                swAspx.WriteLine("                </Columns>");
                swAspx.WriteLine("            </asp:DataGrid>");
                swAspx.WriteLine("            <zxp:XPager runat=\"server\" ID=\"xpager\"></zxp:XPager>");
                swAspx.WriteLine("        </zxp:XListBox>");
                swAspx.WriteLine("    </zxp:XPanel>");

                swAspx.WriteLine("    <script type=\"text/javascript\">");
                swAspx.WriteLine("    $(function(){");
                swAspx.WriteLine(sbStartJS);
                swAspx.WriteLine("    });");
                swAspx.WriteLine(sbFunctionJS);
                swAspx.WriteLine("    </script>");

                swAspx.WriteLine("</asp:Content>");




                swCs.WriteLine(@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using " + project.Namespace + @".Entity;

public partial class " + csClassName + @" : Zippy.Web.PageUser
{
    protected " + pkType.FullName + @" pkid;
    private string includeScript = string.Empty;
    private " + project.Namespace + @".BLL." + table.Name + @" data;
    protected void Page_Load(object sender, EventArgs e)
    {
        panel.Title = " + resourceString1 + table.Name + @" + Resources.Common.Label_List;
        Page.Title = Resources.Common.Label_List  + " + resourceString1 + table.Name + @";
        panel.ToolButtons.Add(new Zippy.Web.UI.XControls.XHtmlAnchor(Resources.Common.Label_Create, """ + table.Name + @"_Edit.aspx""));
        panel.ToolButtons.Add(new Zippy.Web.UI.XControls.XHtmlAnchor(Resources.Common.Label_List, """ + table.Name + @".aspx""));

        try
        {
            pkid = " + pkType.FullName + @".Parse(Request[""pkid""]);
        }catch{}
" + sbQueryFiledCsRequest + @"
        if (!Page.IsPostBack)
        {
            data = new " + project.Namespace + @".BLL." + table.Name + @"();

" + sbCsRegisterJs + sbQueryFiledCsBinder + @"
            if (_action == ""del"")
            {
                data." + colPK.Name + @" = pkid;
                if (data.Delete() > 0)
                {
                    WriteAjax(1);
                }
                else
                {
                    WriteAjax(0);
                }
                return;
            }
            string sql = ""1=1"";
            List<System.Data.Common.DbParameter> dbParams = new List<System.Data.Common.DbParameter>();
" + sbQueryFiledCsDBPara + @"

            Zippy.Data.Collections.PaginatedList<" + project.Namespace + @".BLL." + table.Name + @"> list = data.Take(sql, """ + colPK.Name + @" desc"", _pageSize, _pageNumber, dbParams.ToArray());
            dgList.DataSource = list;
            dgList.DataBind();
            xpager.PageNumber = _pageNumber;
            xpager.PageSize = _pageSize;
            xpager.RecordCount = list.TotalCount;
" + sbQueryFieldPagerHsParam + @"
        }        
    }
" + sbCsGetMethods + @"
    protected void btnQuery_Click(object sender, EventArgs e)
    {
        Response.Redirect(PageSelf + ""?act=list""" + sbQueryFiledRedirectUrl + @");
    }
}
");


                swCs.Close();
                sCs.Close();

                swAspx.Close();
                sAspx.Close();
            }
        }

        #endregion

        #region WebForm Edit

        private void CreateEdit()
        {
            string resourceString0 = "<%$Resources:" + project.Namespace + "Entity,{0}%>";
            string resourceString1 = "Resources." + project.Namespace + "Entity.";

            foreach (Table table in project.Tables)
            {
                string aspxFile = table.Name + "_Edit.aspx";
                string csFile = table.Name + "_Edit.aspx.cs";
                string csClassName = "Admin_" + table.Name + "_Edit";

                Col colPK = FindPKCol(table);
                Type pkType = ZippyCoder.TypeConverter.ToNetType(colPK.DataType);

                StringBuilder sbStartJS = new StringBuilder();

                Stream sAspx = File.Open(outputPath + "\\" + aspxFile, FileMode.Create);
                StreamWriter swAspx = new StreamWriter(sAspx);

                Stream sCs = File.Open(outputPath + "\\" + csFile, FileMode.Create);
                StreamWriter swCs = new StreamWriter(sCs);


                StringBuilder sbLoad = new StringBuilder();
                StringBuilder sbInsert = new StringBuilder();
                StringBuilder sbCsBinder = new StringBuilder();
                StringBuilder sbAspx = new StringBuilder();
                StringBuilder sbCsRegisterJs = new StringBuilder();


                foreach (Col col in table.Cols)
                {
                    Type colType = ZippyCoder.TypeConverter.ToNetType(col.DataType);
                    if ((col.UIColType & UIColTypes.Editable) == UIColTypes.Editable)
                    {

                        sbAspx.AppendLine("                <tr>");
                        sbAspx.AppendLine("                    <td class=\"label\">");
                        sbAspx.AppendLine("                        <%=" + resourceString1 + table.Name + "_" + col.Name + @"%>:");
                        sbAspx.AppendLine("                    </td>");
                        sbAspx.AppendLine("                    <td class=\"value\">");


                        if (col.RenderType == RenderTypes.TextBox)
                        {
                            string maxLength = "";
                            if (!string.IsNullOrEmpty(col.Length))
                                maxLength = col.Length.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                            if (!string.IsNullOrEmpty(maxLength))
                                maxLength = "MaxLength=\"" + maxLength + "\"";
                            sbAspx.AppendLine("                        <asp:TextBox ID=\"edi_" + col.Name + "\" runat=\"server\" CssClass=\"" + col.CssClass + " " + col.CssClassLength + "\" " + maxLength + "></asp:TextBox>");


                            if (colType == typeof(string))
                            {
                                sbLoad.AppendLine("            edi_" + col.Name + ".Text = entity." + col.Name + ";");
                                sbInsert.AppendLine("            entity." + col.Name + " = edi_" + col.Name + ".Text;");
                            }
                            else
                            {
                                if (colType.IsValueType)
                                {
                                    sbLoad.AppendLine("            edi_" + col.Name + ".Text = entity." + col.Name + ".ToString();");
                                    sbInsert.AppendLine("            try");
                                    sbInsert.AppendLine("            {");
                                    //swCs.WriteLine("                entity." + col.Name + " = (" + colType.FullName + ")Convert.ChangeType(edi_" + col.Name + ".Text, TypeCode." + colType.Name + ");");
                                    sbInsert.AppendLine("                entity." + col.Name + " = (" + colType.FullName + ")Convert.ChangeType(edi_" + col.Name + ".Text, TypeCode." + colType.Name + ");");
                                    sbInsert.AppendLine("            } catch { }");
                                    if (colType == typeof(DateTime))
                                    {
                                        sbCsRegisterJs.AppendLine(@"
            includeScript = ""\r\n<link type='text/css' rel='stylesheet' href='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker.css' />"";
            includeScript += ""\r\n<script src='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker.js' type='text/javascript'></script>"";
            includeScript += ""\r\n<script src='"" + Zippy.Helper.ZWeb.Root + ""/js/jquery/ui.datepicker-zh_CN.js' type='text/javascript'></script>\r\n"";
            Page.ClientScript.RegisterClientScriptBlock(typeof(Page), ""ui.datepicker.include"", includeScript, false);
");

                                        sbStartJS.AppendLine("        $(\"#<%=edi_" + col.Name + ".ClientID %>\").datepicker();");
                                    }

                                }
                                else
                                {
                                    sbInsert.AppendLine("            entity." + col.Name + " = edi_" + col.Name + ".Text;");
                                    sbLoad.AppendLine("            edi_" + col.Name + ".Text = entity." + col.Name + ".ToString();");
                                }
                            }
                        }
                        else if (col.RenderType == RenderTypes.TextArea)
                        {
                            sbAspx.AppendLine("                        <asp:TextBox ID=\"edi_" + col.Name + "\" runat=\"server\" CssClass=\"" + col.CssClass + " " + col.CssClassLength + "\" Rows=\"8\" TextMode=\"MultiLine\"></asp:TextBox>");

                            sbLoad.AppendLine("            edi_" + col.Name + ".Text = entity." + col.Name + ";");
                            sbInsert.AppendLine("            entity." + col.Name + " = edi_" + col.Name + ".Text;");
                        }
                        else if (col.RenderType == RenderTypes.Html)
                        {
                            sbAspx.AppendLine("                        <zxp:FCKEditor runat=\"server\" ID=\"edi_" + col.Name + "\"></zxp:FCKEditor>");

                            sbLoad.AppendLine("            edi_" + col.Name + ".Text = entity." + col.Name + ";");
                            sbInsert.AppendLine("            entity." + col.Name + " = edi_" + col.Name + ".Text;");
                        }
                        else if (col.RenderType == RenderTypes.CheckBoxList)
                        {
                            if (!string.IsNullOrEmpty(col.EnumType))
                            {
                                sbAspx.AppendLine("                        <asp:CheckBoxList ID=\"edi_" + col.Name + "\" runat=\"server\" RepeatDirection=\"Horizontal\"></asp:CheckBoxList>");

                                if (!string.IsNullOrEmpty(col.ResourceType))
                                    sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), edi_" + col.Name + ", typeof(" + col.ResourceType + "));");
                                else
                                    sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), edi_" + col.Name + ");");

                                sbLoad.AppendLine(@"
            foreach (ListItem li in edi_" + col.Name + @".Items)
            {
                int xValue = int.Parse(li.Value);
                if ((xValue & entity." + col.Name + @".Value) == xValue)
                    li.Selected = true;
                else
                    li.Selected = false;
            }

");
                                sbInsert.AppendLine(@"
            int i" + col.Name + @" = 0;
            foreach (ListItem li in edi_" + col.Name + @".Items)
            {
                if (li.Selected)
                {
                    int xValue = int.Parse(li.Value);
                    i" + col.Name + @" = i" + col.Name + @" | xValue;
                }
            }
            entity." + col.Name + @" = i" + col.Name + @";

");

                            }
                        }
                        else if (col.RenderType == RenderTypes.RadioButtonList)
                        {
                            sbAspx.AppendLine("                        <asp:" + col.RenderType.ToString() + " ID=\"edi_" + col.Name + "\" runat=\"server\" RepeatDirection=\"Horizontal\"></asp:" + col.RenderType.ToString() + ">");

                            if (!string.IsNullOrEmpty(col.EnumType))
                            {
                                if (!string.IsNullOrEmpty(col.ResourceType))
                                    sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + @"), edi_" + col.Name + ", typeof(" + col.ResourceType + "));");
                                else
                                    sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), edi_" + col.Name + ");");

                                sbLoad.AppendLine("            edi_" + col.Name + ".SelectedValue = entity." + col.Name + ".ToString();");
                                sbInsert.AppendLine(@"
            if (!string.IsNullOrEmpty(edi_" + col.EnumType + @".SelectedValue))
            {
                entity." + col.EnumType + @" = int.Parse(edi_" + col.EnumType + @".SelectedValue);
            }
");

                            }
                        }
                        else if (col.RenderType == RenderTypes.DropDownList)
                        {
                            sbAspx.AppendLine("                        <asp:" + col.RenderType.ToString() + " ID=\"edi_" + col.Name + "\" runat=\"server\" RepeatDirection=\"Horizontal\"></asp:" + col.RenderType.ToString() + ">");

                            sbLoad.AppendLine("            edi_" + col.Name + ".SelectedValue = entity." + col.Name + ".ToString();");
                            sbInsert.AppendLine(@"
            if (!string.IsNullOrEmpty(edi_" + col.Name + @".SelectedValue))
            {
                entity." + col.Name + @" = int.Parse(edi_" + col.Name + @".SelectedValue);
            }
");
                            if (!string.IsNullOrEmpty(col.EnumType))
                            {
                                if (!string.IsNullOrEmpty(col.ResourceType))
                                {
                                    if (col.IsNull)
                                        sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + @"), edi_" + col.Name + ", typeof(" + col.ResourceType + "), Resources.Common.Label_PlsChoose);");
                                    else
                                        sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + @"), edi_" + col.Name + ", typeof(" + col.ResourceType + "));");
                                }
                                else
                                {
                                    if (col.IsNull)
                                        sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), edi_" + col.Name + ", Resources.Common.Label_PlsChoose);");
                                    else
                                        sbCsBinder.AppendLine("            Zippy.Helper.ZData.Bind(typeof(" + col.EnumType + "), edi_" + col.Name + ");");
                                }

                            }
                            else if (!string.IsNullOrEmpty(col.RefTable))
                            {
                                sbCsBinder.AppendLine(@"
        List<" + project.Namespace + @".BLL." + col.RefTable + @"> " + col.RefTable + @"s = entity.db.Take<" + project.Namespace + @".BLL." + col.RefTable + @">();
        edi_" + col.Name + @".DataSource = " + col.RefTable + @"s;
        edi_" + col.Name + @".DataTextField = """ + col.RefColTextField + @""";
        edi_" + col.Name + @".DataValueField = """ + col.RefCol + @""";
        edi_" + col.Name + @".DataBind();
");
                                if (col.IsNull)
                                {
                                    sbCsBinder.AppendLine(@"
        edi_" + col.Name + @".Items.Insert(0, new ListItem(Resources.Common.Label_PlsChoose, ""0""));
");
                                }
                            }
                        }
                        else if (col.RenderType == RenderTypes.MassiveSelector)
                        {
                            sbAspx.AppendLine("                        <asp:HiddenField ID=\"edi_" + col.Name + "\" runat=\"server\"></asp:HiddenField>");
                        }
                        else if (col.RenderType == RenderTypes.UrlQuery) //如果是
                        {
                        }

                        //以下是加入客户端验证...
                        if (col.RenderType == RenderTypes.TextArea || col.RenderType == RenderTypes.TextBox)
                        {

                            if (col.DataType == System.Data.SqlDbType.Int || col.DataType == System.Data.SqlDbType.BigInt || col.DataType == System.Data.SqlDbType.SmallInt)
                            {
                                sbAspx.AppendLine("                        <asp:RegularExpressionValidator ID=\"reg" + col.Name + "\" runat=\"server\" ErrorMessage=\"<%$Resources:Common, Error_WrongNumber %>\" ValidationExpression=\"\\d\" Display=\"Dynamic\" ControlToValidate=\"edi_" + col.Name + "\"></asp:RegularExpressionValidator>");
                            }
                            else if (col.DataType == System.Data.SqlDbType.Decimal || col.DataType == System.Data.SqlDbType.Float || col.DataType == System.Data.SqlDbType.Money ||
                                col.DataType == System.Data.SqlDbType.Real || col.DataType == System.Data.SqlDbType.SmallMoney || col.DataType == System.Data.SqlDbType.Decimal)
                            {
                                sbAspx.AppendLine("                        <asp:RegularExpressionValidator ID=\"reg" + col.Name + "\" runat=\"server\" ErrorMessage=\"<%$Resources:Common, Error_WrongNumber %>\" ValidationExpression=\"" + @"^\d+(\.\d+)?$" + "\" Display=\"Dynamic\" ControlToValidate=\"edi_" + col.Name + "\"></asp:RegularExpressionValidator>");
                            }


                            if (!col.IsNull)
                            {
                                sbAspx.AppendLine("                        <asp:RequiredFieldValidator ID=\"rq" + col.Name + "\" runat=\"server\" ErrorMessage=\"<%$Resources:Common, Error_Required %>\"  Display=\"Dynamic\" ControlToValidate=\"edi_" + col.Name + "\"></asp:RequiredFieldValidator>");
                            }
                        }
                        if (!string.IsNullOrEmpty(col.ValidateRegex))
                        {
                            sbAspx.AppendLine("                        <asp:RegularExpressionValidator ID=\"regCustom" + col.Name + "\" runat=\"server\" ErrorMessage=\"<%$Resources:Common, Error_IllegalData %>\" ValidationExpression=\"" + col.ValidateRegex + "\" Display=\"Dynamic\" ControlToValidate=\"edi_" + col.Name + "\"></asp:RegularExpressionValidator>");
                        }

                        sbAspx.AppendLine("                    </td>");
                        sbAspx.AppendLine("                </tr>");
                        sbAspx.AppendLine();
                    }
                }


                swAspx.WriteLine(@"<%@ Page Language=""C#"" MasterPageFile=""~/MasterPage.master"" AutoEventWireup=""true"" CodeFile=""" + csFile + @""" Inherits=""" + csClassName + @""" Title=""" + string.Format(resourceString0, table.Name) + @""" %>");
                swAspx.WriteLine("<asp:Content ID=\"Content1\" ContentPlaceHolderID=\"head\" runat=\"Server\">");
                swAspx.WriteLine("</asp:Content>");
                swAspx.WriteLine("<asp:Content ID=\"Content2\" ContentPlaceHolderID=\"content\" runat=\"Server\">");
                swAspx.WriteLine("    <zxp:XPanel runat=\"server\" id=\"panel\">");
                swAspx.WriteLine("        <zxp:XEditBox ID=\"editBox\" runat=\"server\">");
                swAspx.WriteLine("            <table class=\"editTable\">");
                swAspx.WriteLine(sbAspx.ToString());
                swAspx.WriteLine("            </table>");
                swAspx.WriteLine("            <zxp:XActionBox runat=\"server\" ID=\"actionBox\">");
                swAspx.WriteLine("                <asp:Button ID=\"btnSave\" runat=\"server\" Text=\"<%$Resources:Common,Label_Save%>\" CssClass=\"button btnSave\" OnClick=\"btnSave_Click\" />");
                swAspx.WriteLine("                <asp:Button ID=\"btnCancel\" runat=\"server\" Text=\"<%$Resources:Common,Label_Cancel%>\" CssClass=\"button btnCancel\" OnClick=\"btnCancel_Click\" CausesValidation=\"false\" />");
                swAspx.WriteLine("            </zxp:XActionBox>");
                swAspx.WriteLine("        </zxp:XEditBox>");
                swAspx.WriteLine("    </zxp:XPanel>");

                swAspx.WriteLine("    <script type=\"text/javascript\">");
                swAspx.WriteLine("    $(function(){");
                swAspx.WriteLine(sbStartJS);
                swAspx.WriteLine("    });");
                swAspx.WriteLine("    </script>");

                swAspx.WriteLine("</asp:Content>");



                swCs.WriteLine(@"using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using " + project.Namespace + @".Entity;

public partial class " + csClassName + @" : Zippy.Web.PageUser
{
    protected " + pkType.FullName + @" pkid;
    private " + project.Namespace + ".BLL." + table.Name + @" entity;
    private string includeScript = string.Empty;
    protected void Page_Load(object sender, EventArgs e)
    {
        panel.Title = Resources.Common.Label_Modify  + " + resourceString1 + table.Name + @";
        Page.Title = Resources.Common.Label_List  + " + resourceString1 + table.Name + @";
        panel.ToolButtons.Add(new Zippy.Web.UI.XControls.XHtmlAnchor(Resources.Common.Label_Create, """ + table.Name + @"_Create.aspx""));
        panel.ToolButtons.Add(new Zippy.Web.UI.XControls.XHtmlAnchor(Resources.Common.Label_List, """ + table.Name + @".aspx""));
        try
        {
            pkid = " + pkType.FullName + @".Parse(Request[""pkid""]);
        }catch{}
        entity = new " + project.Namespace + ".BLL." + table.Name + @"();
        if (!Page.IsPostBack)
        {
" + sbCsRegisterJs + sbCsBinder + @"
            if (pkid!=null && (string.IsNullOrEmpty(pkid.ToString()) || !pkid.Equals(0))) 
                LoadDefault();
        }
        
    }

    protected void LoadDefault()
    {
        entity = entity.db.FindUnique<" + project.Namespace + @".BLL." + table.Name + @">(pkid);
        if (entity != null)
        {
" + sbLoad + @"
        }
        else
        {
            WriteAjax(Resources.Common.Error_IllegalData);
        }
    }
    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (Page.IsValid)
        {
" + sbInsert + @"
            if (pkid!=null && (string.IsNullOrEmpty(pkid.ToString()) || !pkid.Equals(0))) 
            {
                entity." + colPK.Name + @" = pkid;

                if (entity.Update() > 0)
                {
                    LastReturn(""" + table.Name + @".aspx"");
                }
                else
                {
                    JSAlert(Resources.Common.Error_Save);
                }
            }
            else{");
                if (colPK.AutoIncrease)
                {
                    swCs.WriteLine(@"
                entity." + colPK.Name + @" = null;");
                }

                swCs.WriteLine(@"
                if (entity.Insert() > 0)
                {
                    LastReturn(""" + table.Name + @".aspx"");
                }
                else
                {
                    JSAlert(Resources.Common.Error_Save);
                }         
            }
        }
    }

    protected void btnCancel_Click(object sender, EventArgs e)
    {
        LastReturn(""" + table.Name + @".aspx"");
        
    }
}
");


                swCs.Close();
                sCs.Close();

                swAspx.Close();
                sAspx.Close();

            }



        }
        #endregion
        
        #region WebForm Menu

        private void CreateMenuPage()
        {
            string resourceString1 = "Resources." + project.Namespace + "Entity.";
            string aspxFile = "AdminLeft.aspx";
            string csFile = "AdminLeft.aspx.cs";
            Stream sAspx = File.Open(outputPath + "\\" + aspxFile, FileMode.Create);
            StreamWriter swAspx = new StreamWriter(sAspx, Encoding.Default);

            Stream sCs = File.Open(outputPath + "\\" + csFile, FileMode.Create);
            StreamWriter swCs = new StreamWriter(sCs);

            StringBuilder sbMenu = new StringBuilder();

            foreach (Table table in project.Tables)
            {
                sbMenu.AppendLine("                            <li class='treeMenu_child'><span><a href='" + table.Name + ".aspx' target='right'><%=" + resourceString1 + table.Name + "%></a></span></li>");
            }

            swAspx.WriteLine(@"
<%@ Page Language='C#' AutoEventWireup='true' CodeFile='AdminLeft.aspx.cs' Inherits='Admin_AdminLeft' %>

<!DOCTYPE html PUBLIC '-//W3C//DTD XHTML 1.0 Frameset//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-frameset.dtd'>
<html xmlns='http://www.w3.org/1999/xhtml'>
<head id='Head1' runat='server'>
    <title>管理菜单</title>
    <script type='text/javascript' src='<%= Zippy.Helper.ZWeb.Root %>/js/jquery/jquery.js'></script>
    <script src='<%= Zippy.Helper.ZWeb.Root %>/js/cloudbeer.js' type='text/javascript'></script>
</head>
<body>
    <form id='form1' runat='server'>
    <zxp:XPage runat='server' ID='admin_left' Theme='http://dpa.1nuoshop.com/theme/3'>
        <zxp:XPanel runat='server' ID='admin_sidebar' Title='<< 管理菜单'>
            <zxp:XTreeMenu runat='server' ID='XMenuBox1' AccordionMode='true'>
                <ul>
                    <li class='box_L'></li>
                    <li class='treeMenu_item'>
                        <h4>系统维护</h4>
                        <ul>
" + sbMenu.ToString() + @"
                        </ul>
                    </li>
                    <li class='box_R'></li>
                </ul>
            </zxp:XTreeMenu>
        </zxp:XPanel>

        <script type='text/javascript'>
            $(document).ready(
            function() {
                $('#admin_sidebar .treeMenu .innerTreeMenu > ul > li >  ul').hide();
                $('#admin_sidebar .treeMenu .innerTreeMenu > ul > .treeMenu_child:first >  ul').show();
                $('#admin_sidebar .treeMenu .innerTreeMenu > ul > .treeMenu_child:first ').addClass('selected');

            });

            var isOpen = true;
            var title = $('#admin_sidebar .titleBox .title').text();
            $('#admin_sidebar .titleBox').click(function() {
                if (isOpen) {
                    $('.title', this).text('>>');
                    $('#admin_sidebar  .treeMenu').hide();
                    $(parent.window.document).find('#leftBox').width(30);
                    isOpen = false;
                }
                else {
                    $('.title', this).text(title);
                    $('#admin_sidebar .treeMenu').show();
                    $(parent.window.document).find('#leftBox').width(180);
                    isOpen = true;
                }
            });
                 
        </script>

    </zxp:XPage>
    </form>
</body>
</html>
");
            swCs.WriteLine(@"
using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

public partial class Admin_AdminLeft : Zippy.Web.PageUser
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
}
");

            swCs.Close();
            sCs.Close();

            swAspx.Close();
            sAspx.Close();

        }
        #endregion

        private Col FindPKCol(Table table)
        {
            foreach (Col col in table.Cols)
            {
                if (col.IsPK) return col;
            }
            return null;
        }
    }
}
