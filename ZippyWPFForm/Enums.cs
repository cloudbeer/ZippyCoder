using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyWPFForm
{
    public enum Nodes
    {
        None,
        Col,
        Table,
        Project
    }

    public enum ChangeTypes
    {
        None,
        CreateProject,
        AlterProject,
        AddTable,
        AlterTable,
        DropTable,
        AddCol,
        AlterCol,
        DropCol
    }

}
