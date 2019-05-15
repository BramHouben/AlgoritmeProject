using System;
using System.Collections.Generic;
using System.Text;

namespace AlgoritmeProject
{
    class Docent
    {
        public int docentID { get; set; }
        public int aantalkeuzes { get; set; }
        public List<Voorkeur> voorkeuren { get; set; }

        public Docent()
        {
            //voorkeuren = ophaalFunctie(); 
            aantalkeuzes = voorkeuren.Count;

        }
    }
}
