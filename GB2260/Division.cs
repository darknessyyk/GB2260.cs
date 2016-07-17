using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GB2260
{
    public class Division
    {
        public string Revision { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Province { get; set; }
        public string Prefecture { get; set; }
        public string Description { get { return ToString(); } }

        public Division(string code, string name, string revision, string prefecture = null, string province = null)
        {
            Code = code;
            Name = name;
            Revision = revision;
            Prefecture = prefecture;
            Province = province;
        }
        public override string ToString()
        {
            return (Province == null ? "" : Province + " ") + (Prefecture == null ? "" : Prefecture + " ") + (Name == null ? "" : Name);
        }
    }
}
