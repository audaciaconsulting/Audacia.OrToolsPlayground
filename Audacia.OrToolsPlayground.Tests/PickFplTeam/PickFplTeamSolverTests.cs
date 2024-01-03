using Audacia.OrToolsPlayground.Examples.PickFplTeam;
using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.PickFplTeam;

public class PickFplTeamSolverTests
{
    [Fact]
    public void UsesCsv_CreatesOutput()
    {
        var solver = new PickFplTeamSolver(PickFplTeamModel.FromCsv(new PickFplTeamOptions
        {
            MaxPlayersPerTeam = 3,
            Budget = 1002,
            NumberGoalkeepers = 2,
            NumberDefenders = 5,
            NumberMidfielders = 5,
            NumberForwards = 3
        }));
        
        var output = solver.Solve();
    }
}