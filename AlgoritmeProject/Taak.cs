using System.Collections.Generic;

namespace AlgoritmeProject
{
    public  class Taak
    {
        public int TaakID { get; set; }
        public string TaakNaam { get; set; }
        public int Prioriteit { get; set; }
        public int AantalKeerGekozen { get; set; }
        public int BenodigdeUren { get; set; }
        public int AantalKlassen { get; set; }

        public List<Docent> IngedeeldeDocent { get; set; }
        public Taak()
        {
            IngedeeldeDocent = new List<Docent>();
        }
    }
}