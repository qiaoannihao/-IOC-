using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleIocDemo
{
    public class School
    {
        public int Index { get; set; }
        public string Name { get; set; }

        public Class Class { get; set; }

        public School(Class @class)
        {
            this.Class = @class;
        }
    }
}
