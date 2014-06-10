using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder.Entity
{
    /// <summary>
    /// 此结构表示一个外键影射的列和他对应的表
    /// </summary>
    public struct ForeignKeyMap
    {
        public ForeignKeyMap(Table nTable, Col nCol)
        {
            Table = nTable;
            Col = nCol;
        }
        public Table Table;
        public Col Col;
    }
}
