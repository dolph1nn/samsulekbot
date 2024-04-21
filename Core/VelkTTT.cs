/* This file is part of SamSulekBot.
 *
 * SamSulekBot is free software: you can redistribute it and/or modify it
 * under the terms of the GNU General Public License as published by the
 * Free Software Foundation, either version 3 of the License, or (at your
 * option) any later version.
 *
 * SamSulekBot is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or 
 * FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for
 * more details.
 *
 * You should have received a copy of the GNU General Public License along
 * with SamSulekBot. If not, see <https://www.gnu.org/licenses/>.
 */

using System.Collections.Generic;
using System.Threading.Tasks;
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
namespace SSB.Core
{
    public static class VelkTTT
    {

        public static async Task<double> GunTrueRPM(double WeaponBaseRPM, double RPMStat)
        {
            if (RPMStat > 1) { RPMStat /= 100; }
            return WeaponBaseRPM / (1 - RPMStat);
        }

        public static async Task<double> GunEffectiveRPM(double WeaponBaseRPM, double RPMStat)
        {
            double TrueRPM = await GunTrueRPM(WeaponBaseRPM, RPMStat);
            int RPMCap = 3960;

            while (TrueRPM < RPMCap)
            {
                for (int i = 0; TrueRPM < RPMCap; i++) RPMCap = RPMCaps[i];
            }    
            return RPMCap;
        }

        public static async Task<double> ReverseRPM(double WeaponRPM, double DesiredRPM)
        {
            return ((WeaponRPM / DesiredRPM) - 1) * -1;
        }

        public static async Task<double> ReverseRPM(double WeaponRPM, double DesiredRPM, double Trig)
        {
            if (Trig > 1) { Trig /= 100; }
            return ((WeaponRPM / (1 - Trig) / DesiredRPM) - 1) * -1;
        }

        public static async Task<double> doubleToPct(double Value, StatRange Range)
        {
            return ((Range.Max - Range.Min) * Value) + Range.Min;
        }

        public static async Task<double> PctTodouble(double Value, StatRange Range)
        {
            if (Value <= 1) { Value *= 100; }
            return ((Value - Range.Min) / (Range.Max - Range.Min));
        }

        public static int[] RPMCaps = new int[] { 3960 , 1980 , 1320 , 990 , 792 , 660 , 565 , 495 , 440 , 396 , 360 , 264 , 220 , 198 , 180 , 165 , 132 , 120 , 110 , 99 , 90 , 88 , 72, 66 };

        public struct StatRange
        {
            public int Min { get; set; }
            public int Max { get; set; }
        }
    }
}
