using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class PickFplTeamCpModel : CpModel
{
    /// <summary>
    /// A list of all players and whether they've been selected or not.
    /// </summary>
    public List<FplPlayerSelectionVar> Selections = new();

    /// <summary>
    /// A dictionary of TeamId to a list of variables representing player selections.
    /// This is used to ensure we don't pick too many players from a single team.
    /// </summary>
    public Dictionary<string, List<IntVar>> TeamSelectionCounts = new();

    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.GK"/> selection.
    /// </summary>
    public List<IntVar> SelectedGoalkeepers => Selections.Select(s => s.SelectedGoalkeeper).ToList();

    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.DEF"/> selection.
    /// </summary>
    public List<IntVar> SelectedDefenders => Selections.Select(s => s.SelectedDefender).ToList();

    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.MID"/> selection.
    /// </summary>
    public List<IntVar> SelectedMidfielders => Selections.Select(s => s.SelectedMidfielder).ToList();

    /// <summary>
    /// Gets a list of all players' variables representing <see cref="PlayerPosition.FWD"/> selection.
    /// </summary>
    public List<IntVar> SelectedForwards => Selections.Select(s => s.SelectedForward).ToList();
}