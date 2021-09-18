using KostivVolodymyr.RobotChallenge.Interfaces;
using Robot.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KostivVolodymyr.RobotChallenge.Realizations
{
    public class StationManager : IStationManager
    {
        const int maxDistanceToCollect = 2;
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

            //Check above triangle
            for (int y = station.Position.Y + maxDistanceToCollect, count = 0; y > station.Position.Y; --y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
                {
                    if (!_cellManager.CellIsFree(new Position(x, y), robots, currentRobot, out Robot.Common.Robot robotOnCell))
                    {
                        robotsOnStation.Add(robotOnCell);
                    }
                }
            }

            //Check station X Line
            for (int x = station.Position.X - maxDistanceToCollect; x <= station.Position.X + maxDistanceToCollect; ++x)
            {
                if (!_cellManager.CellIsFree(new Position(x, station.Position.Y), robots, currentRobot, out Robot.Common.Robot robotOnCell))
                {
                    robotsOnStation.Add(robotOnCell);
                }
            }

            //Check below triangle
            for (int y = station.Position.Y - maxDistanceToCollect, count = 0; y < station.Position.Y; ++y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
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

        public EnergyStation FindNonTargetedFreeStation(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot, Dictionary<Robot.Common.Robot, EnergyStation> robotsWithTargets)
        {
            _ = map ?? throw new ArgumentNullException(nameof(map));
            _ = robots ?? throw new ArgumentNullException(nameof(robots));
            _ = currentRobot ?? throw new ArgumentNullException(nameof(currentRobot));
            _ = robotsWithTargets ?? throw new ArgumentNullException(nameof(robotsWithTargets));

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
                    if (StationIsFree(station, robots, currentRobot) && !robotsWithTargets.ContainsValue(station))
                    {
                        return station;
                    }
                }
                occupiedStations.Concat<EnergyStation>(orderedNearestStations);
            }
            return null;
        }

        public Position FindBestPositionNearStation(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            Position bestPosition = new Position(station.Position.X, station.Position.Y);
            int minDistance = _cellManager.CalculateDistanceBetweenCells(bestPosition, currentRobot.Position);
            int distanceToCell;

            //Check above triangle
            for (int y = station.Position.Y + maxDistanceToCollect, count = 0; y > station.Position.Y; --y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
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

            //Check station X Line
            for (int x = station.Position.X - maxDistanceToCollect; x <= station.Position.X + maxDistanceToCollect; ++x)
            {
                Position tempPos = new Position(x, station.Position.Y);
                distanceToCell = _cellManager.CalculateDistanceBetweenCells(tempPos, currentRobot.Position);
                if (distanceToCell < minDistance && !_cellManager.MyRobotIsOnCell(tempPos, robots))
                {
                    minDistance = distanceToCell;
                    bestPosition.X = x;
                    bestPosition.Y = station.Position.Y;
                }
            }

            //Check below triangle
            for (int y = station.Position.Y - maxDistanceToCollect, count = 0; y < station.Position.Y; ++y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
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
            for (int y = station.Position.Y + maxDistanceToCollect, count = 0; y > station.Position.Y; --y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
                {
                    Position tempPosition = new Position(x, y);
                    if (tempPosition == currentRobot.Position)
                    {
                        return true;
                    }
                }
            }

            //Check station X Line
            for (int x = station.Position.X - maxDistanceToCollect; x <= station.Position.X + maxDistanceToCollect; ++x)
            {
                Position tempPosition = new Position(x, station.Position.Y);
                if (tempPosition == currentRobot.Position)
                {
                    return true;
                }
            }

            //Check below triangle
            for (int y = station.Position.Y - maxDistanceToCollect, count = 0; y < station.Position.Y; ++y, ++count)
            {
                for (int x = station.Position.X - count; x <= station.Position.X + count; ++x)
                {
                    Position tempPosition = new Position(x, y);
                    if (tempPosition == currentRobot.Position)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public bool StationIsOccupiedOnlyByOneMyRobot(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            IEnumerable<Robot.Common.Robot> res = CheckTerritoryNearStation(station, robots, currentRobot);

            return res.Count() == 1 && res.First().OwnerName == "Kostiv Volodymyr";
        }

        public bool StationIsFree(EnergyStation station, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            IEnumerable<Robot.Common.Robot> robotsOnStation = CheckTerritoryNearStation(station, robots, currentRobot);

            return robotsOnStation.Count() == 0 ? true : false;
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