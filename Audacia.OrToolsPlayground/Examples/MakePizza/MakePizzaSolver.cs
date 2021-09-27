using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Exceptions;
using Audacia.OrToolsPlayground.Examples.MakePizza.Constants;
using Audacia.OrToolsPlayground.Examples.MakePizza.Extensions;
using Audacia.OrToolsPlayground.Examples.MakePizza.Models;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.MakePizza
{
    public class MakePizzaSolver
    {
        private readonly MakePizzaModel _request;
        private readonly int _pizzaCount;
        private readonly long _horizon;
        private const int OvenId = 0;
        private const int DoughRisingDuration = 60;

        public MakePizzaSolver(MakePizzaModel request)
        {
            _request = request;
            _pizzaCount = request.Types.Count;
            _horizon = GetHorizon();
        }

        public MakePizzaSolverOutput Solve()
        {
            // The number of machines is the number of chefs + 1 (the oven)
            var machinesCount = _request.NumberOfChefs + 1;
            var model = new PizzaCpModel(_pizzaCount, machinesCount);

            CreateIntervalVars(model);

            AddConstraints(model, machinesCount);

            DeclareObjective(model);

            // Create the solver.
            CpSolver solver = new();
            // Solve the problem.
            var status = solver.Solve(model);

            if (status == CpSolverStatus.Optimal)
            {
                return GetSolution(model, solver);
            }

            throw OptimisationException.NoSolution();
        }

        private void AddConstraints(PizzaCpModel model, int machinesCount)
        {
            AddStagePrecedence(model);

            for (int machineId = 0; machineId < machinesCount; ++machineId)
            {
                // This ensures each 'machine' (chef/oven) is only doing one thing at a time.
                model.AddNoOverlap(model.MachineIntervals[machineId]);
            }
        }
        
        /// <summary>
        /// Make sure stages are scheduled in the correct order, and add any pre/post stage buffers.
        /// </summary>
        private void AddStagePrecedence(PizzaCpModel model)
        {
            for (int j = 0; j < model.PizzaStageStarts.Count; ++j)
            {
                for (int t = 0; t < model.PizzaStageStarts[j].Count - 1; ++t)
                {
                    var postStageBuffer = 0;
                    if (_request.Types[j].GetStages().ElementAt(t).Key == CookingStage.Shaping)
                    {
                        postStageBuffer = DoughRisingDuration;
                    }

                    model.Add(model.PizzaStageEnds[j][t] + postStageBuffer <= model.PizzaStageStarts[j][t + 1]);
                }
            }
        }

        private MakePizzaSolverOutput GetSolution(PizzaCpModel model, CpSolver solver)
        {
            var result = new MakePizzaSolverOutput();
            for (int i = 0; i < model.PizzaStageStarts.Count; i++)
            {
                var pizzaType = _request.Types[i];
                var pizza = new ScheduledPizza
                {
                    Type = pizzaType,
                };
                var stages = pizzaType.GetStages();
                for (int j = 0; j < model.PizzaStageStarts[i].Count; j++)
                {
                    var start = solver.Value(model.PizzaStageStarts[i][j]);
                    var finish = solver.Value(model.PizzaStageEnds[i][j]);
                    pizza.Stages.Add(new ScheduledPizzaStage
                    {
                        Stage = stages.ElementAt(j).Key,
                        Start = start,
                        Finish = finish,
                    });
                }

                result.Pizzas.Add(pizza);
            }

            return result;
        }

        private void DeclareObjective(PizzaCpModel model)
        {
            // Creates array of end_times of jobs.
            IntVar[] allEnds = new IntVar[_pizzaCount];
            for (var pizzaIndex = 0; pizzaIndex < _pizzaCount; pizzaIndex++)
            {
                allEnds[pizzaIndex] = model.PizzaStageEnds[pizzaIndex].Last();
            }

            // Objective: minimize the makespan (maximum end times of all tasks)
            // of the problem.
            IntVar makespan = model.NewIntVar(0, _horizon, "makespan");
            model.AddMaxEquality(makespan, allEnds);
            model.Minimize(makespan);
        }

        /// <summary>
        /// For each stage that needs to be performed, add <see cref="IntervalVar"/>s to the model, with variables
        /// for their potential start time.
        /// </summary>
        /// <param name="model"></param>
        private void CreateIntervalVars(PizzaCpModel model)
        {
            for (var pizzaIndex = 0; pizzaIndex < _request.Types.Count; pizzaIndex++)
            {
                var pizza = _request.Types[pizzaIndex];
                foreach (var (cookingStage, duration) in pizza.GetStages())
                {
                    var stageName = $"pizza_{pizzaIndex}_{cookingStage}";
                    // Create variables for when this stage starts and finishes
                    var startVar = model.NewIntVar(0, _horizon, $"{stageName}_start");
                    var endVar = model.NewIntVar(0, _horizon, $"{stageName}_end");
                    if (cookingStage == CookingStage.Cooking)
                    {
                        // If this stage is cooking, there is only one 'machine' option, so we can have a mandatory interval var
                        var stageVar =
                            model.NewIntervalVar(startVar, duration, endVar, $"{stageName}_interval");
                        model.PizzaStageIntervals[pizzaIndex].Add(stageVar);
                        model.MachineIntervals[OvenId].Add(stageVar);
                    }
                    else
                    {
                        // This will contain a list of boolean variables, based on which chef is chosen for this stage.
                        var chefActivations = new List<IntVar>();
                        // Start from 1, as index 0 is the oven 'machine'
                        for (var chefIndex = 1; chefIndex <= _request.NumberOfChefs; chefIndex++)
                        {
                            // If we're not cooking, we have a choice of chefs who can perform this task.
                            var chefActivation = model.NewBoolVar($"{stageName}_chef_{chefIndex}");
                            IntVar chefStartVar = model.NewIntVar(0, _horizon, $"{stageName}_start");
                            IntVar chefEndVar = model.NewIntVar(0, _horizon, $"{stageName}_end");

                            // Create an optional interval. This sets the chefActivation to true if the chef is chosen.
                            var stageVar =
                                model.NewOptionalIntervalVar(startVar, duration, endVar, chefActivation,
                                    $"{stageName}_interval");
                            model.PizzaStageIntervals[pizzaIndex].Add(stageVar);
                            model.MachineIntervals[chefIndex].Add(stageVar);

                            // Bind this chef's start and end variables to the actual start and end, if we choose this chef
                            model.Add(chefStartVar == startVar).OnlyEnforceIf(chefActivation);
                            model.Add(chefEndVar == endVar).OnlyEnforceIf(chefActivation);
                            chefActivations.Add(chefActivation);
                        }

                        // Ensure a chef is chosen for this pizza
                        model.Add(new SumArray(chefActivations) == 1);
                    }

                    model.PizzaStageStarts[pizzaIndex].Add(startVar);
                    model.PizzaStageEnds[pizzaIndex].Add(endVar);
                }
            }
        }

        /// <summary>
        /// Return the 'horizon', reflecting the maximum amount of time all pizzas can take to be made.
        /// </summary>
        /// <returns></returns>
        private long GetHorizon()
        {
            var horizon = 0L;
            // Add duration of each stage of each pizza
            foreach (var (_, duration) in _request.Types.SelectMany(t => t.GetStages()))
            {
                horizon += duration;
            }

            // Each pizza also requires rising
            horizon += _request.Types.Count * DoughRisingDuration;

            return horizon;
        }
    }
}