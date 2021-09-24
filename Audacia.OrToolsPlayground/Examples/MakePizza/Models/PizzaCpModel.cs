using System.Collections.Generic;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.MakePizza.Models
{
    /// <summary>
    /// Extension of the <see cref="CpModel"/> type to keep track of useful information
    /// </summary>
    public class PizzaCpModel : CpModel
    {
        public PizzaCpModel(int pizzaCount, int machinesCount)
        {
            PizzaStageIntervals = new List<List<IntervalVar>>(pizzaCount);
            PizzaStageStarts = new List<List<IntVar>>(pizzaCount);
            PizzaStageEnds = new List<List<IntVar>>(pizzaCount);

            MachineIntervals = new List<List<IntervalVar>>(machinesCount);
            for (int i = 0; i < machinesCount; i++)
            {
                MachineIntervals.Add(new List<IntervalVar>());
            }

            for (int i = 0; i < pizzaCount; i++)
            {
                PizzaStageIntervals.Add(new List<IntervalVar>());
                PizzaStageStarts.Add(new List<IntVar>());
                PizzaStageEnds.Add(new List<IntVar>());
            }
        }

        public List<List<IntervalVar>> MachineIntervals { get; set; }

        /// <summary>
        /// Gets a list of the end variables for each pizza's stages
        /// </summary>
        public List<List<IntVar>> PizzaStageEnds { get; }

        /// <summary>
        /// Gets a list of the start variables for each pizza's stages
        /// </summary>
        public List<List<IntVar>> PizzaStageStarts { get; }

        /// <summary>
        /// Gets a list of the intervals for each pizza's stages
        /// </summary>
        public List<List<IntervalVar>> PizzaStageIntervals { get; }
    }
}