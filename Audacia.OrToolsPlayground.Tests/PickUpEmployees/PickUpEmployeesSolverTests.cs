using System.Collections.Generic;
using Audacia.OrToolsPlayground.Examples.PickUpEmployees;
using Audacia.OrToolsPlayground.Examples.PickUpEmployees.Models;
using Audacia.Random.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.PickUpEmployees
{
    public class PickUpEmployeesSolverTests
    {
        /// <summary>
        /// Symmetric matrix of node distances, of size n+1 x n+1, where n is the number of employees to pick up.
        /// </summary>
        private readonly long[,] _distances =
        {
            { 0, 3, 2, 5 },
            { 3, 0, 1, 11 },
            { 2, 1, 0, 10 },
            { 5, 11, 10, 0 }
        };

        [Fact]
        public void Each_employee_is_picked_up()
        {
            var random = new System.Random();
            var employeeNames = new List<string> {random.Forename(), random.Forename(), random.Forename()};
            var driverName = random.Forename();

            var model = new PickUpEmployeesModel(_distances, employeeNames, new List<string> {driverName});
            var solver = new PickUpEmployeesSolver(model);
            var assignments = solver.Solve();

            assignments[driverName].Should().HaveCount(employeeNames.Count);
        }

        [Fact]
        public void Model_minimizes_distance_travelled()
        {
            var random = new System.Random();
            var employeeNames = new List<string> {random.Forename(), random.Forename(), random.Forename()};
            var driverName = random.Forename();

            var solver = new PickUpEmployeesSolver(new PickUpEmployeesModel(_distances, employeeNames, new List<string> {driverName}));
            var assignments = solver.Solve();

            // By inspection, we should go to nodes B -> D -> C.
            var expectedRoute = new[] {employeeNames[0], employeeNames[2], employeeNames[1]};
            assignments[driverName].Should().BeEquivalentTo(expectedRoute);
        }

        [Fact]
        public void Uses_multiple_cars_if_faster()
        {
            var random = new System.Random();
            var employeeNames = new List<string> {random.Forename(), random.Forename(), random.Forename()};
            var firstDriverName = random.Forename();
            var secondDriverName = random.Forename();
            var driverNames = new List<string> {firstDriverName, secondDriverName};

            var solver = new PickUpEmployeesSolver(new PickUpEmployeesModel(_distances, employeeNames, driverNames));
            var assignments = solver.Solve();

            assignments.Should().HaveCount(driverNames.Count);
        }
    }
}