using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn.Model
{
    public class Room 
    {
        public string Colour { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool IsSelected { get; set; }

        public string DisplayName 
        { 
            get 
            {
                return string.Format("{0}, {1} Room", Title, Colour);
            }
        }
    }
}
