using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KostivVolodymyr.RobotChallenge.Realizations
{
    public class StationManager : IStationManager
    {
        const int MaxDistanceToCollect = 2;
        const int MaxNumberOfRobotsNearStation = 2;
        private readonly ICellManager _cellManager;

        public StationManager()
        {
            _cellManager = ServiceManager.ServiceManager.CreateCellManager();
        }

        public IEnumerable<Robot.Common.Robot> CheckTerritoryNearStation(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            _ = station ?? throw new ArgumentNullException(nameof(station));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));

            List<Robot.Common.Robot> robotsOnStation = new List<Robot.Common.Robot>();

            for (int y = station.Position.Y - MaxDistanceToCollect; y <= station.Position.Y + MaxDistanceToCollect; ++y)
            {
                for (int x = station.Position.X - MaxDistanceToCollect; x <= station.Position.X + MaxDistanceToCollect; ++x)
                {
                    if (!_cellManager.CellIsFree(new Position(x, y), robots, currentRobot, out Robot.Common.Robot robotOnCell))
                    {
                        robotsOnStation.Add(robotOnCell);
                    }
                }
            }       

            return robotsOnStation;
        }

        public EnergyStation FindNearestFreeStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));

            List<EnergyStation> occupiedStations = new List<EnergyStation>();
            for (int distanceCounter = 1; distanceCounter < 100; ++distanceCounter)
            {
                IEnumerable<EnergyStation> nearbyStations = map.GetNearbyResources(currentRobot.Position, distanceCounter);
                if (nearbyStations.Count() <= 0)
                {
                    continue;
                }

                IEnumerable<EnergyStation> orderedNearestStations = nearbyStations.Distinct<EnergyStation>().OrderBy(p => _cellManager.CalculateDistanceBetweenCells(currentRobot.Position, p.Position));
                foreach (EnergyStation station in orderedNearestStations)
                {
                    if (StationIsFree(station, robots, currentRobot))
                    {
                        return station;
                    }
                }
                occupiedStations.Concat<EnergyStation>(orderedNearestStations);
            }

            return null;
        }

        public EnergyStation FindNearestStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));

            for (int distanceCounter = 1; distanceCounter < 100; ++distanceCounter)
            {
                IEnumerable<EnergyStation> nearbyStations = map.GetNearbyResources(currentRobot.Position, distanceCounter);
                if (nearbyStations.Count() <= 0)
                {
                    continue;
                }

                EnergyStation nearestStation = nearbyStations.OrderBy(p => _cellManager.CalculateDistanceBetweenCells(currentRobot.Position, p.Position)).First();
                return nearestStation;
            }
            return null;
        }

        public (Position, EnergyStation) FindBestStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));

            Position bestPosition = null;
            EnergyStation bestStation = null;
            int minDistance = -1;

            foreach (EnergyStation station in map.Stations)
            {
                if (CheckTerritoryNearStation(station, robots, currentRobot).Count() < MaxNumberOfRobotsNearStation)
                {
                    Position tempPosition = FindBestPositionNearStation(station, robots, currentRobot);
                    int tempDistance = _cellManager.CalculateDistanceBetweenCells(currentRobot.Position, tempPosition);
                    if (tempDistance < minDistance || minDistance == -1)
                    {
                        minDistance = tempDistance;
                        bestPosition = tempPosition;
                        bestStation = station;
                    }
                }
            }

            return (bestPosition, bestStation);
        }

        public Position FindBestPositionNearStation(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            _ = station ?? throw new ArgumentNullException(nameof(station));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));

            Position bestPosition = new Position(station.Position.X, station.Position.Y);
            int minDistance = _cellManager.CalculateDistanceBetweenCells(bestPosition, currentRobot.Position);
            int distanceToCell;

            for (int y = station.Position.Y - MaxDistanceToCollect; y <= station.Position.Y + MaxDistanceToCollect; ++y)
            {
                for (int x = station.Position.X - MaxDistanceToCollect; x <= station.Position.X + MaxDistanceToCollect; ++x)
                {
                    Position tempPos = new Position(x, y);
                    distanceToCell = _cellManager.CalculateDistanceBetweenCells(tempPos, currentRobot.Position);
                    if (distanceToCell < minDistance && !_cellManager.MyRobotIsOnCell(tempPos, robots))
                    {
                        minDistance = distanceToCell;
                        bestPosition.X = x;
                        bestPosition.Y = y;
                    }
                }
            }

            return bestPosition;
        }

        public bool RobotInStationRange(EnergyStation station, Robot.Common.Robot currentRobot)
        {
            return currentRobot.Position.X >= (station.Position.X - MaxDistanceToCollect) &&
                currentRobot.Position.X <= (station.Position.X + MaxDistanceToCollect) &&
                currentRobot.Position.Y >= (station.Position.Y - MaxDistanceToCollect) &&
                currentRobot.Position.Y <= (station.Position.Y + MaxDistanceToCollect);
        }


        public bool StationIsFree(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            IEnumerable<Robot.Common.Robot> robotsOnStation = CheckTerritoryNearStation(station, robots, currentRobot);

            return robotsOnStation.Count() == 0;
        }

        public bool StationIsOccupiedByEnemy(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            IEnumerable<Robot.Common.Robot> robotsOnStation = CheckTerritoryNearStation(station, robots, currentRobot);

            return robotsOnStation.Where(x => x.OwnerName != ServiceManager.ServiceManager.MyName).Count() > 0;
        }

        public bool StationIsOccupiedByMe(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            IEnumerable<Robot.Common.Robot> robotsOnStation = CheckTerritoryNearStation(station, robots, currentRobot);

            return robotsOnStation.Where(x => x.OwnerName == ServiceManager.ServiceManager.MyName).Count() > 0;
        }
    }
}