using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class PickFplTeamCpModel : CpModel
{
    /// <summary>
    /// A list of all players and whether they've been selected or not.
    /// </summary>
    public List<FplPlayerSelectionVar> Selections = new List<FplPlayerSelectionVar>();
    
    /// <summary>
    /// A dictionary of TeamId to a list of variables representing player selections.
    /// This is used to ensure we don't pick too many players from a single team.
    /// </summary>
    public Dictionary<int, List<IntVar>> TeamSelectionCounts = new Dictionary<int, List<IntVar>>();

    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.Goalkeeper"/> selection.
    /// </summary>
    public List<IntVar> SelectedGoalkeepers => Selections.Select(s => s.SelectedGoalkeeper).ToList();
    
    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.Defender"/> selection.
    /// </summary>
    public List<IntVar> SelectedDefenders => Selections.Select(s => s.SelectedDefender).ToList();
    
    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.Midfielder"/> selection.
    /// </summary>
    public List<IntVar> SelectedMidfielders => Selections.Select(s => s.SelectedMidfielder).ToList();
    
    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.Forward"/> selection.
    /// </summary>
    public List<IntVar> SelectedForwards => Selections.Select(s => s.SelectedForward).ToList();
}