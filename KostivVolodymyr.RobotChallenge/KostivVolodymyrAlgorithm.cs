using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KostivVolodymyr.RobotChallenge
{
    public class KostivVolodymyrAlgorithm : IRobotAlgorithm
    {
        public string Author => "Kostiv Volodymyr";
        private readonly ICellManager _cellManager;
        private readonly IStationManager _stationManager;

        const int MinParentEnergyToCreateRobot = 300;
        const int MaxRoundToCreateRobot = 40;
        const int EnergyParentGivesToSon = 200;
        const int MaxDistanceToCollect = 2;
        const int MaxRoundToJumpToStation = 25;
        const int MaxNumOfRobots = 100;

        private Dictionary<Robot.Common.Robot, EnergyStation> robotsWithTheirStations;

        public int RoundCounter { get; set; }
        public int RobotCounter { get; set; }

        public KostivVolodymyrAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
            RobotCounter = 10;
            robotsWithTheirStations = new Dictionary<Robot.Common.Robot, EnergyStation>();
            _cellManager = ServiceManager.ServiceManager.CreateCellManager();
            _stationManager = ServiceManager.ServiceManager.CreateStationManager();
        }

        private void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            RoundCounter++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot currentRobot = robots[robotToMoveIndex];

            if (RobotCounter < MaxNumOfRobots && currentRobot.Energy > MinParentEnergyToCreateRobot && RoundCounter < MaxRoundToCreateRobot)
            {
                ++RobotCounter;
                return new CreateNewRobotCommand() { NewRobotEnergy = EnergyParentGivesToSon };
            }

            robotsWithTheirStations.TryGetValue(currentRobot, out EnergyStation possibleTarget);
            if (possibleTarget != null && _stationManager.RobotInStationRange(possibleTarget, currentRobot))
            {
                return new CollectEnergyCommand();
            }

            EnergyStation nearestStation = _stationManager.FindNearestStation(map, robots, currentRobot);
            if (_stationManager.StationIsFree(nearestStation, robots, currentRobot) ||
                _stationManager.StationIsOccupiedOnlyByOneMyRobot(nearestStation, robots, currentRobot))
            {
                robotsWithTheirStations[currentRobot] = nearestStation;

                if (_stationManager.RobotInStationRange(nearestStation, currentRobot))
                {
                    return new CollectEnergyCommand();
                }

                Position bestPositionToNearestStation = _stationManager.FindBestPositionNearStation(nearestStation, robots, currentRobot);

                if (currentRobot.Energy >= _cellManager.CalculateDistanceBetweenCells(bestPositionToNearestStation, currentRobot.Position))
                {
                    robotsWithTheirStations[currentRobot] = nearestStation;

                    return new MoveCommand() { NewPosition = bestPositionToNearestStation };
                }
            }

            EnergyStation targetStation = _stationManager.FindNearestFreeStation(map, robots, currentRobot);
            Position targetPosition = _stationManager.FindBestPositionNearStation(targetStation, robots, currentRobot);

            if (currentRobot.Energy >= _cellManager.CalculateDistanceBetweenCells(targetPosition, currentRobot.Position))
            {
                robotsWithTheirStations[currentRobot] = nearestStation;

                return new MoveCommand() { NewPosition = targetPosition };
            }

            int xDist = targetPosition.X < currentRobot.Position.X ? -1 : 1;
            xDist = targetPosition.X == currentRobot.Position.X ? 0 : xDist;

            int yDist = targetPosition.Y < currentRobot.Position.Y ? -1 : 1;
            yDist = targetPosition.Y == currentRobot.Position.Y ? 0 : yDist;

            return new MoveCommand() { NewPosition = new Position(currentRobot.Position.X + xDist, currentRobot.Position.Y + yDist) };
        }
    }
}