using System.Collections.Generic;
using System.Linq;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;

public class PickFplTeamCpModel : CpModel
{
    public List<FplPlayerSelectionVar> Selections = new List<FplPlayerSelectionVar>();
    
    public Dictionary<int, List<IntVar>> TeamDicts = new Dictionary<int, List<IntVar>>();

    public List<IntVar> SelectedGoalkeepers => Selections.Select(s => s.SelectedGoalkeeper).ToList();
    
    public List<IntVar> SelectedDefenders => Selections.Select(s => s.SelectedDefender).ToList();
    
    public List<IntVar> SelectedMidfielders => Selections.Select(s => s.SelectedMidfielder).ToList();
    
    public List<IntVar> SelectedForwards => Selections.Select(s => s.SelectedForward).ToList();
}