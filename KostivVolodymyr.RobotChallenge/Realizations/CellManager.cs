using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KostivVolodymyr.RobotChallenge.Realizations
{
    public class CellManager : ICellManager
    {
        public int CalculateDistanceBetweenCells(Position p1, Position p2)
        {
            return (int)(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
        }

        public bool CellIsFree(Position targetCell, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot, out Robot.Common.Robot robotOnCell)
        {
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
            return !MyRobotIsOnCell(targetCell, robots);
        }

        public bool MyRobotIsOnCell(Position targetCell, IEnumerable<Robot.Common.Robot> robots)
        {
            foreach (Robot.Common.Robot robot in robots)
            {
                if (robot.Position == targetCell)
                {
                    return robot.OwnerName == "Kostiv Volodymyr" ? true : false;
                }
            }
            return false;
        }
    }
}
