using System.Collections.Generic;
using Audacia.OrToolsPlayground.Common.Models;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Models
{
    public class MakePizzaModel
    {
        public MakePizzaModel(int numberOfChefs, List<PizzaType> types)
        {
            NumberOfChefs = numberOfChefs;
            Types = types;
        }

        /// <summary>
        /// Gets the number of chefs that will be making the pizza
        /// </summary>
        public int NumberOfChefs { get; }

        /// <summary>
        /// Gets a list of pizzas to make
        /// </summary>
        public List<PizzaType> Types { get; }
    }
}