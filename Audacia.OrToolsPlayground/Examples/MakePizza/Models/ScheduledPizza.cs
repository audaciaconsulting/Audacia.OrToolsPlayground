using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Models;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Models
{
    public class ScheduledPizza
    {
        public PizzaType Type { get; set; }

        public List<ScheduledPizzaStage> Stages { get; set; } = new ();

        public long Start => Stages[0].Start;
        
        public long Finish => Stages.Last().Finish;
    }
}