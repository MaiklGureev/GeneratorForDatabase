using GeneratorForDatabase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace GeneratorDataForBase
{
    public partial class GeneratorData : Form
    {

        static SqlConnection connection = AuthorizationForm.GetSqlConnection();
        SqlConnectionStringBuilder sqlCSB = new SqlConnectionStringBuilder();

        Random rand = new Random();
        SqlCommand sqlCommand = connection.CreateCommand();

        string date;
        string time;

        DateTime date1;
        DateTime date2;

        int addToTime;

        int valueRadioButton = 3;

        int countOfPeople;
        float avgCountPeolpe;
        float procentJitel4Kv, procentJitel8Kv, procentJitel16Kv, procentJitel50Kv, procentJitel100Kv, procentJitel200Kv;
        int countFlat4Kv, countFlat8Kv, countFlat16Kv, countFlat50Kv, countFlat100Kv, countFlat200Kv;
        int countHouses4Kv, countHouses8Kv, countHouses16Kv, countHouses50Kv, countHouses100Kv, countHouses200Kv;

        float procMercuri, procEnergomera, procMayak, procMatrica;

        int countHouses, countFlats, meanDay, stdDevDay, meanNight, stdDevNight, counterForHouses;

        //названия производителей счётчиков
        string nameManufacturer1 = "'Меркурий'";
        string nameManufacturer2 = "'Энергомера'";
        string nameManufacturer3 = "'Маяк'";
        string nameManufacturer4 = "'Матрица'";

        //названия типов счётчиков
        string nameTypeMeter1;
        string nameTypeMeter2;
        string nameTypeMeter3;
        string nameTypeMeter4;

        int countCS, procentOtkzaUSPD, procentOtkzaMeter, countDays;

        ElectricMeter[] electricMeters;
        USPD[] uSPDs;

        //очистка БД
        String clearCom =
           "delete from jReading;DBCC CHECKIDENT('jReading', RESEED, 0);" +
           "delete from jCHANGE_METER1;DBCC CHECKIDENT('jCHANGE_METER1', RESEED, 0);" +
           "delete from jCHANGE_USPD;DBCC CHECKIDENT('jCHANGE_USPD', RESEED, 0);" +
           "delete from sFlat;DBCC CHECKIDENT('sFlat', RESEED, 0);" +
           "delete from rTypeMeter;DBCC CHECKIDENT('rTypeMeter', RESEED, 0);" +
           "delete from sHouse;DBCC CHECKIDENT('sHouse', RESEED, 0);" +
           "delete from rPoint;DBCC CHECKIDENT('rPoint', RESEED, 0);" +
           "delete from jCS;DBCC CHECKIDENT('jCS', RESEED, 0);";

        string rebIndexCom = "alter index nci on jReading REBUILD;"+ 
        "alter index nci2 on jReading REBUILD;"+
        
        "alter index ncci_DATA_READING on jReading REBUILD;"+
        "alter index NCI3_TYPE_METER on rTypeMeter REBUILD;"+
        "alter index ncci_rpoint on rPoint REBUILD; ";

        //sHouse
        int NumFlat, IdStreet, IdTown, NumUSPD;

        //rPoint
        int DateStart, DateFinish, TypePoint, IdLastChangeMeter1, IdLastChangeMeter3, IdLastChangeUSPD, IdLastChangeUSPD1, CodOtdel, CodProject;

        //кнопка выйти
        private void exitButton_Click(object sender, EventArgs e)
        {
            AuthorizationForm.CloseSqlConnection();
            Application.Exit();
        }

        private void rebuildIndexButton_Click(object sender, EventArgs e)
        {
            try
            {
                SqlCommand clearCommand = connection.CreateCommand();
                clearCommand.CommandText = rebIndexCom;
                clearCommand.ExecuteNonQuery();

                System.Threading.Thread.Sleep(300);
                MessageBox.Show("Индексы перестроены!!!");
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Ошибка при перестроении индексов. \n" + ex.Message);
            }
        }


        //обновление инфо панели справа
        private void updateInfoPanel(object sender, EventArgs e)
        {
            CountParameters();
        }

        //Открыть папку с отчётами
        private void openFolderButton_Click(object sender, EventArgs e)
        {
            Process.Start("explorer.exe", System.IO.Directory.GetCurrentDirectory()+ @"\Reports\" );
            //+(@"..\..\Reports\")
        }

        string DateLastReading, TimeLastReading;
       //sFlat
        int IdPoint, IdHouse, Numer;
        string Comment;

        //rTypeMeter
        int IdTypeMeter;
        string ManufacturerMeter, NameTypeMeter;

        //jCHANGE_METER1
        int IdChangeMeterBefore, PhusikalnumberMeter1, CodetypeMeter1,
           NumRequest, TypeChange, CodContract, CodWorker, CodChief, CodTP, IdReading, IdReadingbefore;
        string DateLastGospoverkaMeter1;

        //jCHANGE_USPD
        int CodNaryada, IdNaryada, TypeOfWork, IdTypeUSPD, IdTypeUSPDnew;

        //jCS
        int CodeErrorCS, NumReadings;
        //IdCS
        string DateCS, TimeCS;

        //jReading
        int IdCS, NumZone, CodErrorReading, ReadingMeter;
        //IdPoint,DateCS,TimeCS,IdCS,IdReading,CodTP
        string DateReading, TimeReading;



        //кнопка генерации опроса
        private void generateOprosButton_Click(object sender, EventArgs e)
        {
            try
            {
                
                SetValuesData();

                CountParameters();

                FillUSPDs();

                InsertjReadingAndjCS();

                Thread.Sleep(300);               
    
                MessageBox.Show("Данные опроса сгенерированы!!!");
            }
            catch {
                
                MessageBox.Show("Не удалось сгенерировать данные опроса!!!");
            }
           
        }

        //проверка радиобаттон для отчёта
        private void radiobuttons_CheckedChanged(object sender, EventArgs e)
        {
            

            if (val3.Checked == true)
            {valueRadioButton = 3;}
            else if (val7.Checked == true)
            { valueRadioButton = 7;}
            else if (val30.Checked == true)
            {valueRadioButton = 30;}

         
        }

        //кнопка генерации отчёта по дневному интервалу 
        private void reportIntervalData_Click(object sender, EventArgs e)
        {
            try
            {
                string typeMeter = typeMeterTextBox2.Text;
                Reports.Report_ByDate(valueRadioButton,typeMeter, sqlCommand);
                string a = String.Format("Отчёт за последние {0} суток создан!!!", valueRadioButton.ToString());
                MessageBox.Show(a);
            }
            catch
            {
                MessageBox.Show("Ошибка при создании отчёта!!!");
            }
           
        }

        // кнопка генерации отчёта по коду проекта
        private void reportCodProject_Click(object sender, EventArgs e)
        {
            //try
            {
                int codProject = int.Parse(codProjectBox.Text);
                string typeMeter = typeMeterTextBox1.Text;
                Reports.Report_ByCodProject(codProject,typeMeter, sqlCommand);
                MessageBox.Show("Отчёт создан!!!");
            }
            //catch {
              //  MessageBox.Show("Ошибка при создании отчёта!!!");
           // }
            
        }

        //кнопка генерации справочных данных
        private void generateDataButton_Click(object sender, EventArgs e)
        {
            //try
            {
                SetValuesDataMeter();
                InsertTypeMeter();
                //System.Threading.Thread.Sleep(300);

                SetValuesData();
                CountParameters();
                InsertHouses();
                InsertPointsFlatsChangeMeter1ALL();
                InsertChangeUSPDs();

                Thread.Sleep(300);
                MessageBox.Show("Данные сгенерированы!!!");
            }
            
            //catch (SqlException ex)
            {
                //MessageBox.Show("Не удалось сгенерировать справочные данные!!!\n" + ex.Message);
            }
        }

        // кнопка очистки БД
        private void clearButton_Click(object sender, EventArgs e)
        {
            ClearDataBase();
        }

        //персчёт колличества дней при изменении параметров
        private void DataChanged(object sender, EventArgs e)
        {
            CountDays();
        }

        //установка заданной даты
        void SetDateTime()
        {
            date = "'" + date1.Year + "-" + date1.Month + "-" + date1.Day + "'";
            time = "'" + date1.Hour + ":" + date1.Minute + "'";

        }

        //добавление домов заданного типа
        void InsertHouse(int countHoses, int countFlatsInHouse)
        {
            for (int i = 1; i <= countHoses; i++)
            {
                NumFlat = countFlatsInHouse;
                IdStreet = 1;
                IdTown = 1;
                NumUSPD = 1;

                Random rand = new Random();
                SqlCommand insertCommand = connection.CreateCommand();

                insertCommand.CommandText = "INSERT sHouse VALUES (" + NumFlat + "," + IdStreet + "," + IdTown + "," + NumUSPD + ");";
                insertCommand.ExecuteNonQuery();
            }
        }

        //добавление всех домов 
        void InsertHouses()
        {
            IdHouse = 1;
            CodProject = 100;

            //houses4
            InsertHouse(countHouses4Kv, 4);

            //houses8
            InsertHouse(countHouses8Kv, 8);

            //houses16
            InsertHouse(countHouses16Kv, 16);

            //houses50
            InsertHouse(countHouses50Kv, 50);

            //houses100
            InsertHouse(countHouses100Kv, 100);

            //houses200
            InsertHouse(countHouses200Kv, 200);

        }

        //добавление точек учёта и квартир заданного типа дома
        void InsertPointsFlatsChangeMeter(int countFlat, int flatsInHouse)
        {
            //rPoint
            TypePoint = rand.Next(5);
            //rFlat
            Numer = 1;
            SetDateTime();
            //jChange_Meter1
            CodContract = rand.Next(1, 50);
            CodWorker = rand.Next(1, 100);
            CodChief = rand.Next(1, 10);
            DateLastGospoverkaMeter1 = date;

            for (int p = 1; p <= countFlat; p++)
            {
                TypePoint = 1;
                IdLastChangeMeter3 = 0;
                IdLastChangeUSPD = 0;
                IdLastChangeUSPD1 = 0;
                CodOtdel = rand.Next(100);

                IdChangeMeterBefore = 0;
                IdReading = 0;
                IdReadingbefore = 0;
                TypeChange = 1;
                IdTypeMeter = CodetypeMeter1 = rand.Next(1, 100);
                CodTP = rand.Next(0, 5);


                sqlCommand.CommandText = "INSERT rPoint VALUES (" + date + ",null," + TypePoint + "," + IdLastChangeMeter1 + ","
                    + IdLastChangeMeter3 + "," + IdLastChangeUSPD + "," + IdLastChangeUSPD1 + ","
                    + date + "," + time + "," + CodOtdel + "," + CodProject + ")";
                sqlCommand.ExecuteNonQuery();

                sqlCommand.CommandText = "INSERT sFlat VALUES (" + IdPoint + "," + IdHouse + "," + Numer + "," + "'kvartira'" + ")";
                sqlCommand.ExecuteNonQuery();

                sqlCommand.CommandText = "INSERT jCHANGE_METER1 VALUES (" + IdPoint + "," + IdTypeMeter + "," + IdChangeMeterBefore
                    + "," + PhusikalnumberMeter1 + "," + CodetypeMeter1 + "," + DateLastGospoverkaMeter1 + "," + NumRequest + ","
                    + TypeChange + "," + CodContract + "," + CodWorker + "," + CodChief + "," + CodTP + "," + IdReading + "," + IdReadingbefore + ")";
                sqlCommand.ExecuteNonQuery();

                IdPoint++;

                if (Numer == flatsInHouse) { Numer = 0; IdHouse++; }

                PhusikalnumberMeter1++;
                IdLastChangeMeter1++;
                Numer++;

            }
            CodProject++;
        }

        //добавление всех точек учёта и квартир
        void InsertPointsFlatsChangeMeter1ALL()
        {

            IdPoint = 1;
            IdHouse = 1;
            IdLastChangeMeter1 = 1;
            PhusikalnumberMeter1 = 1;

            //points and flats4
            InsertPointsFlatsChangeMeter(countFlat4Kv, 4);

            //points and flats8
            InsertPointsFlatsChangeMeter(countFlat8Kv, 8);

            //points and flats16
            InsertPointsFlatsChangeMeter(countFlat16Kv, 16);

            //points and flats50
            InsertPointsFlatsChangeMeter(countFlat50Kv, 50);

            //points and flats100
            InsertPointsFlatsChangeMeter(countFlat100Kv, 100);

            //points and flats200
            InsertPointsFlatsChangeMeter(countFlat200Kv, 200);

        }
      
        //заполнения массива УСПД
        void FillUSPDs()
        {
            //начало и конец idPoint для 4х квартирых домов
            int fId4kv = 1;
            int lId4kv = fId4kv + countFlat4Kv - 1;

            //начало и конец idPoint для 8 квартирых домов
            int fId8kv = lId4kv + 1;
            int lId8kv = fId8kv + countFlat8Kv - 1;

            //начало и конец idPoint для 16 квартирых домов
            int fId16kv = lId8kv + 1;
            int lId16kv = fId16kv + countFlat16Kv - 1;

            //начало и конец idPoint для 50 квартирых домов
            int fId50kv = lId16kv + 1;
            int lId50kv = fId50kv + countFlat50Kv - 1;

            //начало и конец idPoint для 100 квартирых домов
            int fId100kv = lId50kv + 1;
            int lId100kv = fId100kv + countFlat100Kv - 1;

            //начало и конец idPoint для 200 квартирых домов
            int fId200kv = lId100kv + 1;
            int lId200kv = fId200kv + countFlat200Kv - 1;

            
            uSPDs = new USPD[countHouses + 1];
            for (int p = 1; p <= countHouses; p++)
            {
                
                USPD uspd = new USPD();
                uSPDs[p] = uspd;
                
            }

            counterForHouses = 1;
            CodProject = 100;

            PointsToUSPD(countHouses4Kv, countFlat4Kv, 4, fId4kv, lId4kv);
            PointsToUSPD(countHouses8Kv, countFlat8Kv, 8, fId8kv, lId8kv);
            PointsToUSPD(countHouses16Kv, countFlat16Kv, 16, fId16kv, lId16kv);
            PointsToUSPD(countHouses50Kv, countFlat50Kv, 50, fId50kv, lId50kv);
            PointsToUSPD(countHouses100Kv, countFlat100Kv, 100, fId100kv, lId100kv);
            PointsToUSPD(countHouses200Kv, countFlat200Kv, 200, fId200kv, lId200kv);

        }

        //заполнение УСПД соответствующего типу дома
        void PointsToUSPD(int countHoses, int countFlat, int flatsInHouse, int idFirst, int idLast)
        {
            int idTypeM = rand.Next(1,100);
            for (int i = 1; i <= countHoses; i++)
            {

                if (flatsInHouse > countFlat)
                {
                    flatsInHouse = idLast - idFirst + 1;
                }
                
                uSPDs[counterForHouses].FillElectricMetersInUSPD(flatsInHouse,date1, idFirst,meanDay,stdDevDay,meanNight,stdDevNight,CodProject,idTypeM);
                idFirst = idFirst + flatsInHouse - 1;
                countFlat -= flatsInHouse;
                counterForHouses++;
            }
            CodProject++;
        }

        //добавление всех сеансов связи и показаний по счётчикам и УСПД
        void InsertjReadingAndjCS()
        {
            DateTime date = date1;
            DateTime dateForCS = date1;

            IdCS = 1;

            for (int d = 0; d < countDays; d++)
            {
                
                for (int h = 1; h <= countHouses; h++)
                {

                    uSPDs[h].SetDateTime(date);

                   
                        //УСПД запрос успешно
                        if (MyRandom.GetRandomEvent(rand, procentOtkzaUSPD) == false)
                        {
                            CodeErrorCS = 0;
                            uSPDs[h].DoCS(IdCS,CodeErrorCS, procentOtkzaMeter, sqlCommand);

                        }
                        else//УСПД запрос неудачный
                        {
                            CodeErrorCS = 1;
                            uSPDs[h].DoCS(IdCS,CodeErrorCS, procentOtkzaMeter,  sqlCommand);

                        }
                    IdCS++;
                } //конец домов
                date = date.AddDays(1);
            }//конец дней


        }

        //добавление типов счёчиков
        void InsertTypeMeter()
        {

            int countMerc = (int)Math.Round(100 * 0.01 * procMercuri);
            int countEnerg = (int)Math.Round(100 * 0.01 * procEnergomera);
            int countMayak = (int)Math.Round(100 * 0.01 * procMayak);
            int countMatric = (int)Math.Round(100 * 0.01 * procMatrica);

            //mercury
            for (int m = 1; m <= countMerc; m++)
            {
                int numType1 = 1;
                nameTypeMeter1 = "'AAA-" + numType1 + "'";
                sqlCommand.CommandText = "INSERT rTypeMeter VALUES (" + nameManufacturer1 + "," + nameTypeMeter1 + ")";
                sqlCommand.ExecuteNonQuery();
                numType1++;
            }
            //eneromera
            for (int m = 1; m <= countEnerg; m++)
            {
                int numType2 = 1;
                nameTypeMeter2 = "'BBB-" + numType2 + "'";
                sqlCommand.CommandText = "INSERT rTypeMeter VALUES (" + nameManufacturer2 + "," + nameTypeMeter2 + ")";
                sqlCommand.ExecuteNonQuery();
                numType2++;
            }

            //mayak
            for (int m = 1; m <= countMayak; m++)
            {
                int numType3 = 1;
                nameTypeMeter3 = "'CCC-" + numType3 + "'";
                sqlCommand.CommandText = "INSERT rTypeMeter VALUES (" + nameManufacturer3 + "," + nameTypeMeter3 + ")";
                sqlCommand.ExecuteNonQuery();
                numType3++;
            }

            //matrica
            for (int m = 1; m <= countMatric; m++)
            {
                int numType4 = 1;
                nameTypeMeter4 = "'DDD-" + numType4 + "'";
                sqlCommand.CommandText = "INSERT rTypeMeter VALUES (" + nameManufacturer4 + "," + nameTypeMeter4 + ")";
                sqlCommand.ExecuteNonQuery();
                numType4++;
            }

        }

        //добавление замен УСПД в дом заданного типа
        void InsertChangeUSPD(int countHouseN_Kv, int countFlatsInHous)
        {
            CodChief = rand.Next(10);
            IdTypeUSPD = 0;
            IdTypeUSPDnew = rand.Next(100);

            for (int p = 1; p <= countHouseN_Kv; p++)
            {

                CodNaryada = rand.Next(10);
                IdNaryada = rand.Next(500);


                sqlCommand.CommandText = "INSERT jCHANGE_USPD VALUES (" + IdHouse + "," + CodNaryada + ","
                    + IdNaryada + "," + TypeOfWork + "," + CodChief + "," + IdTypeUSPD + "," + IdTypeUSPDnew + ")";
                sqlCommand.ExecuteNonQuery();

                if (Numer == countFlatsInHous) { Numer = 0; IdHouse++; }

                Numer++;
            }
        }

        //добавление всех замен УСПД
        void InsertChangeUSPDs()
        {

            //jCHANGE_USPD
            CodNaryada = 1;
            IdNaryada = 1;
            TypeOfWork = 1;
            IdTypeUSPD = 1;
            IdTypeUSPDnew = 1;
            TypeOfWork = 22;
            IdHouse = 1;

            //jCHANGE_USPD_4
            InsertChangeUSPD(countHouses4Kv, 4);

            //jCHANGE_USPD_8
            InsertChangeUSPD(countHouses8Kv, 8);

            //jCHANGE_USPD_16
            InsertChangeUSPD(countHouses16Kv, 16);

            //jCHANGE_USPD_50
            InsertChangeUSPD(countHouses50Kv, 50);

            //jCHANGE_USPD_100
            InsertChangeUSPD(countHouses100Kv, 100);

            //jCHANGE_USPD_200
            InsertChangeUSPD(countHouses200Kv, 200);
        }

        //расчёт колличества квартир из заданного соотношения
        void CountParameters()
        {
            
            try
            {
                SetValuesData();

                //countFlats = (int)Math.Ceiling(countOfPeople / avgCountPeolpe);
                countFlat4Kv = (int)Math.Ceiling(countOfPeople * procentJitel4Kv * 0.01 / avgCountPeolpe);
                countHouses4Kv = (int)Math.Ceiling((double)countFlat4Kv / 4);

                countFlat8Kv = (int)Math.Ceiling(countOfPeople * procentJitel8Kv * 0.01 / avgCountPeolpe);
                countHouses8Kv = (int)Math.Ceiling((double)countFlat8Kv / 8);

                countFlat16Kv = (int)Math.Ceiling(countOfPeople * procentJitel16Kv * 0.01 / avgCountPeolpe);
                countHouses16Kv = (int)Math.Ceiling((double)countFlat16Kv / 16);

                countFlat50Kv = (int)Math.Ceiling(countOfPeople * procentJitel50Kv * 0.01 / avgCountPeolpe);
                countHouses50Kv = (int)Math.Ceiling((double)countFlat50Kv / 50);

                countFlat100Kv = (int)Math.Ceiling(countOfPeople * procentJitel100Kv * 0.01 / avgCountPeolpe);
                countHouses100Kv = (int)Math.Ceiling((double)countFlat100Kv / 100);

                countFlat200Kv = (int)Math.Ceiling(countOfPeople * procentJitel200Kv * 0.01 / avgCountPeolpe);
                countHouses200Kv = (int)Math.Ceiling((double)countFlat200Kv / 200);

                countFlats = countFlat4Kv + countFlat8Kv + countFlat16Kv + countFlat50Kv + countFlat100Kv + countFlat200Kv;

                countHouses = countHouses4Kv + countHouses8Kv + countHouses16Kv + countHouses50Kv + countHouses100Kv + countHouses200Kv;

                addToTime = (int)Math.Ceiling(24 / (double)countCS);


                tAllLab.Text = countFlats + "/" + countHouses;
                t4Label.Text = countFlat4Kv + "/" + countHouses4Kv;
                t8Label.Text = countFlat8Kv + "/" + countHouses8Kv;
                t16Label.Text = countFlat16Kv + "/" + countHouses16Kv;
                t50Label.Text = countFlat50Kv + "/" + countHouses50Kv;
                t100Label.Text = countFlat100Kv + "/" + countHouses100Kv;
                t200Label.Text = countFlat200Kv + "/" + countHouses200Kv;

                CountDays();
                label38.Text = (countDays * countFlats * 2).ToString();
                label39.Text = (countDays * countHouses).ToString();
                //MessageBox.Show(addToTime.ToString());
            }
            catch {
                MessageBox.Show("Не удалось рассчитать парметры для генерации опроса. Проверьте заданные значения.");
            }


        }

        //значения процентного соотношения квартир
        void SetValuesData()
        {

            try
            {
                countOfPeople = int.Parse(numPeople.Text);
                avgCountPeolpe = float.Parse(avgPeople.Text);

                procentJitel4Kv = int.Parse(kv4.Text);
                procentJitel8Kv = int.Parse(kv8.Text);
                procentJitel16Kv = int.Parse(kv16.Text);
                procentJitel50Kv = int.Parse(kv50.Text);
                procentJitel100Kv = int.Parse(kv100.Text);
                procentJitel200Kv = int.Parse(kv200.Text);

                procentOtkzaUSPD = int.Parse(VerErrorUSPD.Text);
                procentOtkzaMeter = int.Parse(VerErrorMeter.Text);

                meanDay = int.Parse(meanDayBox.Text);
                stdDevDay = int.Parse(stdDevDayBox.Text);

                meanNight = int.Parse(meanNightBox.Text);
                stdDevNight = int.Parse(stdDevNightBox.Text);
            }
            catch
            {
                MessageBox.Show("Неверно введены данные!!!");
            }

        }

        //значения процентного соотношения счётчиков
        void SetValuesDataMeter()
        {
            try
            {
                procMercuri = int.Parse(mercury.Text);
                procEnergomera = int.Parse(energomera.Text);
                procMayak = int.Parse(mayak.Text);
                procMatrica = int.Parse(matrica.Text);
            }
            catch
            {
                MessageBox.Show("Неверно введены данные!!!");
            }
        }

        //очистка БД
        void ClearDataBase()
        {
            try
            {
                SqlCommand clearCommand = connection.CreateCommand();
                clearCommand.CommandText = clearCom;
                clearCommand.ExecuteNonQuery();

                System.Threading.Thread.Sleep(300);
                MessageBox.Show("Таблица очищена!!!");
            }
            catch (SqlException ex){
                MessageBox.Show("Ошибка при очистке бд. \n"+ex.Message);
            }
            
        }

        //подсчёт дней в окне
        void CountDays()
        {
            try
            {
                int d1 = int.Parse(dataDay.Text);
                int d2 = int.Parse(dataDay2.Text);

                int m1 = int.Parse(dataMonth.Text);
                int m2 = int.Parse(dataMonth2.Text);

                int y1 = int.Parse(dataYear.Text);
                int y2 = int.Parse(dataYear2.Text);

                date1 = new DateTime(y1, m1, d1);
                date2 = new DateTime(y2, m2, d2);
                TimeSpan time = date2 - date1;
                countDays = time.Days;
                labelCountDays.Text = time.Days.ToString();
            }
            catch
            {
                labelCountDays.Text = "Ошибка";
            }


        }

        //инициализация при запуске
        public GeneratorData()
        {
            InitializeComponent();
            CountDays();
            CountParameters();
        }


    }


}
