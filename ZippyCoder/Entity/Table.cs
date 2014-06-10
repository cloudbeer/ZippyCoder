using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder.Entity
{
    [Serializable]
    public class Table
    {
        public string Name { get; set; }

        public string Title { get; set; }

        public string Remark { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public bool IsCoding { get; set; }

        [System.Xml.Serialization.XmlIgnore]
        public Project Parent { get; set; }

        public Table()
        {
            IsCoding = true;
        }

        private List<Col> _Cols;
        public List<Col> Cols
        {
            get
            {
                if (_Cols == null)
                    _Cols = new List<Col>();
                return _Cols;
            }
        }

        public Col Exists(string colName)
        {
            foreach (Col col in Cols)
            {
                if (col.Name == colName)
                    return col;
            }
            return null;
        }
        public override string ToString()
        {
            System.Text.StringBuilder sb = new StringBuilder();
            System.IO.StringWriter sw = new System.IO.StringWriter(sb);
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(Table));
            xser.Serialize(sw, this);
            sw.Close();
            return sb.ToString();
        }
        public static Table LoadFromString(string xml)
        {
            System.IO.StringReader sr = new System.IO.StringReader(xml);
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(Table));
            Table table = (Table)xser.Deserialize(sr);
            return table;
        }

        /// <summary>
        /// 找到主键列
        /// </summary>
        /// <returns></returns>
        public Col FindPKCol()
        {
            foreach (Col col in Cols)
            {
                if (col.IsPK) return col;
            }
            return null;
        }
    }
}
