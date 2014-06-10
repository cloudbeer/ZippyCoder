using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyWPFForm
{
    public class TTSetting
    {
        //public string MyFlag { get; set; }
        /// <summary>
        /// 存储路径
        /// </summary>
        public string SaveingPath { get; set; }
        /// <summary>
        /// 文件名的模板
        /// </summary>
        public string FileNamePattern { get; set; }
        /// <summary>
        /// 需要去掉的文件前缀
        /// </summary>
        public string FixDelete { get; set; }
        /// <summary>
        /// 是否每个文件都输出一个目录
        /// </summary>
        public bool MultiDir { get; set; }

        public TTSetting()
        {
            SaveingPath = "c:\\";
            FileNamePattern = "{0}";
        }

        public void Save(string path)
        {
            System.IO.FileInfo fiPath = new System.IO.FileInfo(path);

            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(this.GetType());
            System.IO.StreamWriter sw = new System.IO.StreamWriter(path, false, System.Text.Encoding.Default);
            xser.Serialize(sw, this);
            sw.Close();

        }

        public static TTSetting Load(string path)
        {
            System.Xml.Serialization.XmlSerializer xser = new System.Xml.Serialization.XmlSerializer(typeof(TTSetting));
            System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.Default);
            TTSetting rtn = (TTSetting)xser.Deserialize(sr);
            sr.Close();
            return rtn;
        }

    }
}
