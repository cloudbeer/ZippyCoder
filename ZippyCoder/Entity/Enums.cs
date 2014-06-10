using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder.Entity
{

    public enum RenderTypes
    {
        /// <summary>
        /// 文本框
        /// </summary>
        TextBox,
        /// <summary>
        /// 隐藏，不输出
        /// </summary>
        Hidden,
        /// <summary>
        /// 多行文本
        /// </summary>
        TextArea,
        /// <summary>
        /// 超文本，如 FckEditor
        /// </summary>
        Html,
        /// <summary>
        /// 下拉框
        /// </summary>
        DropDownList,
        /// <summary>
        /// 复选框
        /// </summary>
        CheckBoxList,
        /// <summary>
        /// 单选框
        /// </summary>
        RadioButtonList,
        /// <summary>
        /// 使用弹出框选择
        /// </summary>
        MassiveSelector,
        /// <summary>
        /// 从url中传输而来
        /// </summary>
        UrlQuery,
        /// <summary>
        /// 文件上传
        /// </summary>
        FileUpload
    }
    public enum CssClassWidth
    {
        w1, w2, w3, w4, w5, w6, w7, w8, w9, w10, w11, w12, w13, w14, w15, w16,
        w17, w18, w19, w20, w21, w22, w23, w24, w25, w26, w27, w28, w29, w30, w31, w32,
        w50, w100
    }

    [Flags]
    public enum UIColTypes
    {
        /// <summary>
        /// 在查询框中显示
        /// </summary>
        Queryable = 1,
        /// <summary>
        /// 可以被排序
        /// </summary>
        Sortable = 2,
        /// <summary>
        /// 可以被编辑
        /// </summary>
        Editable = 4,
        /// <summary>
        /// 在列表中显示
        /// </summary>
        Listable = 8,
        /// <summary>
        /// 在详情中显示
        /// </summary>
        /// 
        Detailable = 16
    }
}
