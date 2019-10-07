using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;

namespace GeneratorForDatabase
{
    public class Reports
    {
        public static void Report_ByCodProject(int codProject,string typeMeter, SqlCommand sqlCommand) {
            string report = String.Format
                ("select  jReading.IdPoint,IdCS,jReading.CodTP,NumZone,ReadingMeter,DateReading,TimeReading,CodErrorReading,jReading.CodProject,jReading.IdTypeMeter "+
                    "from jReading "+
                    "inner join rPoint "+
                    "on jReading.IdPoint = rPoint.IdPoint "+
                    "inner "+
                    "join rTypeMeter "+
                    "on jReading.idTypeMeter = rTypeMeter.idTypeMeter "+
                    "where jReading.CodErrorReading = 1 and rPoint.CodProject = {0} and  ManufacturerMeter = '{1}'", codProject,typeMeter);

            sqlCommand.CommandText = report;
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            string fileName = String.Format("{0}.{1}.{2} report CodProject No. {3}.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, codProject);

            //StreamWriter tw = File.CreateText(@"C:\Users\Admin\Desktop\report.txt");
            StreamWriter tw = File.CreateText(System.IO.Directory.GetCurrentDirectory() + @"\Reports\" + fileName);
            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("Отчёт из журанала jReading по проекту № {0} для счётчика типа {1}", codProject, typeMeter);

            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("IdPoint  IdCS CodTP  NumZone  ReadingMeter  DateReading     TimeReading  CodErrorReading  CodProject");
            tw.WriteLine("---------------------------------------------------------------------------------------------------");

            int a0 = sqlDataReader.GetOrdinal("IdPoint");
            int a1 = sqlDataReader.GetOrdinal("IdCS");
            int a2 = sqlDataReader.GetOrdinal("CodTP");
            int a3 = sqlDataReader.GetOrdinal("NumZone");
            int a4 = sqlDataReader.GetOrdinal("ReadingMeter");
            int a5 = sqlDataReader.GetOrdinal("DateReading");
            int a6 = sqlDataReader.GetOrdinal("TimeReading");
            int a7 = sqlDataReader.GetOrdinal("CodErrorReading");
            int a8 = sqlDataReader.GetOrdinal("CodProject");

            string jRead = "";

            while (sqlDataReader.Read()) {
                jRead += sqlDataReader.GetInt64(a0) + "\t\t";
                jRead += sqlDataReader.GetValue(a1) + "\t\t";
                jRead += sqlDataReader.GetValue(a2) + "\t\t";
                jRead += sqlDataReader.GetValue(a3) + "\t\t";
                jRead += sqlDataReader.GetValue(a4) + "\t\t";
                jRead += sqlDataReader.GetValue(a5) + "\t\t";
                jRead += sqlDataReader.GetValue(a6) + "\t\t";
                jRead += sqlDataReader.GetValue(a7) + "\t\t";
                jRead += sqlDataReader.GetValue(a8) + "\t\t";
                
                tw.WriteLine(jRead);
                jRead = "";
            }

            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("Report Generate at : " + DateTime.Now);

            sqlDataReader.Close();
            tw.Close();


        }

        public static void Report_ByDate(int days, string typeMeter, SqlCommand sqlCommand)
        {
            string reportDate = String.Format
                    ("select * "+
                    "from jReading "+
                    "inner join rTypeMeter "+
                    "on jReading.idTypeMeter = rTypeMeter.idTypeMeter "+
                    "where jReading.CodErrorReading = 1 and ManufacturerMeter = '{0}' and "+
                    "DateReading >="+
                    "(select dateadd(day, -{1}, (select max(DateReading) from jReading)))",typeMeter, days);



            sqlCommand.CommandText = reportDate;
            SqlDataReader sqlDataReader = sqlCommand.ExecuteReader();

            string fileName = String.Format("{0}.{1}.{2} report days {3}.txt", DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year, days);

            //StreamWriter tw = File.CreateText(@"C:\Users\Admin\Desktop\report.txt");
            StreamWriter tw = File.CreateText(System.IO.Directory.GetCurrentDirectory() + @"\Reports\" + fileName);
            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("Отчёт из журанала jReading за последние {0} суток для счётчика типа {1}", days,typeMeter);

            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("IdPoint  IdCS CodTP  NumZone  ReadingMeter  DateReading\t\t\t\tTimeReading  CodErrorReading");
            tw.WriteLine("---------------------------------------------------------------------------------------------------");

            int a0 = sqlDataReader.GetOrdinal("IdPoint");
            int a1 = sqlDataReader.GetOrdinal("IdCS");
            int a2 = sqlDataReader.GetOrdinal("CodTP");
            int a3 = sqlDataReader.GetOrdinal("NumZone");
            int a4 = sqlDataReader.GetOrdinal("ReadingMeter");
            int a5 = sqlDataReader.GetOrdinal("DateReading");
            int a6 = sqlDataReader.GetOrdinal("TimeReading");
            int a7 = sqlDataReader.GetOrdinal("CodErrorReading");

            string jRead = "";

            while (sqlDataReader.Read())
            {
                jRead += sqlDataReader.GetInt64(a0) + "\t\t";
                jRead += sqlDataReader.GetValue(a1) + "\t\t";
                jRead += sqlDataReader.GetValue(a2) + "\t\t";
                jRead += sqlDataReader.GetValue(a3) + "\t\t";
                jRead += sqlDataReader.GetValue(a4) + "\t\t";
                jRead += sqlDataReader.GetValue(a5) + "\t\t";
                jRead += sqlDataReader.GetValue(a6) + "\t\t";
                jRead += sqlDataReader.GetValue(a7) + "\t\t";

                tw.WriteLine(jRead);
                jRead = "";
            }

            tw.WriteLine("---------------------------------------------------------------------------------------------------");
            tw.WriteLine("Report Generate at : " + DateTime.Now);

            sqlDataReader.Close();
            tw.Close();

        }


    }
}
