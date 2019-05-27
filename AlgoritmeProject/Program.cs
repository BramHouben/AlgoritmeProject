﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AlgoritmeProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Taak> taken = new List<Taak>();
            List<Docent> users = new List<Docent>();
            SqlConnection sqlConnection = new SqlConnection("Data Source=mssql.fhict.local;User ID=dbi410994;Password=Test123!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            string constring = "Data Source = mssql.fhict.local; User ID = dbi410994; Password = Test123!; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";

            List<Voorkeur> OphalenVoorkeuren(int docentID)
            {
                List<Voorkeur> voorkeuren = new List<Voorkeur>();
                try
                {
                    constring = "Data Source = mssql.fhict.local; User ID = dbi410994; Password = Test123!; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
                    using (SqlConnection con = new SqlConnection(constring))
                    {
                        con.Open();
                        using (SqlCommand cmd = new SqlCommand("SELECT b.TaakID, d.Prioriteit, b.Taak, t.BenodigdeUren from Bekwaamheid b inner join DocentVoorkeur d ON b.Bekwaam_Id = d.Bekwaamheid_id inner join Taak t ON b.TaakID = t.TaakID  WHERE d.DocentID = @docentID", con))
                        {
                            cmd.Parameters.AddWithValue("@docentID", docentID);
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    Voorkeur voorkeur = new Voorkeur();
                                    voorkeur.TaakID = (int)reader["TaakID"];
                                    voorkeur.TaakNaam = (string)reader["Taak"];
                                    voorkeur.Prioriteit = (int)reader["Prioriteit"];
                                    if (DBNull.Value.Equals(reader["BenodigdeUren"]))
                                    {
                                        voorkeur.BenodigdeUren = 0;
                                    }
                                    else
                                    {
                                        voorkeur.BenodigdeUren = (int)reader["BenodigdeUren"];
                                    }
                                    voorkeuren.Add(voorkeur);
                                }
                            }
                        }
                    }
                }
                catch
                {
                    Console.WriteLine("Something went wrong");
                }
                return voorkeuren;
            }

            void Indelen()
            {
                foreach (var taak in TakenOphalen())
                {
                    List<Docent> DocentenScores = new List<Docent>();

                    foreach (var docent in InzetbareDocenten(taak.TaakID))
                    {
                        int prioriteit = PrioriteitOphalen(docent.voorkeuren, taak.TaakID);

                        docent.Score = ScoreBerekenen(docent.InzetbareUren, taak.BenodigdeUren, taak.AantalKeerGekozen, taak.AantalKlassen, prioriteit);

                        DocentenScores.Add(docent);
                    }

                    List<Docent> GesorteerdeDocentenScores = DocentenScores.OrderByDescending(o => o.Score).ToList();

                    Console.WriteLine("-----------------------------------------" + taak.TaakNaam + "  " + taak.TaakID + "----------------------------------------------");
                    foreach (var item in GesorteerdeDocentenScores)
                    {
                        Console.WriteLine(item.docentID + " " + item.Score);
                    }
                }
            }

            int PrioriteitOphalen(List<Voorkeur> voorkeuren, int taakID)
            {
                foreach (var voorkeur in voorkeuren)
                {
                    if (voorkeur.TaakID == taakID)
                    {
                        return voorkeur.Prioriteit;
                    }
                }
                throw new Exception();
            }

            double ScoreBerekenen(int inzetbareUren, int benodigdeUren, int aantalkeergekozen, int aantalklassen, int prioriteit)
            {
                double score = 100;

                score -= (double)inzetbareUren / (double)benodigdeUren * prioriteit;

                return score;
            }

            List<Taak> TakenOphalen()
            {
                taken = new List<Taak>();
                constring = "Data Source = mssql.fhict.local; User ID = dbi410994; Password = Test123!; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (var command = new SqlCommand("Select * From Taak", con))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var taak = new Taak();
                                taak.TaakID = (int)reader["TaakID"];
                                taak.TaakNaam = reader["TaakNaam"].ToString();
                                taak.BenodigdeUren = (int)reader["BenodigdeUren"];
                                taak.AantalKlassen = (int)reader["Aantal_Klassen"];

                                taken.Add(taak);
                            }
                        }
                    }
                }
                return taken;
            }

            List<Docent> InzetbareDocenten(int taakID)
            {
                List<Docent> docenten = new List<Docent>();
                constring = "Data Source = mssql.fhict.local; User ID = dbi410994; Password = Test123!; Connect Timeout = 30; Encrypt = False; TrustServerCertificate = False; ApplicationIntent = ReadWrite; MultiSubnetFailover = False";
                using (SqlConnection con = new SqlConnection(constring))
                {
                    con.Open();
                    using (var command = new SqlCommand("SELECT D.* " +
                                                        "FROM Docent as D " +
                                                        "INNER JOIN Bekwaamheid as B ON D.DocentID = B.Docent_id " +
                                                        "WHERE B.TaakID = @taakID", con))
                    {
                        command.Parameters.AddWithValue("@taakID", taakID);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var docent = new Docent();

                                docent.docentID = (int)reader["DocentId"];
                                docent.InzetbareUren = (int)reader["RuimteVoorInzet"];
                                docent.voorkeuren = OphalenVoorkeuren(docent.docentID);

                                docenten.Add(docent);
                            }
                        }
                    }
                }
                return docenten;
            }

            //void ScoreBerekenen()
            //{
            //    foreach (var docent in DocentenOphalen())
            //    {
            //        int aantaltaken = docent.aantalkeuzes;
            //        if(docent.voorkeuren.Count != 0) {
            //        Console.WriteLine(docent.docentID + "--------------------------------");
            //        }
            //        foreach (var voorkeur in docent.voorkeuren)
            //        {
            //            int prioriteit = voorkeur.Prioriteit;

            //            if (aantaltaken == 1)
            //            {
            //                if(docent.InzetbareUren > voorkeur.BenodigdeUren)
            //                {
            //                    voorkeur.Score = (100 - (5 * prioriteit) * 0.5);
            //                }
            //                else
            //                {
            //                    int verschil = voorkeur.BenodigdeUren - docent.InzetbareUren;
            //                    voorkeur.Score = (100 - (5 * prioriteit) * 0.5) - (verschil / 8);
            //                }
            //                WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score, voorkeur.TaakNaam);
            //            }
            //            else if (prioriteit == 1)
            //            {
            //                if (docent.InzetbareUren > voorkeur.BenodigdeUren)
            //                {
            //                    voorkeur.Score = (100 - (5 * aantaltaken) * 0.5);
            //                }
            //                else
            //                {
            //                    int verschil = voorkeur.BenodigdeUren - docent.InzetbareUren;
            //                    voorkeur.Score = (100 - (5 * aantaltaken) * 0.5) - (verschil / 8);
            //                }
            //                WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score, voorkeur.TaakNaam);
            //            }
            //            else if (prioriteit > 1 && aantaltaken > 1)
            //            {
            //                if(docent.InzetbareUren > voorkeur.BenodigdeUren)
            //                {
            //                    voorkeur.Score = (100 - (5 * aantaltaken * prioriteit) * 0.5);
            //                }
            //                else
            //                {
            //                    int verschil = voorkeur.BenodigdeUren - docent.InzetbareUren;
            //                    voorkeur.Score = (100 - (5 * aantaltaken * prioriteit) * 0.5) - (verschil / 8);
            //                }
            //                WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score, voorkeur.TaakNaam);
            //            }
            //            else if (aantaltaken == 1 && prioriteit == 1)
            //            {
            //                voorkeur.Score = 100;
            //                WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score, voorkeur.TaakNaam);
            //            }
            //        }
            //        if (docent.voorkeuren.Count != 0)
            //        {
            //            Console.WriteLine("---------------------------------");
            //        }
            //    }
            //}
            Indelen();
            void WriteResult(int docentid, int prioriteit, int aantalkeuzes, double score, string Naam)
            {
                Console.WriteLine(String.Format("Docent id: {0}, Taak: {4}, Prioriteit: {1}, Aantal taken: {2}, Score: {3}", docentid, prioriteit, aantalkeuzes, score, Naam));
            }
        }
    }
}