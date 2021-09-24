using System.Collections.Generic;
using Audacia.OrToolsPlayground.Common.Models;

namespace Audacia.OrToolsPlayground.Examples.AssignPizza.Models
{
    public class AssignPizzaModel
    {
        public List<PizzaType> Pizzas { get; set; }

        public List<Employee> Employees { get; set; }
    }
}