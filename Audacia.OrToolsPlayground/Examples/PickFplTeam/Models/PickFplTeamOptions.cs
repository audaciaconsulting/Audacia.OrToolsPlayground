namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class PickFplTeamOptions
{
    public int MaxPlayersPerTeam { get; set; } = int.MaxValue;

    /// <summary>
    /// Budget in millions
    /// </summary>
    public decimal BudgetMillions { get; set; } = int.MaxValue;

    /// <summary>
    /// Budget, in 10s of currency units.
    /// </summary>
    public int Budget => (int)(BudgetMillions * 10);

    public int NumberGoalkeepers { get; set; }

    public int NumberDefenders { get; set; }

    public int NumberMidfielders { get; set; }

    public int NumberForwards { get; set; }
}