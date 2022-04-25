using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MetaImageEditor
{
    class CreateFilename
    {
        int position = 0;
        public string ReturnFilename(string text)
        {
            string result;
            position = text.IndexOf(".");
            result = text.Remove(position, 4).Remove(0, 4);
            while (result.Contains("\\"))
            {
                position = text.IndexOf("\\");
                result = result.Substring(position - 1);
            }
            return result;
        }
    }
}
