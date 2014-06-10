using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder.Entity
{
    [Serializable]
    public class Col
    {
        public Col()
        {
            IsNull = true;
            RefTable = string.Empty;
            RefCol = string.Empty;
            Name = string.Empty;
            Title = string.Empty;
            Length = string.Empty;
            Default = string.Empty;
            Remark = string.Empty;
            RefColTextField = string.Empty;
            EnumType = string.Empty;
            ResourceType = string.Empty;
            CssClass = string.Empty;
            CssClassLength = string.Empty;
            ValidateRegex = string.Empty;
            IsCoding = true;
            WidthPx = 0;
        }

        [System.Xml.Serialization.XmlIgnore]
        public bool IsCoding { get; set; }
        public string Name { get; set; }
        public System.Data.SqlDbType DataType { get; set; }
        public string Length { get; set; }
        public bool IsPK { get; set; }
        public bool IsNull { get; set; }
        public bool Unique { get; set; }
        public bool AutoIncrease { get; set; }
        /// <summary>
        /// 是否创建索引
        /// </summary>
        public bool IsIndex { get; set; }
        public string Default { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        public string RefTable { get; set; }
        public string RefCol { get; set; }
        public string RefColTextField { get; set; }
        /// <summary>
        /// 是否强制约束
        /// </summary>
        public bool FkWithNoCheck { get; set; }
        /// <summary>
        /// 是否级联删除
        /// </summary>
        public bool FkDeleteCascade { get; set; }
        /// <summary>
        /// 在页面中的输出类型
        /// </summary>
        public RenderTypes RenderType { get; set; }
        public string EnumType { get; set; }
        public string ResourceType { get; set; }

        public string CssClass { get; set; }
        public string CssClassLength { get; set; }
        /// <summary>
        /// 列的验证串
        /// </summary>
        public string ValidateRegex { get; set; }

        public UIColTypes UIColType { get; set; }
        /// <summary>
        /// 列宽像素
        /// </summary>
        public int WidthPx { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Table Parent { get; set; }

        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(Col));
            xser.Serialize(sw, this);
            sw.Close();
            return sb.ToString();
            

        }
        public static Col LoadFromString(string xml)
        {
            System.IO.StringReader sr = new System.IO.StringReader(xml);
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(Col));
            Col col = (Col)xser.Deserialize(sr);
            return col;
        }


    }
}
