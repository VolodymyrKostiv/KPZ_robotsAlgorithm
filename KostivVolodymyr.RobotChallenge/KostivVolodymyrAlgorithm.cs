using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;
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

        public static int RoundCounter { get; set; } = 0;

        public KostivVolodymyrAlgorithm()
        {
            Logger.OnLogRound += Logger_OnLogRound;
            _cellManager = ServiceManager.ServiceManager.CreateCellManager();
            _stationManager = ServiceManager.ServiceManager.CreateStationManager();
        }

        void Logger_OnLogRound(object sender, LogRoundEventArgs args)
        {
            RoundCounter++;
        }

        public RobotCommand DoStep(IList<Robot.Common.Robot> robots, int robotToMoveIndex, Map map)
        {
            Robot.Common.Robot currentRobot = robots[robotToMoveIndex];
            if (currentRobot.Energy > 600 && RoundCounter < 35)
            {
                return new CreateNewRobotCommand() { NewRobotEnergy = 300 };
            }
            EnergyStation target = _stationManager.FindNearestFreeStation(map, robots, currentRobot);
            int distanceToTarget = _cellManager.CalculateDistanceBetweenCells(currentRobot.Position, target.Position);

            if (distanceToTarget < 2 || currentRobot.Energy == 0)
            {
                return new CollectEnergyCommand();
            }

            Position newPosition = new Position() { X = (target.Position.X + currentRobot.Position.X) / 2, Y = (target.Position.Y + currentRobot.Position.Y) / 2 };
            if(Math.Pow(distanceToTarget, 2) > currentRobot.Energy)
            {
                return new MoveCommand() { NewPosition = newPosition };
            }

            return new MoveCommand() { NewPosition = target.Position };
        }
    }
}
