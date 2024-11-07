using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bound.Models
{
    public class SettingsModel
    {
        public Dictionary<string, string> General;

        public Dictionary<string, string> InputValues;

        public override string ToString()
        {
            var str = "";

            foreach (var kvp in General)
                str += (kvp.Key + ": " + kvp.Value + "\n");
            
            str += "\n";

            foreach (var kvp in InputValues)
                str += (kvp.Key + ": " + kvp.Value  + "\n");

            return str;
        }
    }
}
