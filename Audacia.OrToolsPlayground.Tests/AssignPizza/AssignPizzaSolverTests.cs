using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Models;
using Audacia.OrToolsPlayground.Examples.AssignPizza;
using Audacia.OrToolsPlayground.Examples.AssignPizza.Models;
using Audacia.Random.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.AssignPizza
{
    public class AssignPizzaSolverTests
    {
        [Fact]
        public void Each_employee_gets_a_pizza()
        {
            var random = new System.Random();
            var employees = new List<Employee>
            {
                new(random.Forename(), random.Next(0, 10), random.Next(0, 10), random.Next(0, 10)),
                new(random.Forename(), random.Next(0, 10), random.Next(0, 10), random.Next(0, 10)),
                new(random.Forename(), random.Next(0, 10), random.Next(0, 10), random.Next(0, 10))
            };
            var model = new AssignPizzaModel
            {
                Employees = employees,
                // Make more than enough pizzas
                Pizzas = Enumerable.Range(0, 10)
                    .Select(_ => random.Enum<PizzaType>())
                    .ToList()
            };

            var solver = new AssignPizzaSolver(model);
            var assignment = solver.Solve();

            assignment.Should().HaveCount(employees.Count);
        }

        [Fact]
        public void Undesirable_pizzas_are_left_over()
        {
            // Create some employees that all don't like hawaiian as much as the other types
            var random = new System.Random();
            var employees = new List<Employee>
            {
                new(random.Forename(), margheritaPreference: 10, pepperoniPreference: 10, hawaiianPreference: 9),
                new(random.Forename(), margheritaPreference: 10, pepperoniPreference: 10, hawaiianPreference: 9)
            };
            var model = new AssignPizzaModel
            {
                Employees = employees,
                Pizzas = new List<PizzaType> {PizzaType.Margherita, PizzaType.Pepperoni, PizzaType.Hawaiian}
            };

            var solver = new AssignPizzaSolver(model);
            var assignment = solver.Solve();

            assignment.Values.Should().NotContain(PizzaType.Hawaiian);
        }

        [Fact]
        public void Assigns_pizzas_that_people_like_the_most()
        {
            var random = new System.Random();
            var firstPerson = random.Forename();
            var secondPerson = random.Forename();
            var thirdPerson = random.Forename();
            var employees = new List<Employee>
            {
                new(firstPerson, margheritaPreference: 6, pepperoniPreference: 2, hawaiianPreference: 2),
                new(secondPerson, margheritaPreference: 10, pepperoniPreference: 8, hawaiianPreference: 7),
                new(thirdPerson, margheritaPreference: 10, pepperoniPreference: 9, hawaiianPreference: 2)
            };
            var model = new AssignPizzaModel
            {
                Employees = employees,
                Pizzas = new List<PizzaType> {PizzaType.Margherita, PizzaType.Pepperoni, PizzaType.Hawaiian}
            };

            var solver = new AssignPizzaSolver(model);
            var assignment = solver.Solve();

            // Though the first person isn't as passionate about margherita as the rest, they really don't like the others
            assignment[firstPerson].Should().Be(PizzaType.Margherita);
            assignment[secondPerson].Should().Be(PizzaType.Hawaiian);
            assignment[thirdPerson].Should().Be(PizzaType.Pepperoni);
        }
    }
}