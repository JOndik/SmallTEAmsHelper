using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPAddInTry
{
    public class ListBoxObject : Object
    {
        public virtual string Text { get; set; }
        public virtual object Tag { get; set; }
        public virtual object Object { get; set; }
        public virtual string Name { get; set; }

        public ListBoxObject()
        {
            this.Text = string.Empty;
            this.Tag = null;
            this.Name = string.Empty;
            this.Object = null;
        }

        public ListBoxObject(string Text, string Name, object Tag, object Object)
        {
            this.Text = Text;
            this.Tag = Tag;
            this.Name = Name;
            this.Object = Object;
        }

        public ListBoxObject(object Object)
        {
            this.Text = Object.ToString();
            this.Name = Object.ToString();
            this.Object = Object;
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}
