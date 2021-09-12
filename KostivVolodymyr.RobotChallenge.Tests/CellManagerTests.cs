using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace KostivVolodymyr.RobotChallenge.Tests
{
    [TestClass]
    public class CellManagerTests
    {
        [TestMethod]
        public void TestMethod1()
        {
            Robot.Common.Robot a = new Robot.Common.Robot() { Energy = 100, OwnerName = "Me", Position = new Robot.Common.Position { X = 5, Y = 5 } };
            Robot.Common.EnergyStation e = new Robot.Common.EnergyStation() { Energy = 100, Position = new Robot.Common.Position { X = 5, Y = 5 }, RecoveryRate = 50 };
        }
    }
}
