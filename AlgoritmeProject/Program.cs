using System;
using System.Data.SqlClient;

namespace AlgoritmeProject
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SqlConnection sqlConnection = new SqlConnection("Data Source=mssql.fhict.local;User ID=dbi410994;Password=Test123!;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            sqlConnection.Open();
            //connstring

            // begin algoritme

            // als 1e de lijst met gefixeerde laag naar hoog
            using (SqlCommand command = new SqlCommand("INSERT INTO EindTabelAlgoritme(Docent_id, Taak_id) SELECT DocentID,Taak_id FROM GefixeerdeTaken"))
            {
                command.Connection = sqlConnection;
                command.ExecuteNonQuery();
                Console.WriteLine("Gefixeerde mensen ingedeeld!");
            }
            // Alle taken optellen die gekozen zijn van laag naar hoog, 0 achteraan
            select Taak, count(taak)Aantal from Bekwaamheid group by Taak having count(Taak) > 0 order by Aantal
            // mensen met het aantal voorkeuren van laag naar hoog
             select UserId, count(UserId)Gekozen from Bekwaamheid group by UserId having count(UserId) > 0 order by Gekozen
            //mensen van hoog aantal beschikbare uren naar laag

            //Check of er nog open/ niet ingevulde taken in de lijst staan

            // mensen beschikbaar? zoniet opnieuw beginnen

            //
        }
    }
}