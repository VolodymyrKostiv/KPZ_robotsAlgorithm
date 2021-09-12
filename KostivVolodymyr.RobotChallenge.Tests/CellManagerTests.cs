using KostivVolodymyr.RobotChallenge.Realizations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robot.Common;
using System;
using System.Collections.Generic;

namespace KostivVolodymyr.RobotChallenge.Tests
{
    [TestClass]
    public class CellManagerTests
    {
        private readonly CellManager _cellManager;

        public CellManagerTests()
        {
            _cellManager = new CellManager();
        }

        [TestMethod]
        public void CalculateDistanceBetweenCells_P1IsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.CalculateDistanceBetweenCells(null, new Position()));
        }

        [TestMethod]
        public void CalculateDistanceBetweenCells_P2IsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.CalculateDistanceBetweenCells(new Position(), null));
        }

        [TestMethod]
        public void CalculateDistanceBetweenCells_ReturnsZero()
        {
            //Arrange
            Position p1 = new Position(10, 10);
            Position p2 = new Position(10, 10);

            //Act
            int result = _cellManager.CalculateDistanceBetweenCells(p1, p2);

            //Assert
            Assert.IsTrue(result == 0);
        }

        [TestMethod]
        public void CalculateDistanceBetweenCells_ReturnsBiggerThanZero()
        {
            //Arrange
            Position p1 = new Position(5, 5);
            Position p2 = new Position(10, 10);

            //Act
            int result = _cellManager.CalculateDistanceBetweenCells(p1, p2);

            //Assert
            Assert.IsTrue(result > 0);
        }

        [TestMethod]
        public void CellIsFree_TargetCellIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.CellIsFree(null, GetTestRobotsForOcupation(), new Robot.Common.Robot(), out Robot.Common.Robot x));
        }

        [TestMethod]
        public void CellIsFree_RobotsIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.CellIsFree(new Position(), null, new Robot.Common.Robot(), out Robot.Common.Robot x));
        }

        [TestMethod]
        public void CellIsFree_CurrentRobotIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.CellIsFree(new Position(), GetTestRobotsForOcupation(), null, out Robot.Common.Robot x));
        }

        [TestMethod]
        public void CellIsFree_RobotIsOnCell_ReturnsFalse()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot
            {
                Energy = 1000,
                OwnerName = "Lester",
                Position = new Position(21, 54)
            };
            Robot.Common.Robot expectedRobotOnCell = new Robot.Common.Robot
            {
                Energy = 7,
                OwnerName = "Trevor",
                Position = new Position(4, 3)
            };
            Position target = new Position(4, 3);

            //Act
            bool result = _cellManager.CellIsFree(target, GetTestRobotsForOcupation(), currentRobot, out Robot.Common.Robot resultRobotOnCell);

            //Assert
            Assert.IsFalse(result);
            Assert.AreEqual(expectedRobotOnCell.OwnerName, resultRobotOnCell.OwnerName);
            Assert.AreEqual(expectedRobotOnCell.Position, resultRobotOnCell.Position);
            Assert.AreEqual(expectedRobotOnCell.Energy, resultRobotOnCell.Energy);
        }

        [TestMethod]
        public void CellIsFree_NoRobotsOnCell_ReturnsTrue()
        {
            //Arrange
            Robot.Common.Robot currentRobot = new Robot.Common.Robot
            {
                Energy = 1000,
                OwnerName = "Lester",
                Position = new Position(22, 54)
            };

            Position target = new Position(21, 54);

            //Act
            bool result = _cellManager.CellIsFree(target, GetTestRobotsForOcupation(), currentRobot, out Robot.Common.Robot resultRobotOnCell);

            //Assert
            Assert.IsTrue(result);
            Assert.IsNull(resultRobotOnCell);
        }

        [TestMethod]
        public void MyRobotIsOnCell_TargetCellIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.MyRobotIsOnCell(null, GetTestRobotsForOcupation()));
        }

        [TestMethod]
        public void MyRobotIsOnCell_RobotsIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.MyRobotIsOnCell(new Position(), null));
        }

        [TestMethod]
        public void MyRobotIsOnCell_MyCell_ReturnsTrue()
        {
            //Arrange
            Position target = new Position(50, 50);

            //Act
            bool result = _cellManager.MyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void MyRobotIsOnCell_EnemyCell_ReturnsFalse()
        {
            //Arrange
            Position target = new Position(99, 99);

            //Act
            bool result = _cellManager.MyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void MyRobotIsOnCell_NoRobotsOnCell_ReturnsFalse()
        {
            //Arrange
            Position target = new Position(42, 42);

            //Act
            bool result = _cellManager.MyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EnemyRobotIsOnCell_TargetCellIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.EnemyRobotIsOnCell(null, GetTestRobotsForOcupation()));
        }

        [TestMethod]
        public void EnemyRobotIsOnCell_RobotsIsNull_ThrowArgumentNullException()
        {
            //Assert
            Assert.ThrowsException<ArgumentNullException>(() =>
                _cellManager.EnemyRobotIsOnCell(new Position(), null));
        }

        [TestMethod]
        public void EnemyRobotIsOnCell_EnemyCell_ReturnsTrue()
        {
            //Arrange
            Position target = new Position(99, 99);

            //Act
            bool result = _cellManager.EnemyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void EnemyRobotIsOnCell_MyCell_ReturnsFalse()
        {
            //Arrange
            Position target = new Position(50, 50);

            //Act
            bool result = _cellManager.EnemyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void EnemyRobotIsOnCell_NoRobotsOnCell_ReturnsFalse()
        {
            //Arrange
            Position target = new Position(42, 42);

            //Act
            bool result = _cellManager.EnemyRobotIsOnCell(target, GetTestRobotsForOcupation());

            //Assert
            Assert.IsFalse(result);
        }


        private static IEnumerable<Robot.Common.Robot> GetTestRobotsForOcupation()
        {
            return new List<Robot.Common.Robot>
            {
                new Robot.Common.Robot {Energy = 100, OwnerName = "Franklin", Position = new Position(99, 99) },
                new Robot.Common.Robot {Energy = 1, OwnerName = "Trevor", Position = new Position(3,1) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 1, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 9, OwnerName = "Trevor", Position = new Position(2,4) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Kostiv Volodymyr", Position = new Position(50, 50) },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Trevor", Position = new Position(3,4) },
                new Robot.Common.Robot {Energy = 7, OwnerName = "Trevor", Position = new Position(4,3) },
                new Robot.Common.Robot {Energy = 10, OwnerName = "Michael" },
                new Robot.Common.Robot {Energy = 15, OwnerName = "Franklin" },
            };
        }

    }
}
