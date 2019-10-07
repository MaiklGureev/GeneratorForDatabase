

using System;

namespace GeneratorDataForBase
{
    internal class MyRandom
    {
        static Random rand = new Random();

        public static bool GetRandomEvent(Random rand, int truePercentage)
        {
            return rand.NextDouble() < truePercentage / 100.0;
        }

        public static int GetGaussRandomValue(int mean, double stdDev )
        {
        
            //reuse this if you are generating many
            double u1 = 1.0 - rand.NextDouble(); //uniform(0,1] random doubles
            double u2 = 1.0 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) *
                         Math.Sin(2.0 * Math.PI * u2); //random normal(0,1)
            double randNormal =
                         mean + stdDev * randStdNormal; //random normal(mean,stdDev^2)
            int a = (int)Math.Ceiling(randNormal);
            if (a < 0) {
                return 0;
            }
            if (a > (mean + stdDev)) {
                return (int)(mean + stdDev);
            }
            return a;
        }

    }

}