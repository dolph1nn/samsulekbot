using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public async Task TestFetchGuildUserRoles()
        {
            await SSB.Core.Database.DBHandler.OpenConnection(new SSB.Core.SSBConfig { DBHostname = "2019-SRV14.lunarcolony.local", DBDatabase = "ssbd_test" });
            List<ulong> Expected = new List<ulong>() { 205150878464868352, 685511556569759808, 635951376211640331, 635652821911339019, 691871029303443565, 629498416640294934 };
            List<ulong> Actual = await SSB.Core.Database.DBHandler.FetchGuildUserRoles(217097093226037249, 170683612092432387);

            Assert.AreEqual(Expected.Count, Actual.Count);
            foreach (ulong role in Expected) 
            {
                Assert.IsTrue(Actual.Contains(role));
            }
            foreach (ulong role in Actual)
            {
                Assert.IsTrue(Expected.Contains(role));
            }
        }

        [TestMethod]
        public async Task TestCheckUserRolesExists()
        {
            await SSB.Core.Database.DBHandler.OpenConnection(new SSB.Core.SSBConfig { DBHostname = "2019-SRV14.lunarcolony.local", DBDatabase = "ssbd_test" });
            Assert.IsTrue(await SSB.Core.Database.DBHandler.CheckUserRolesExists(217097093226037249, 170683612092432387));
            Assert.IsTrue(!await SSB.Core.Database.DBHandler.CheckUserRolesExists(42069, 69420));
        }

        [TestMethod]
        public async Task TestCheckGuildExists()
        {
            await SSB.Core.Database.DBHandler.OpenConnection(new SSB.Core.SSBConfig { DBHostname = "2019-SRV14.lunarcolony.local", DBDatabase = "ssbd_test" });
            Assert.IsTrue(await SSB.Core.Database.DBHandler.CheckGuildExists(170683612092432387));
            Assert.IsTrue(!await SSB.Core.Database.DBHandler.CheckGuildExists(17068361209243238));
        }
    }
}
