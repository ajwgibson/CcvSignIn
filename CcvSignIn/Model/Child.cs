﻿using LINQtoCSV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CcvSignIn.Model
{
    public class Child
    {
        [CsvColumn]
        public int Id { get; set; }

        [CsvColumn]
        public string First { get; set; }

        [CsvColumn]
        public string Last { get; set; }

        [CsvColumn]
        public string Room { get; set; }

        [CsvColumn]
        public DateTime? SignedInAt { get; set; }

        [CsvColumn]
        public bool IsNewcomer { get; set; }

        public string Fullname 
        {
            get { return string.Format("{0} {1}", First, Last); }
        }
    }
}
