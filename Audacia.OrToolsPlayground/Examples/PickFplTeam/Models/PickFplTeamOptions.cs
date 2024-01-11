namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class PickFplTeamOptions
{
    public int MaxPlayersPerTeam { get; set; } = int.MaxValue;

    public int Budget { get; set; } = int.MaxValue;

    public int NumberGoalkeepers { get; set; }

    public int NumberDefenders { get; set; }

    public int NumberMidfielders { get; set; }

    public int NumberForwards { get; set; }
}