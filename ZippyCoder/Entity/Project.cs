using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder.Entity
{
    [Serializable]
    public class Project
    {
        public string Namespace { get; set; }
        public string Title { get; set; }
        public string Remark { get; set; }
        private List<Table> _Tables;
        public List<Table> Tables
        {
            get
            {
                if (_Tables == null)
                    _Tables = new List<Table>();
                return _Tables;
            }
        }

        public void SaveProject(string path)
        {
            if (!string.IsNullOrEmpty(Namespace))
            {
                System.IO.FileInfo fiPath = new System.IO.FileInfo(path);

                if (fiPath.Attributes.ToString().IndexOf("ReadOnly") != -1)
                    throw new Exception("文件只读了，请修改文件访问权限，或将此文件从源代码管理中签出。");

                System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(this.GetType());
                System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
                xser.Serialize(sw, this);
                sw.Close();

            }
        }

        public static Project Load(string path)
        {
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(Project));
            System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Default);
            Project rtn = (Project)xser.Deserialize(sr);
            sr.Close();
            return rtn;
        }

        public Table FindTable(string tableName)
        {
            foreach (Table table in Tables)
            {
                if (table.Name == tableName)
                    return table;
            }
            return null;
        }


        /// <summary>
        /// 找到系统的所有外键影射
        /// 存储方式 表名 - 〉外键列表
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, List<ForeignKeyMap>> GetForeignKeyMaps()
        {
            Dictionary<string, List<ForeignKeyMap>> rtn = new Dictionary<string, List<ForeignKeyMap>>();
            foreach (Table table in _Tables)
            {
                List<ForeignKeyMap> wmap = new List<ForeignKeyMap>();
                foreach (Table reftable in _Tables)
                {
                    foreach (Col refCol in reftable.Cols)
                    {
                        if (refCol.RefTable == table.Name)
                            wmap.Add(new ForeignKeyMap(reftable, refCol));
                    }
                }
                rtn.Add(table.Name, wmap);
            }
            return rtn;
        }
        /// <summary>
        /// 找到某个表的所有外键影射
        /// </summary>
        /// <param name="table"></param>
        /// <returns></returns>
        public List<ForeignKeyMap> GetForeignKeyMaps(Table table)
        {
            List<ForeignKeyMap> wmap = new List<ForeignKeyMap>();
            foreach (Table reftable in _Tables)
            {
                foreach (Col refCol in reftable.Cols)
                {
                    if (refCol.RefTable == table.Name)
                        wmap.Add(new ForeignKeyMap(reftable, refCol));
                }
            }
            return wmap;

        }

    }



}
