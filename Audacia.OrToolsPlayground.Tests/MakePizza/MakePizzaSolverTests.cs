using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Models;
using Audacia.OrToolsPlayground.Examples.MakePizza;
using Audacia.OrToolsPlayground.Examples.MakePizza.Constants;
using Audacia.OrToolsPlayground.Examples.MakePizza.Models;
using Audacia.Random.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.MakePizza
{
    public class MakePizzaSolverTests
    {
        [Fact]
        public void Waits_one_hour_after_shaping()
        {
            var model = new MakePizzaModel(1, new List<PizzaType> {PizzaType.Margherita});
            var solver = new MakePizzaSolver(model);
            
            var solution = solver.Solve();

            var cookedPizza = solution.Pizzas[0];
            var shapingStage = cookedPizza.Stages.Single(s => s.Stage == CookingStage.Shaping);
            var preparingStage = cookedPizza.Stages.Single(s => s.Stage == CookingStage.Preparing);
            const int oneHour = 60;
            var expectedPreparingStart = shapingStage.Finish + oneHour;
            preparingStage.Start.Should().Be(expectedPreparingStart);
        }

        [Fact]
        public void All_pizzas_are_made()
        {
            var random = new System.Random();
            var pizzas = Enumerable.Range(0, 5).Select(_ => random.Enum<PizzaType>()).ToList();
            var model = new MakePizzaModel(1, pizzas);
            var solver = new MakePizzaSolver(model);
            
            var solution = solver.Solve();

            var expectedCount = pizzas.Count;
            solution.Pizzas.Should().HaveCount(expectedCount);
        }

        [Fact]
        public void Minimizes_the_time_taken_to_cook_all_pizzas()
        {
            var model = new MakePizzaModel(2, new List<PizzaType> {PizzaType.Margherita, PizzaType.Pepperoni});
            var solver = new MakePizzaSolver(model);
            
            var solution = solver.Solve();

            const long expectedFinish = 138;
            solution.Finish.Should().Be(expectedFinish);
        }

        [Fact]
        public void Pizzas_stages_are_done_in_correct_order()
        {
            var model = new MakePizzaModel(1, new List<PizzaType> {PizzaType.Margherita});
            var solver = new MakePizzaSolver(model);
            
            var solution = solver.Solve();

            var expectedStageOrder = new List<CookingStage> {CookingStage.Shaping, CookingStage.Preparing, CookingStage.Cooking};
            var scheduledStages = solution.Pizzas[0].Stages.Select(s => s.Stage);
            scheduledStages.Should().ContainInOrder(expectedStageOrder);
        }

        [Fact]
        public void Only_one_pizza_fits_in_the_oven()
        {
            var model = new MakePizzaModel(2, new List<PizzaType> {PizzaType.Margherita, PizzaType.Margherita});
            var solver = new MakePizzaSolver(model);
            
            var solution = solver.Solve();

            var firstCookingStage = solution.Pizzas[0].Stages.Single(s => s.Stage == CookingStage.Cooking);
            var secondCookingStage = solution.Pizzas[1].Stages.Single(s => s.Stage == CookingStage.Cooking);
            var twoPizzasCookingAtTheSameTime = firstCookingStage.Start < secondCookingStage.Finish &&
                                                secondCookingStage.Start > firstCookingStage.Finish;

            twoPizzasCookingAtTheSameTime.Should().BeFalse();
        }
    }
}