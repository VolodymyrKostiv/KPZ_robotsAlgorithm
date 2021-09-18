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

        private Dictionary<Robot.Common.Robot, EnergyStation> robotsWithTargetStations;

        public int RoundCounter { get; set; }
        public int RobotCounter { get; set; }

        public KostivVolodymyrAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
            RobotCounter = 10;
            robotsWithTargetStations = new Dictionary<Robot.Common.Robot, EnergyStation>();
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

            EnergyStation bestTarget = _stationManager.FindNearestFreeStation(map, robots, currentRobot);
            Position bestTargetPosition = _stationManager.FindBestPositionNearStation(bestTarget, robots, currentRobot);
            int distanceToBestTarget = _cellManager.CalculateDistanceBetweenCells(bestTargetPosition, currentRobot.Position);

            if (distanceToBestTarget < MaxDistanceToCollect || currentRobot.Energy == 0)
            {
                return new CollectEnergyCommand();
            }

            if (distanceToBestTarget <= currentRobot.Energy)
            {
                return new MoveCommand() { NewPosition = bestTargetPosition };
            }
            else 
            {
                EnergyStation target = bestTarget;
                robotsWithTargetStations.TryGetValue(currentRobot, out EnergyStation resVal);
                if (resVal != null)
                {
                    target = robotsWithTargetStations[currentRobot];
                }
                else
                {
                    target = _stationManager.FindNonTargetedFreeStation(map, robots, currentRobot, robotsWithTargetStations);
                    if (target != null)
                    {
                        robotsWithTargetStations[currentRobot] = target;
                    }
                    else
                    {
                        return new MoveCommand() { NewPosition = new Position(currentRobot.Position.X + 1, currentRobot.Position.Y + 1) };
                    }
                }

                Position targetPosition = _stationManager.FindBestPositionNearStation(bestTarget, robots, currentRobot);
                int distanceToTarget = _cellManager.CalculateDistanceBetweenCells(bestTargetPosition, currentRobot.Position);

                int xDist = target.Position.X < currentRobot.Position.X ? -1 : 1;
                xDist = target.Position.X == currentRobot.Position.X ? 0 : xDist;

                int yDist = target.Position.Y < currentRobot.Position.Y ? -1 : 1;
                yDist = target.Position.Y == currentRobot.Position.Y ? 0 : yDist;

                return new MoveCommand() { NewPosition = new Position(currentRobot.Position.X + xDist, currentRobot.Position.Y + yDist) };
            }
        }
    }
}
