using GeneratorDataForBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace GeneratorForDatabase
{
    public class ElectricMeter
    {
        private DateTime date_time;

        private DateTime[] date_timeArray = new DateTime[60];
        private int[] meterArray = new int[60];
        private int[] csArray = new int[60];
        private int[] codErrorArray = new int[60];
        private int[] numZoneArray = new int[60];

        private int meterValueDay, meterValueNight, counter, idPoint, meanDay, stdDevDay, meanNight, stdDevNight;
        private string dateReading, timeReading;

        private int codTP, numZone, codProj, idTypeMeter;

        private static Random rand = new Random(DateTime.Now.Millisecond);


        public ElectricMeter(int id,  int cod_TP, int meanOprosDay, int stdDevOprosDay, int meanOprosNight, int stdDevOprosNight, int codProject, int idTypeM)
        {
            idPoint = id;
            codTP = cod_TP;
            codProj = codProject;

            
            idTypeMeter = rand.Next(1,100);

            meanDay = meanOprosDay;
            stdDevDay = stdDevOprosDay;

            meanNight = meanOprosNight;
            stdDevNight = stdDevOprosNight;
        }


        public void DoMeterOpros(int codError, int idCS, DateTime dateTime, SqlCommand insertCommand)
        {
            date_time = dateTime;

            dateReading = "'" + date_time.Year + "-" + date_time.Month + "-" + date_time.Day + "'";
            timeReading = "'" + date_time.Hour + ":" + date_time.Minute + "'";

            meterValueDay = MyRandom.GetGaussRandomValue(meanDay, stdDevDay);
            meterValueNight = MyRandom.GetGaussRandomValue(meanNight, stdDevNight);

            if (codError == 0)
            {
                //Успех. Текущие данные записаны.
                
                numZone = 1;
                insertCommand.CommandText = "INSERT jReading VALUES (" + idPoint + "," + idCS + "," + codTP + ","
                    + numZone + "," + meterValueDay + "," + dateReading + "," + timeReading + "," + codError + "," + codProj +","+ idTypeMeter + ")";
                insertCommand.ExecuteNonQuery();

                numZone = 0;
                insertCommand.CommandText = "INSERT jReading VALUES (" + idPoint + "," + idCS + "," + codTP + ","
                    + numZone + "," + meterValueNight + "," + dateReading + "," + timeReading + "," + codError + "," + codProj + "," + idTypeMeter + ")";
                insertCommand.ExecuteNonQuery();

                if (counter > 0)
                {
                    //Данные из памяти
                    for (int a = 0; a < counter; a++)
                    {
                        
                        dateReading = "'" + date_timeArray[a].Year + "-" + date_timeArray[a].Month + "-" + date_timeArray[a].Day + "'";
                        timeReading = "'" + date_timeArray[a].Hour + ":" + date_timeArray[a].Minute + "'";

                        insertCommand.CommandText = "INSERT jReading VALUES (" + idPoint + "," + csArray[a] + "," + codTP + ","
                            + numZoneArray[a] + "," + meterArray[a] + "," + dateReading + "," + timeReading + "," + codErrorArray[counter] + "," + codProj + "," + idTypeMeter + ")";
                        insertCommand.ExecuteNonQuery();
                    }
                    counter = 0;
                }

            }
            else
            {
                if (counter < 60)
                {

                    int error = 2;
                    numZone = 1;
                    meterArray[counter] = meterValueDay;
                    date_timeArray[counter] = date_time;
                    csArray[counter] = idCS;
                    codErrorArray[counter] = error;
                    numZoneArray[counter] = numZone;
                    counter++;

                    numZone = 0;
                    meterArray[counter] = meterValueNight;
                    date_timeArray[counter] = date_time;
                    csArray[counter] = idCS;
                    codErrorArray[counter] = error;
                    numZoneArray[counter] = numZone;
                    counter++;

                    meterValueDay = 0;
                    meterValueNight = 0;

                    numZone = 1;
                    insertCommand.CommandText = "INSERT jReading VALUES (" + idPoint + "," + idCS + "," + codTP + ","
                        + numZone + "," + meterValueDay + "," + dateReading + "," + timeReading + "," + codError + "," + codProj + "," + idTypeMeter + ")";
                    insertCommand.ExecuteNonQuery();

                    numZone = 0;
                    insertCommand.CommandText = "INSERT jReading VALUES (" + idPoint + "," + idCS + "," + codTP + ","
                        + numZone + "," + meterValueNight + "," + dateReading + "," + timeReading + "," + codError + "," + codProj + "," + idTypeMeter + ")";
                    insertCommand.ExecuteNonQuery();
                    //Опрос неудался. Данные добавлены в память счётчика.
                }
                else
                {
                    //Память счётчика заполнена!
                }

            }
            
        }
    }
}
