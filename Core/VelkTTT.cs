using System.Collections.Generic;
using System.Threading.Tasks;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SSB.Core
{
    public static class VelkTTT
    {

        public static async Task<float> GunTrueRPM(float WeaponBaseRPM, float RPMStat)
        {
            if (RPMStat > 1) { RPMStat /= 100; }
            return WeaponBaseRPM / (1 - RPMStat);
        }

        public static async Task<float> GunEffectiveRPM(float WeaponBaseRPM, float RPMStat)
        {
            float TrueRPM = await GunTrueRPM(WeaponBaseRPM, RPMStat);
            int RPMCap = 3960;

            while (TrueRPM < RPMCap)
            {
                for (int i = 0; i < 24; i++) RPMCap = RPMCaps[i];
            }    
            return RPMCap;
        }

        public static async Task<float> ReverseRPM(float WeaponRPM, float DesiredRPM)
        {
            return ((WeaponRPM / DesiredRPM) - 1) * -1;
        }

        public static async Task<float> ReverseRPM(float WeaponRPM, float DesiredRPM, float Trig)
        {
            if (Trig > 1) { Trig /= 100; }
            return ((WeaponRPM / (1 - Trig) / DesiredRPM) - 1) * -1;
        }

        public static async Task<float> FloatToPct(float Value, StatRange Range)
        {
            return ((Range.Max - Range.Min) * Value) + Range.Min;
        }

        public static async Task<float> PctToFloat(float Value, StatRange Range)
        {
            if (Value <= 1) { Value *= 100; }
            return ((Value - Range.Min) / (Range.Max - Range.Min));
        }

        public static List<int> RPMCaps = new List<int>(){ 3960 , 1980 , 1320 , 990 , 792 , 660 , 565 , 495 , 440 , 396 , 360 , 264 , 220 , 198 , 180 , 165 , 132 , 120 , 110 , 99 , 90 , 88 , 72 };

        public struct StatRange
        {
            public int Min { get; set; }
            public int Max { get; set; }

            public StatRange(int _Min, int _Max)
            {
                Min = _Min; Max = _Max;
            }
        }
    }
}
