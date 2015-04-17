using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn.Model
{
    public class Child
    {
        public int Id { get; set; }
        public string First { get; set; }
        public string Last { get; set; }
        public string Room { get; set; }
        public DateTime? SignedInAt { get; set; }
    }
}
