using System.Collections.Generic;
using Audacia.OrToolsPlayground.Common.Exceptions;
using Audacia.OrToolsPlayground.Common.Models;
using Audacia.OrToolsPlayground.Examples.AssignPizza.Models;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.AssignPizza
{
    public class AssignPizzaSolver
    {
        private readonly AssignPizzaModel _model;
        private readonly int[,] _costs;
        private readonly int _numEmployees;
        private readonly int _numPizzas;

        public AssignPizzaSolver(AssignPizzaModel model)
        {
            _model = model;
            _costs = GetCosts(model);
            _numEmployees = _costs.GetLength(0);
            _numPizzas = _costs.GetLength(1);
        }

        public Dictionary<string, PizzaType> Solve()
        {
            var model = new CpModel();
 
            IntVar[,] costVars = new IntVar[_numEmployees, _numPizzas];
            IntVar[] flattenedCostVars = new IntVar[_numEmployees * _numPizzas];
            int[] flattenedCosts = new int[_numEmployees * _numPizzas];
            SetModelVars(costVars, model, flattenedCostVars, flattenedCosts);

            AddConstraints(costVars, model);

            // set the objective of the model
            model.Maximize(LinearExpr.ScalProd(flattenedCostVars, flattenedCosts));

            // Solve
            CpSolver solver = new ();
            var status = solver.Solve(model);
            if (status != CpSolverStatus.Optimal)
            {
                throw OptimisationException.SubOptimalCpSolution(status);
            }

            var assignments = GetOutput(solver, costVars);

            return assignments;
        }

        /// <summary>
        /// Obtain the result from the provided <paramref name="solver"/>.
        /// </summary>
        /// <returns>A dictionary of Employee Name -> the Pizza Type they're assigned.</returns>
        private Dictionary<string, PizzaType> GetOutput(CpSolver solver, IntVar[,] costVars)
        {
            var assignments = new Dictionary<string, PizzaType>();
            for (var employeeIndex = 0; employeeIndex < _numEmployees; employeeIndex++)
            {
                var matchedEmployee = _model.Employees[employeeIndex];
                for (var pizzaIndex = 0; pizzaIndex < _numPizzas; pizzaIndex++)
                {
                    var value = solver.Value(costVars[employeeIndex, pizzaIndex]);
                    if (value == 1)
                    {
                        assignments.Add(matchedEmployee.Name, _model.Pizzas[pizzaIndex]);
                    }
                }
            }

            return assignments;
        }

        private void AddConstraints(IntVar[,] costVars, CpModel model)
        {
            // Ensure one pizza per employee
            for (var employeeIndex = 0; employeeIndex < _numEmployees; employeeIndex++)
            {
                IntVar[] assignments = new IntVar[_numPizzas];
                for (var pizzaIndex = 0; pizzaIndex < _numPizzas; pizzaIndex++)
                {
                    assignments[pizzaIndex] = costVars[employeeIndex, pizzaIndex];
                }

                model.Add(LinearExpr.Sum(assignments) == 1);
            }

            // Ensure no pizza is given out more than once
            for (var pizzaIndex = 0; pizzaIndex < _numPizzas; ++pizzaIndex)
            {
                IntVar[] vars = new IntVar[_numEmployees];
                for (var employeeIndex = 0; employeeIndex < _numEmployees; ++employeeIndex)
                {
                    vars[employeeIndex] = costVars[employeeIndex, pizzaIndex];
                }

                model.Add(LinearExpr.Sum(vars) <= 1);
            }
        }

        /// <summary>
        /// Declare all variables for the model to solve
        /// </summary>
        private void SetModelVars(IntVar[,] costVars, CpModel model, IList<IntVar> flattenedCostVars, IList<int> flattenedCosts)
        {
            for (var employeeIndex = 0; employeeIndex < _numEmployees; ++employeeIndex)
            {
                for (var pizzaIndex = 0; pizzaIndex < _numPizzas; ++pizzaIndex)
                {
                    // create a variable that is set to 1 if this employee is given this pizza, and 0 otherwise
                    costVars[employeeIndex, pizzaIndex] = model.NewIntVar(0, 1, $"employee_{employeeIndex}_pizza_{pizzaIndex}");
                    var flattenedIndex = employeeIndex * _numPizzas + pizzaIndex;
                    flattenedCostVars[flattenedIndex] = costVars[employeeIndex, pizzaIndex];
                    flattenedCosts[flattenedIndex] = _costs[employeeIndex, pizzaIndex];
                }
            }
        }

        /// <summary>
        /// Return a 2-dimensional matrix of size employees x pizzas,
        /// where matrix[i,j] is how much employee i likes pizza j.
        /// </summary>
        private static int[,] GetCosts(AssignPizzaModel model)
        {
            var costs = new int[model.Employees.Count, model.Pizzas.Count];
            for (var employeeIndex = 0; employeeIndex < model.Employees.Count; employeeIndex++)
            {
                var employee = model.Employees[employeeIndex];
                for (var pizzaIndex = 0; pizzaIndex < model.Pizzas.Count; pizzaIndex++)
                {
                    var pizzaType = model.Pizzas[pizzaIndex];
                    costs[employeeIndex, pizzaIndex] = employee.Preferences[pizzaType];
                }
            }

            return costs;
        }
    }
}