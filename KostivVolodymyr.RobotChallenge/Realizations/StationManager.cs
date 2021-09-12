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
            List<Robot.Common.Robot> robotsOnStation = new List<Robot.Common.Robot>();

            //for (int y = station.Position.Y - maxDistanceToCollect; y >= station.Position.Y + maxDistanceToCollect; ++y)
            //{
            //    for(int x = station.Position.X - maxDistanceToCollect; x >= station.Position.Y + maxDistanceToCollect; ++x)
            //    {
            //        if(!_cellManager.CellIsFree(new Position, ))
            //    }
            //}

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
                    if (StationIsFree(station, robots, currentRobot) 
                        || (StationIsOccupiedByEnemy(station, robots, currentRobot)
                        && !StationIsOccupiedByMe(station, robots, currentRobot)))
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

        public Position FindGroupOfStations(Map map, IEnumerable<Robot.Common.Robot> robots, Robot.Common.Robot currentRobot)
        {
            throw new NotImplementedException();
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