using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZippyCoder
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PluginIndicatorAttribute : Attribute
    {
        protected string title;
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
    }
}
