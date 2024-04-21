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

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class VelkTTT
    {
        [TestMethod]
        public async Task TestTrueRPM()
        {
            double Expected = (double)Math.Round(820.7792207792207, 3);
            double Actual = await SSB.Core.VelkTTT.GunTrueRPM(632, .23);
            Assert.AreEqual(Expected, Actual, 0.001f);
        }

        [TestMethod]
        public async Task TestEffectiveRPM()
        {
            Assert.AreEqual(792, await SSB.Core.VelkTTT.GunEffectiveRPM(632, .23));
        }

        [TestMethod]
        public async Task TestReverseRPM()
        {
            double Expected = 0.3622603430877901;
            double Actual = await SSB.Core.VelkTTT.ReverseRPM(632, 991);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public async Task TestReverseRPMTrig()
        {
            double Expected = 0.2625582135612744;
            double Actual = await SSB.Core.VelkTTT.ReverseRPM(632, 991, 13.52);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public async Task TestdoubleToPct()
        {
            double Expected = 24.37;
            double Actual = await SSB.Core.VelkTTT.doubleToPct(0.67, new SSB.Core.VelkTTT.StatRange() { Min = 17, Max = 28 });
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public async Task TestPctTodouble()
        {
            double Expected = 0.67;
            double Actual = await SSB.Core.VelkTTT.PctTodouble(24.37, new SSB.Core.VelkTTT.StatRange() { Min = 17, Max = 28 });
            Assert.AreEqual(Expected, Actual);
        }
    }
}
