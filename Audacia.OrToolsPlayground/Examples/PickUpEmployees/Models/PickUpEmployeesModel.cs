using System.Collections.Generic;

namespace Audacia.OrToolsPlayground.Examples.PickUpEmployees.Models
{
    public class PickUpEmployeesModel
    {
        public List<string> EmployeesToPickUp { get; }
        
        public List<string> DriverNames { get; }
        
        public long[,] DistanceMatrix { get; }

        public PickUpEmployeesModel(long[,] distanceMatrix, List<string> employeesToPickUp, List<string> driverNames)
        {
            DistanceMatrix = distanceMatrix;
            EmployeesToPickUp = employeesToPickUp;
            DriverNames = driverNames;
        }
    };
}