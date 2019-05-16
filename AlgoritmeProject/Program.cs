using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace AlgoritmeProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            List<Taak> alleTaken = new List<Taak>();
            List<Docent> users = new List<Docent>();
            SqlConnection sqlConnection = new SqlConnection("Data Source=mssql.fhict.local;User ID=dbi410994;Password=Test123!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            //connstring
            sqlConnection.Open();
            // begin algoritme

            // als 1e de lijst met gefixeerde laag naar hoog
            using (sqlConnection)
            {
                using (SqlCommand command = new SqlCommand("INSERT INTO EindTabelAlgoritme(Docent_id, Taak_id) SELECT DocentID,Taak_id FROM GefixeerdeTaken"))
                {
                    command.Connection = sqlConnection;
                    command.ExecuteNonQuery();
                    Console.WriteLine("Gefixeerde mensen ingedeeld!");
                }
            }
            // Alle taken optellen die gekozen zijn van laag naar hoog, 0 achteraan

            using (sqlConnection)
            {
                
                using (var command = new SqlCommand("SelecteerAlleinzetten", sqlConnection)
                {
                    CommandType = CommandType.StoredProcedure
                })
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        //var reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            var item = new Docent();
                            item.docentID = (int)reader["DocentID"];
                            item.voorkeuren = OphalenVoorkeuren((int)reader["DocentID"]);
                            users.Add(item);
                        }

                    }
                }
            }

            ScoreBerekenen();

            //    "select Taak, count(taak)Aantal from Bekwaamheid group by Taak having count(Taak) > 0 order by Aantal";
            //// mensen met het aantal voorkeuren van laag naar hoog
            //"select Docent_id, count(Docent_id)Gekozen from Bekwaamheid group by Docent_id having count(Docent_id) > 0 order by Gekozen";
            //mensen van hoog aantal beschikbare uren naar laag

            //Check of er nog open/ niet ingevulde taken in de lijst staan

            // mensen beschikbaar? zoniet opnieuw beginnen

            //

            List<Voorkeur> OphalenVoorkeuren(int docentID)
            {
                List<Voorkeur> voorkeuren = new List<Voorkeur>();
                try
                {
                    using (sqlConnection)
                    {
                        using (SqlCommand cmd = new SqlCommand("SELECT b.TaakID, d.Prioriteit, b.Taak from Bekwaamheid b inner join DocentVoorkeur d ON b.Bekwaam_Id = d.Bekwaamheid_id WHERE d.DocentID = @docentID", sqlConnection))
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

            List<Docent> DocentenOphalen()
            {
                List<Docent> docenten = new List<Docent>();

                using (sqlConnection)
                {
                    using (var command = new SqlCommand("Select * From Docent", sqlConnection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var docent = new Docent();
                                docent.docentID = (int)reader["DocentID"];
                                docent.voorkeuren = OphalenVoorkeuren(docent.docentID);
                                docenten.Add(docent);
                            }
                        }
                    }
                }
                return docenten;
            }

            void ScoreBerekenen()
            {
                foreach (var docent in DocentenOphalen())
                {
                    int aantaltaken = docent.aantalkeuzes;

                    foreach (var voorkeur in docent.voorkeuren)
                    {
                        int prioriteit = voorkeur.Prioriteit;

                        if (aantaltaken == 1)
                        {
                            voorkeur.Score = (100 - (5 * prioriteit) * 0.5);
                            WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score);
                        }
                        else if (prioriteit == 1)
                        {
                            voorkeur.Score = (100 - (5 * aantaltaken) * 0.5);
                            WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score);
                        }
                        else if (prioriteit > 1 && aantaltaken > 1)
                        {
                            voorkeur.Score = (100 - (5 * aantaltaken * prioriteit) * 0.5);
                            WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score);
                        }
                        else if (aantaltaken == 1 && prioriteit == 1)
                        {
                            voorkeur.Score = 100;
                            WriteResult(docent.docentID, voorkeur.Prioriteit, docent.aantalkeuzes, voorkeur.Score);
                        }
                    }
                }
            }

            void WriteResult(int docentid, int prioriteit, int aantalkeuzes, double score)
            {
                Console.WriteLine(String.Format("Docent id: {0}, Prioriteit: {1}, Aantal taken: {2}, Score: {3}", docentid, prioriteit, aantalkeuzes, score));

            }
        }
    }
}