using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZippyCoder;
using ZippyCoder.Entity;
using System.IO;
using System.Xml;

namespace Zhwell.ZPPlugins
{
    [PluginIndicator(Title = "资源文件")]
    public class GlobalResources : AbstractCoder
    {
        System.Windows.Forms.FolderBrowserDialog dlgSavePath = new System.Windows.Forms.FolderBrowserDialog();
        public override void Create()
        {
            System.Windows.Forms.MessageBox.Show("请选择要输出的目录");
            if (dlgSavePath.ShowDialog() != System.Windows.Forms.DialogResult.OK)
                return;
            string outputPath = dlgSavePath.SelectedPath;


            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            doc.LoadXml(@"<?xml version='1.0' encoding='utf-8'?>
<root>
  <xsd:schema id='root' xmlns='' xmlns:xsd='http://www.w3.org/2001/XMLSchema' xmlns:msdata='urn:schemas-microsoft-com:xml-msdata'>
    <xsd:import namespace='http://www.w3.org/XML/1998/namespace' />
    <xsd:element name='root' msdata:IsDataSet='true'>
      <xsd:complexType>
        <xsd:choice maxOccurs='unbounded'>
          <xsd:element name='metadata'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' />
              </xsd:sequence>
              <xsd:attribute name='name' use='required' type='xsd:string' />
              <xsd:attribute name='type' type='xsd:string' />
              <xsd:attribute name='mimetype' type='xsd:string' />
              <xsd:attribute ref='xml:space' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='assembly'>
            <xsd:complexType>
              <xsd:attribute name='alias' type='xsd:string' />
              <xsd:attribute name='name' type='xsd:string' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='data'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
                <xsd:element name='comment' type='xsd:string' minOccurs='0' msdata:Ordinal='2' />
              </xsd:sequence>
              <xsd:attribute name='name' type='xsd:string' use='required' msdata:Ordinal='1' />
              <xsd:attribute name='type' type='xsd:string' msdata:Ordinal='3' />
              <xsd:attribute name='mimetype' type='xsd:string' msdata:Ordinal='4' />
              <xsd:attribute ref='xml:space' />
            </xsd:complexType>
          </xsd:element>
          <xsd:element name='resheader'>
            <xsd:complexType>
              <xsd:sequence>
                <xsd:element name='value' type='xsd:string' minOccurs='0' msdata:Ordinal='1' />
              </xsd:sequence>
              <xsd:attribute name='name' type='xsd:string' use='required' />
            </xsd:complexType>
          </xsd:element>
        </xsd:choice>
      </xsd:complexType>
    </xsd:element>
  </xsd:schema>
  <resheader name='resmimetype'>
    <value>text/microsoft-resx</value>
  </resheader>
  <resheader name='version'>
    <value>2.0</value>
  </resheader>
  <resheader name='reader'>
    <value>System.Resources.ResXResourceReader, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
  <resheader name='writer'>
    <value>System.Resources.ResXResourceWriter, System.Windows.Forms, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089</value>
  </resheader>
</root>
            ");
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            CreateResourcesKeyValue(doc, project.Namespace, project.Title);
            foreach (Table table in project.Tables)
            {
                string tName = table.Name;
                CreateResourcesKeyValue(doc, tName, table.Title);
                foreach (Col col in table.Cols)
                {
                    //if (col.Name != "Creator" &&
                    //    col.Name != "CreateDate" &&
                    //    col.Name != "Updater" &&
                    //    col.Name != "UpdateDate" &&
                    //    col.Name != "ExtendData" &&
                    //    col.Name != "Remark")
                    //{
                    CreateResourcesKeyValue(doc, tName + "_" + col.Name, col.Title);
                    //}

                }
            }

            doc.Save(outputPath + "\\" + project.Namespace + "Entity.resx");
        }
        private void CreateResourcesKeyValue(XmlDocument doc, string key, string value)
        {

            XmlElement data = doc.CreateElement("data");
            doc.ChildNodes[1].AppendChild(data);

            XmlAttribute attrName = doc.CreateAttribute("name");
            attrName.Value = key;
            data.Attributes.Append(attrName);
            XmlAttribute attrSpace = doc.CreateAttribute("xml:space");
            attrSpace.Value = "preserve";
            data.Attributes.Append(attrSpace);

            XmlElement elvalue = doc.CreateElement("value");
            data.AppendChild(elvalue);
            elvalue.InnerText = value;
        }
    }
}
