using Audacia.OrToolsPlayground.Examples.MakePizza.Constants;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Models
{
    public class ScheduledPizzaStage
    {
        public CookingStage Stage { get; set; }
        
        public long Start { get; set; }
        
        public long Finish { get; set; }
    }
}