using System.Collections.Generic;
using System.Linq;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam.Models
{
    public class FplTeamSelection
    {
        public List<FplPlayer> SelectedPlayers { get; set; } = new List<FplPlayer>();

        public long Cost => SelectedPlayers.Sum(sp => sp.Cost);
    }
}