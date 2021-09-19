using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KostivVolodymyr.RobotChallenge.Interfaces
{
    public interface ICellManager
    {
        int CalculateDistanceBetweenCells(Position p1, Position p2);

        bool RobotCanGoToCell(Robot.Common.Robot robot, Position target);

        bool CellIsFree(Position targetCell, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot, out Robot.Common.Robot robotOnCell);

        bool MyRobotIsOnCell(Position targetCell, IEnumerable<Robot.Common.Robot> robots);

        bool EnemyRobotIsOnCell(Position targetCell, IEnumerable<Robot.Common.Robot> robots);
    }
}
