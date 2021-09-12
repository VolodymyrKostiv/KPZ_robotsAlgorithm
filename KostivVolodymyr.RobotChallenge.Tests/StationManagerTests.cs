using KostivVolodymyr.RobotChallenge.Interfaces;
using KostivVolodymyr.RobotChallenge.Realizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KostivVolodymyr.RobotChallenge.Tests
{
    [TestClass]
    public class StationManagerTests
    {

        const int maxDistanceToCollect = 2;
        private readonly Mock<ICellManager> _cellManager;
        private readonly StationManager _stationManager;

        public StationManagerTests()
        {
            _cellManager = new Mock<ICellManager>();
            _stationManager = new StationManager();
        }

        [TestMethod]
        public void CheckTerritoryNearStation_StationIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.CheckTerritoryNearStation(null, GetTestRobotsForOcupation(), new Robot.Common.Robot()));
        }

        [TestMethod]
        public void CheckTerritoryNearStation_RobotsIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.CheckTerritoryNearStation(new EnergyStation(), null, new Robot.Common.Robot()));
        }

        [TestMethod]
        public void CheckTerritoryNearStation_CurrentRobotIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.CheckTerritoryNearStation(new EnergyStation(), GetTestRobotsForOcupation(), null));
        }

        [TestMethod]
        public void CheckTerritoryNearStation_FullPresenceOnStation_ReturnsIEnumerable()
        {
            //Arrange
            Robot.Common.Robot robotOnCell = new Robot.Common.Robot { Energy = 5, OwnerName = "Michael", Position = new Position(10, 10) };
            Robot.Common.Robot currentRobot = new Robot.Common.Robot { Energy = 20, OwnerName = "Franklin", Position = new Position(2, 2) };
            EnergyStation station = new EnergyStation { Position = new Position { X = 3, Y = 3 } };
            IEnumerable<Robot.Common.Robot> expected = GetTestRobotsForOcupation().Where(x => x.OwnerName == "Trevor").ToList();

            _cellManager
                .Setup(x => x.CellIsFree(It.IsAny<Position>(), GetTestRobotsForOcupation(), currentRobot, out robotOnCell))
                .Returns(true);

            List<Robot.Common.Robot> expectedOrdered = expected.OrderBy(x => x.Energy).ToList<Robot.Common.Robot>();

            //Act
            var result = _stationManager.CheckTerritoryNearStation(station, GetTestRobotsForOcupation(), currentRobot);
            List<Robot.Common.Robot> resultOrdered = result.OrderBy(t => t.Energy).ToList<Robot.Common.Robot>();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Robot.Common.Robot>));
            Assert.IsTrue(Enumerable.SequenceEqual(expectedOrdered, resultOrdered));
        }

        [TestMethod]
        public void CheckTerritoryNearStation_NoPresenceOnStation_ReturnsIEnumerableRobots()
        {
            //Arrange
            Robot.Common.Robot robotOnCell = new Robot.Common.Robot { Energy = 5, OwnerName = "Michael", Position = new Position(10, 10) };
            Robot.Common.Robot currentRobot = new Robot.Common.Robot { Energy = 20, OwnerName = "Franklin", Position = new Position(2, 2) };
            EnergyStation station = new EnergyStation { Position = new Position { X = 50, Y = 50 } };
            IEnumerable<Robot.Common.Robot> expected = GetTestRobotsForOcupation().Where(x => x.OwnerName == "Trevor").ToList();

            _cellManager
                .Setup(x => x.CellIsFree(It.IsAny<Position>(), GetTestRobotsForOcupation(), currentRobot, out robotOnCell))
                .Returns(false);

            List<Robot.Common.Robot> expectedOrdered = expected.OrderBy(x => x.Energy).ToList<Robot.Common.Robot>();

            //Act
            var result = _stationManager.CheckTerritoryNearStation(station, GetTestRobotsForOcupation(), currentRobot);
            List<Robot.Common.Robot> resultOrdered = result.OrderBy(t => t.Energy).ToList<Robot.Common.Robot>();

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IEnumerable<Robot.Common.Robot>));
            Assert.IsTrue(result.Count() == 0);
        }

        [TestMethod]
        public void FindNearestFreeStation_MapIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestFreeStation(null, GetTestRobotsForOcupation(), new Robot.Common.Robot()));
        }

        [TestMethod]
        public void FindNearestFreeStation_RobotsIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestFreeStation(new Map(), null, new Robot.Common.Robot()));
        }

        [TestMethod]
        public void FindNearestFreeStation_CurrentRobotIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestFreeStation(new Map(), GetTestRobotsForOcupation(), null));
        }

        [TestMethod]
        public void FindNearestFreeStation_NearbyStationsAlwaysZero_ReturnsNull()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot();
            Map map = new Map();

            //Act
            var result = _stationManager.FindNearestFreeStation(map, GetTestRobotsForOcupation(), currentRobot);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindNearestFreeStation_ReturnsEnergyStation()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot() { Position = new Position(100, 100) };
            Map map = new Map(2, 10);

            //Act
            var result = _stationManager.FindNearestFreeStation(map, GetTestRobotsForOcupation(), currentRobot);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(EnergyStation));
        }

        [TestMethod]
        public void FindNearestStation_MapIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestStation(null, GetTestRobotsForOcupation(), new Robot.Common.Robot()));
        }

        [TestMethod]
        public void FindNearestStation_RobotsIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestStation(new Map(), null, new Robot.Common.Robot()));
        }

        [TestMethod]
        public void FindNearestStation_CurrentRobotIsNull_ThrowsArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _stationManager.FindNearestStation(new Map(), GetTestRobotsForOcupation(), null));
        }

        [TestMethod]
        public void FindNearestStation_NearbyStationsAlwaysZero_ReturnsNull()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot();
            Map map = new Map();

            //Act
            var result = _stationManager.FindNearestStation(map, GetTestRobotsForOcupation(), currentRobot);

            //Assert
            Assert.IsNull(result);
        }

        [TestMethod]
        public void FindNearestStation_ReturnsEnergyStation()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot() { Position = new Position(100, 100) };
            Map map = new Map(2, 10);

            //Act
            var result = _stationManager.FindNearestFreeStation(map, GetTestRobotsForOcupation(), currentRobot);

            //Assert
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(EnergyStation));
        }

        [TestMethod]
        public void FindGroupOfStations_ThrowNotImplementedException()
        {
            //Assert
            Assert.ThrowsException<NotImplementedException>(() =>
                _stationManager.FindGroupOfStations(It.IsAny<Map>(), GetTestRobotsForOcupation(), It.IsAny<Robot.Common.Robot>()));
        }

        [TestMethod]
        public void StationIsFree_ThrowNotImplementedException()
        {
            //Assert
            Assert.ThrowsException<NotImplementedException>(() =>
                _stationManager.FindGroupOfStations(It.IsAny<Map>(), GetTestRobotsForOcupation(), It.IsAny<Robot.Common.Robot>()));
        }

        private static IEnumerable<Robot.Common.Robot> GetTestRobotsForOcupation()
        {
            return new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot {Energy = 100, OwnerName = "Franklin", Position = new Position(99, 99) },
                new Robot.Common.Robot {Energy = 1, OwnerName = "Trevor", Position = new Position(3,1) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 2, OwnerName = "Trevor", Position = new Position(2,2) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 1, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 13, OwnerName = "Trevor", Position = new Position(3,2) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 4, OwnerName = "Trevor", Position = new Position(4,2) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 5, OwnerName = "Trevor", Position = new Position(1,3) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 6, OwnerName = "Trevor", Position = new Position(2,3) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 7, OwnerName = "Trevor", Position = new Position(4,3) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 8, OwnerName = "Trevor", Position = new Position(5,3) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 9, OwnerName = "Trevor", Position = new Position(2,4) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Trevor", Position = new Position(3,4) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 11, OwnerName = "Trevor", Position = new Position(4,4) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 12, OwnerName = "Trevor", Position = new Position(3,5) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
            };
        }
    }
}