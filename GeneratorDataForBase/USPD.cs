using GeneratorDataForBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GeneratorForDatabase
{
    public class USPD
    {

        int IdCS, CodeErrorCS, NumReadings, flatsForUSPD;
        string DateCS, TimeCS;
        DateTime date_time;
        int CodTP, NumZone;
        int codProject;
        int idTypeMeter;

        Random rand = new Random();

        ElectricMeter[] electricMeters;

        public USPD()
        {
        }

        public void SetDateTime(DateTime dateTimeStart)
        {
            date_time = dateTimeStart;

            DateCS = "'" + date_time.Year + "-" + date_time.Month + "-" + date_time.Day + "'";
            TimeCS = "'" + date_time.Hour + ":" + date_time.Minute + "'";
        }

        public void DoCS(int id,int codError, int procentOtkzaMeter, SqlCommand sqlCommand)
        {
            IdCS = id;
            CodeErrorCS = codError;

            if (codError == 0)
            {NumReadings = flatsForUSPD;}
            else { NumReadings = 0; }

            sqlCommand.CommandText = "INSERT jCS VALUES (" + DateCS + "," + TimeCS + "," + CodeErrorCS + "," + NumReadings + ")";
            sqlCommand.ExecuteNonQuery();

            if (codError == 0)// удачный сеанс связи
            {
                for (int f = 1; f <= flatsForUSPD; f++)
                {
                    //счётчик опросился успешно
                    if (MyRandom.GetRandomEvent(rand, procentOtkzaMeter) == false)
                    { 
                            CodeErrorCS = 0;
                            electricMeters[f].DoMeterOpros(CodeErrorCS, IdCS, date_time, sqlCommand);                 
                    }
                    else//счётчик опрошен неудачно
                    {
                        CodeErrorCS = 1;
                        electricMeters[f].DoMeterOpros(CodeErrorCS, IdCS, date_time, sqlCommand);  
                    }
                }

            }
            else//сеанс связи неудался, данные записались в память
            {
                for (int f = 1; f <= flatsForUSPD; f++)
                {               
                        CodeErrorCS = 1;
                        electricMeters[f].DoMeterOpros(CodeErrorCS, IdCS, date_time, sqlCommand);
                }
            }
            IdCS++;
        }



        //заполнение массива счётчиков
        public void FillElectricMetersInUSPD(int countFlatsForUSPD,DateTime dateTime, int idFirst, int meanDay, int stdDevDay,
                                                                                        int meanNight, int stdDevNight, int codPrjct,int idTypeMeter)
        {    

            codProject = codPrjct;
            electricMeters = new ElectricMeter[countFlatsForUSPD + 1];
            date_time = dateTime;
            flatsForUSPD = countFlatsForUSPD;
       

            for (int p = 1; p <= countFlatsForUSPD; p++)
            {
                CodTP = 22; 
                ElectricMeter meter = new ElectricMeter(idFirst, CodTP, meanDay, stdDevDay,meanNight,stdDevNight, codPrjct, idTypeMeter);
                electricMeters[p] = meter;
                idFirst++;
            }
        }

        


    }

}
