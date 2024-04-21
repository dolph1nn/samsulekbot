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

namespace Tests
{
    [TestClass]
    public class ProgressBar
    {
        [TestMethod]
        public void ProgressBarTest0Pct()
        {
            string Expected = "[          ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(0);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest10Pct()
        {
            string Expected = "[=         ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.1f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest20Pct()
        {
            string Expected = "[==        ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.2f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest30Pct()
        {
            string Expected = "[===       ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.3f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest40Pct()
        {
            string Expected = "[====      ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.4f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest50Pct()
        {
            string Expected = "[=====     ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.5f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest60Pct()
        {
            string Expected = "[======    ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.6f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest70Pct()
        {
            string Expected = "[=======   ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.7f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest80Pct()
        {
            string Expected = "[========  ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.8f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest90Pct()
        {
            string Expected = "[========= ]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(.9f);
            Assert.AreEqual(Expected, Actual);
        }

        [TestMethod]
        public void ProgressBarTest100Pct()
        {
            string Expected = "[==========]";
            string Actual = SSB.Discord.Commands.UpdateProgressBar(1);
            Assert.AreEqual(Expected, Actual);
        }
    }
}
