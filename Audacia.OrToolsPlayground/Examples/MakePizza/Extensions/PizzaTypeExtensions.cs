using System;
using System.Collections.Generic;
using Audacia.OrToolsPlayground.Common.Models;
using Audacia.OrToolsPlayground.Examples.MakePizza.Constants;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Extensions
{
    public static class PizzaTypeExtensions
    {
        /// <summary>
        /// Return how long each stage takes to cook the provided <paramref name="type"/> of pizza.
        /// </summary>
        /// <returns>An dictionary of (cooking stage, time taken) for each stage of cooking the pizza.</returns>
        public static Dictionary<CookingStage, long> GetStages(this PizzaType type)
        {
            return type switch
            {
                PizzaType.Margherita => new Dictionary<CookingStage, long>
                {
                    {CookingStage.Shaping, 30},
                    {CookingStage.Preparing, 10},
                    {CookingStage.Cooking, 15}
                },
                PizzaType.Hawaiian => new Dictionary<CookingStage, long>
                {
                    {CookingStage.Shaping, 30},
                    {CookingStage.Slicing, 10},
                    {CookingStage.Preparing, 15},
                    {CookingStage.Cooking, 17}
                },
                PizzaType.Pepperoni => new Dictionary<CookingStage, long>
                {
                    {CookingStage.Shaping, 30},
                    {CookingStage.Slicing, 8},
                    {CookingStage.Preparing, 20},
                    {CookingStage.Cooking, 20}
                },
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }
    }
}