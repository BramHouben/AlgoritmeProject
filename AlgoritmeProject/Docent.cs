﻿using System;
using System.Collections.Generic;
using System.Text;

namespace AlgoritmeProject
{
   public class Docent
    {
        public int docentID { get; set; }
        public int aantalkeuzes { get; set; }
        public List<Voorkeur> voorkeuren { get; set; }
        public int InzetbareUren { get; set; }
        public double Score { get; set; }

        public Docent()
        {
            voorkeuren = new List<Voorkeur>();
            aantalkeuzes = voorkeuren.Count;

        }
    }
}
