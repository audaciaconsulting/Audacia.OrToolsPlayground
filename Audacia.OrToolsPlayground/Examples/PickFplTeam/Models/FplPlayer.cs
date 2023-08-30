using CsvHelper.Configuration.Attributes;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models
{
    public class FplPlayer
    {
        [Name("id")] public int Id { get; set; }

        [Name("first_name")] public string FirstName { get; set; } = null!;

        [Name("second_name")] public string Surname { get; set; } = null!;

        [Name("total_points")] public int Points { get; set; }

        [Name("now_cost")] public long Cost { get; set; }

        [Name("team")] public int TeamId { get; set; }

        [Name("element_type")] public PlayerPosition Position { get; set; }

        [Name("chance_of_playing_next_round")] public string ChanceOfPlayingNextRound { get; set; } = null!;

        public bool ShouldAdd => ChanceOfPlayingNextRound == "None"
                                 // Harry Kane
                                 && Id != 500
                                 // David Raya
                                 && Id != 113;

        public override string ToString()
        {
            return $"{FirstName}_{Surname}_{Id}".Replace(" ", "_");
        }
    }
}