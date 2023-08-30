using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class FplPlayerSelectionVar
{
    public IntVar SelectedGoalkeeper { get; set; } = null!;
    public IntVar SelectedDefender { get; set; } = null!;
    public IntVar SelectedMidfielder { get; set; } = null!;
    public IntVar SelectedForward { get; set; } = null!;
}