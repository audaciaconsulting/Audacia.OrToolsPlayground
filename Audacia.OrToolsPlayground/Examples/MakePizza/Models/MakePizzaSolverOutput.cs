using System.Collections.Generic;
using System.Linq;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Models
{
    /// <summary>
    /// The result object of solving the <see cref="MakePizzaSolver"/>.
    /// </summary>
    public class MakePizzaSolverOutput
    {
        public long Finish => Pizzas.Max(p => p.Finish);

        public List<ScheduledPizza> Pizzas { get; set; } = new ();
    }
}