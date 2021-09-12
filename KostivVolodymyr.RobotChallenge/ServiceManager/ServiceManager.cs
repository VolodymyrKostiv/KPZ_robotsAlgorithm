using KostivVolodymyr.RobotChallenge.Interfaces;
using KostivVolodymyr.RobotChallenge.Realizations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KostivVolodymyr.RobotChallenge.ServiceManager
{
    public static class ServiceManager
    {
        public static readonly string MyName = "Kostiv Volodymyr";

        public static ICellManager CreateCellManager()
        {
            return new CellManager();
        }
        public static IStationManager CreateStationManager()
        {
            return new StationManager();
        }
    }
}
