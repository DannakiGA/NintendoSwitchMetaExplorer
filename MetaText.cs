using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaImageEditor
{
    class MetaText
    {
        public string Meta;
        string path;

        public string Path
        {
            get
            {
                return path;
            }
        }

        public MetaText(string text, string path)
        {
            Meta = text;
            this.path = path;
        }
    }
}
