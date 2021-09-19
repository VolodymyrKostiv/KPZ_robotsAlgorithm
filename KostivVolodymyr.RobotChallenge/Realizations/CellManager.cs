using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;

namespace KostivVolodymyr.RobotChallenge.Realizations
{
    public class CellManager : ICellManager
    {
        public int CalculateDistanceBetweenCells(Position p1, Position p2)
        {
            _ = p1 ?? throw new ArgumentNullException();
            _ = p2 ?? throw new ArgumentNullException();

            return (int)(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public bool RobotCanGoToCell(Robot.Common.Robot robot, Position target)
        {
            _ = robot ?? throw new ArgumentNullException();
            _ = target ?? throw new ArgumentNullException();

            int distance = CalculateDistanceBetweenCells(robot.Position, target);

            return robot.Energy >= distance;
        }

        public bool CellIsFree(Position targetCell, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot, out Robot.Common.Robot robotOnCell)
        {
            _ = targetCell ?? throw new ArgumentNullException();
            _ = robots ?? throw new ArgumentNullException();
            _ = currentRobot ?? throw new ArgumentNullException();

            foreach (Robot.Common.Robot robot in robots)
            {
                if (robot != currentRobot)
                {
                    if (robot.Position == targetCell)
                    {
                        robotOnCell = robot;
                        return false;
                    }
                }
            }

            robotOnCell = null;

            return true;
        }
        public bool EnemyRobotIsOnCell(Position targetCell, IEnumerable<Robot.Common.Robot> robots)
        {
            _ = targetCell ?? throw new ArgumentNullException();
            _ = robots ?? throw new ArgumentNullException();

            foreach (Robot.Common.Robot robot in robots)
            {
                if (robot.Position == targetCell)
                {
                    return robot.OwnerName != "Kostiv Volodymyr";
                }
            }

            return false;
        }

        public bool MyRobotIsOnCell(Position targetCell, IEnumerable<Robot.Common.Robot> robots)
        {
            _ = targetCell ?? throw new ArgumentNullException();
            _ = robots ?? throw new ArgumentNullException();

            foreach (Robot.Common.Robot robot in robots)
            {
                if (robot.Position == targetCell)
                {
                    return robot.OwnerName == "Kostiv Volodymyr";
                }
            }

            return false;
        }
    }
}
