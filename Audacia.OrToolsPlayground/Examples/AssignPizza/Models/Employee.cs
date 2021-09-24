using System.Collections.Generic;
using Audacia.OrToolsPlayground.Common.Models;

namespace Audacia.OrToolsPlayground.Examples.AssignPizza.Models
{
    public class Employee
    {
        public Employee(string name, int margheritaPreference, int pepperoniPreference, int hawaiianPreference)
        {
            Name = name;
            Preferences = new Dictionary<PizzaType, int>
            {
                {PizzaType.Margherita, margheritaPreference},
                {PizzaType.Pepperoni, pepperoniPreference},
                {PizzaType.Hawaiian, hawaiianPreference}
            };
        }

        public string Name { get; }
        public Dictionary<PizzaType, int> Preferences { get; }
    }
}