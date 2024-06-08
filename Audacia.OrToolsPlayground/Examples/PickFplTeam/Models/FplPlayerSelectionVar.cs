using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

/// <summary>
/// Position-specific variables for player selections.
/// Only one of these variables will ever be set to true.
/// </summary>
public class FplPlayerSelectionVar
{
    /// <summary>
    /// Gets or sets a variable set to whether we've selected this player as a <see cref="PlayerPosition.GK"/>.
    /// </summary>
    public IntVar SelectedGoalkeeper { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets a variable set to whether we've selected this player as a <see cref="PlayerPosition.DEF"/>.
    /// </summary>
    public IntVar SelectedDefender { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets a variable set to whether we've selected this player as a <see cref="PlayerPosition.MID"/>.
    /// </summary>
    public IntVar SelectedMidfielder { get; set; } = null!;
    
    /// <summary>
    /// Gets or sets a variable set to whether we've selected this player as a <see cref="PlayerPosition.FWD"/>.
    /// </summary>
    public IntVar SelectedForward { get; set; } = null!;
}