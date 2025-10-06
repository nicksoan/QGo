using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QGo.App.Models
{
    public sealed class Shortcut
    {
        public string Key { get; set; } = "";
        public string Template { get; set; } = "";
        public override string ToString() => Key;
    }
}
