using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Exceptions;
using Audacia.OrToolsPlayground.Examples.PickUpEmployees.Models;
using Google.OrTools.ConstraintSolver;
using Google.Protobuf.WellKnownTypes;

namespace Audacia.OrToolsPlayground.Examples.PickUpEmployees
{
    public class PickUpEmployeesSolver
    {
        private readonly PickUpEmployeesModel _model;
        private readonly int _numberOfCars;

        public PickUpEmployeesSolver(PickUpEmployeesModel model)
        {
            _model = model;
            _numberOfCars = _model.DriverNames.Count;
        }

        public Dictionary<string, string[]> Solve()
        {
            RoutingIndexManager manager = new(_model.DistanceMatrix.GetLength(0), _numberOfCars, 0);

            RoutingModel routing = new(manager);

            SetTravelDistances(routing, manager);

            var solution = SolveModel(routing);
            if (solution == null)
            {
                throw OptimisationException.NoSolution();
            }

            var assignments = GetAssignments(routing, manager, solution);

            return assignments;
        }

        private static Assignment SolveModel(RoutingModel routing)
        {
            RoutingSearchParameters searchParameters =
                operations_research_constraint_solver.DefaultRoutingSearchParameters();
            searchParameters.FirstSolutionStrategy = FirstSolutionStrategy.Types.Value.PathCheapestArc;
            searchParameters.LocalSearchMetaheuristic = LocalSearchMetaheuristic.Types.Value.GuidedLocalSearch;
            searchParameters.TimeLimit = new Duration {Seconds = 1};

            Assignment solution = routing.SolveWithParameters(searchParameters);
            return solution;
        }

        private void SetTravelDistances(RoutingModel routing, RoutingIndexManager manager)
        {
            var transitCallbackIndex = routing.RegisterTransitCallback((fromIndex, toIndex) =>
            {
                // Convert from routing variable Index to distance matrix NodeIndex.
                var fromNode = manager.IndexToNode(fromIndex);
                var toNode = manager.IndexToNode(toIndex);
                return _model.DistanceMatrix[fromNode, toNode];
            });

            routing.SetArcCostEvaluatorOfAllVehicles(transitCallbackIndex);
        }

        /// <summary>
        /// Obtain the output from the provided <paramref name="solution"/>
        /// </summary>
        /// <returns>A dictionary of Driver Name -> the Employee Names of who they're assigned to pick up.</returns>
        /// <exception cref="OptimisationException"></exception>
        private Dictionary<string, string[]> GetAssignments(RoutingModel routing, RoutingIndexManager manager,
            Assignment solution)
        {
            var assignments = new Dictionary<string, string[]>();
            for (var carIndex = 0; carIndex < _numberOfCars; carIndex++)
            {
                var driverName = _model.DriverNames[carIndex];
                var driverVisits = new List<string>();
                var index = routing.Start(carIndex);
                while (!routing.IsEnd(index))
                {
                    index = solution.Value(routing.NextVar(index));
                    // If zero, the driver has returned to the office
                    var employeeIndex = manager.IndexToNode(index);
                    if (employeeIndex > 0)
                    {
                        driverVisits.Add(_model.EmployeesToPickUp[employeeIndex - 1]);
                    }
                }

                if (driverVisits.Any())
                {
                    assignments.Add(driverName, driverVisits.ToArray());
                }
            }

            return assignments;
        }
    }
}