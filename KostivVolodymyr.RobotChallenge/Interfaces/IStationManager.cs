using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KostivVolodymyr.RobotChallenge.Interfaces
{
    public interface IStationManager
    {
        EnergyStation FindNearestFreeStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        EnergyStation FindNearestStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        (Position, EnergyStation) FindBestStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        Position FindBestPositionNearStation(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        IEnumerable<Robot.Common.Robot> CheckTerritoryNearStation(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        bool RobotInStationRange(EnergyStation station, Robot.Common.Robot currentRobot);

        bool StationIsFree(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        bool StationIsOccupiedByMe(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);

        bool StationIsOccupiedByEnemy(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot);
    }
}
