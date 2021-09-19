using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System.Collections.Generic;

namespace KostivVolodymyr.RobotChallenge
{
    public class KostivVolodymyrAlgorithm : IRobotAlgorithm
    {
        public string Author => "Kostiv Volodymyr";
        private readonly ICellManager _cellManager;
        private readonly IStationManager _stationManager;

        const int MinParentEnergyToCreateRobot = 400;
        const int MaxRoundToCreateRobot = 40;
        const int EnergyParentGivesToSon = 300;
        const int MaxDistanceToCollect = 2;
        const int MaxRoundToJumpToStation = 25;
        const int MaxNumOfRobots = 100;

        Dictionary<int, EnergyStation> robotsWithTargets;

        public int RoundCounter { get; set; }
        public int RobotCounter { get; set; }

        public KostivVolodymyrAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;

            RobotCounter = 10;

            robotsWithTargets = new Dictionary<int, EnergyStation>();

            _cellManager = ServiceManager.ServiceManager.CreateCellManager();
            _stationManager = ServiceManager.ServiceManager.CreateStationManager();
        }

        private void Logger_OnLogRound(object sender, LogRoundEventArgs e)
        {
            RoundCounter++;
        }

        private bool CanCreateRobot(Robot.Common.Robot robot)
        {
            return RobotCounter < MaxNumOfRobots && robot.Energy >= MinParentEnergyToCreateRobot && RoundCounter < MaxRoundToCreateRobot;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot currentRobot = robots[robotToMoveIndex];

            if (CanCreateRobot(currentRobot))
            {
                ++RobotCounter;
                return new CreateNewRobotCommand() { NewRobotEnergy = EnergyParentGivesToSon };
            }

            if (robotsWithTargets.TryGetValue(robotToMoveIndex, out EnergyStation _))
            {
                return new CollectEnergyCommand();
            }

            (Position position, EnergyStation station) target = _stationManager.FindBestStation(map, robots, currentRobot);

            if (target.position == null || target.station == null)
            {
                return new CollectEnergyCommand();
            }

            if (_stationManager.RobotInStationRange(target.station, currentRobot) || target.position == currentRobot.Position)
            {
                robotsWithTargets[robotToMoveIndex] = target.station;
                return new CollectEnergyCommand();
            }
            else if (_cellManager.RobotCanGoToCell(currentRobot, target.position))
            {
                robotsWithTargets[robotToMoveIndex] = target.station;
                return new MoveCommand() { NewPosition = target.position };
            }
            else
            {
                int xDist = target.position.X < currentRobot.Position.X ? -1 : 1;
                xDist = target.position.X == currentRobot.Position.X ? 0 : xDist;

                int yDist = target.position.Y < currentRobot.Position.Y ? -1 : 1;
                yDist = target.position.Y == currentRobot.Position.Y ? 0 : yDist;

                return new MoveCommand() { NewPosition = new Position(currentRobot.Position.X + xDist, currentRobot.Position.Y + yDist) };
            }
        }
    }
}