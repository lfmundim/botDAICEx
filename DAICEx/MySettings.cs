using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Takenet.Mpa.MpaTranslateLime;

namespace DAICEx
{
    [DataContract]
    public class MySettings
    {
        public string settings1 { get; set; }
        [DataMember(Name = "mpa")]
        public MPASettings mpaSettings { get; set; }
    }
}
