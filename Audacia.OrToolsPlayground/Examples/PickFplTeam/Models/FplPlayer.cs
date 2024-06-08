using CsvHelper.Configuration.Attributes;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models
{
    public class FplPlayer
    {
        [Name("name")]
        public string Name { get; set; } = null!;

        [Name("team")]
        public string Team { get; set; } = null!;

        /// <summary>
        /// Gets or sets raw cost in millions.
        /// </summary>
        [Name("cost")]
        public decimal CostMillions { get; set; }

        /// <summary>
        /// Gets the cost in an OR-Tools friendly format (i.e no decimals).
        /// Assumes cost  never has > 1dp specificity.
        /// </summary>
        public int Cost => (int)(CostMillions * 10);

        [Name("pos")]
        public PlayerPosition Position { get; set; }

        [Name("sel")]
        public int PercentSelectedBy { get; set; }

        public override string ToString()
        {
            return $"{Name}-{Team}-{Position}".Replace(" ", "_");
        }
    }
}