using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Examples.PickFplTeam;
using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Audacia.OrToolsPlayground.Tests.PickFplTeam.Builders;
using Audacia.Random.Extensions;
using FluentAssertions;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.PickFplTeam;

public class PickFplTeamSolverTest
{
    [Fact]
    public void RealWorldScenario_CanFindASolution()
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

        output.SelectedPlayers.Should().HaveCount(15);
    }

    [Fact]
    public void LotsOfGoodPlayersPlayForTeam_PicksWorsePlayersToNotExceedMaxPlayersRule()
    {
        var options = new PickFplTeamOptions
        {
            MaxPlayersPerTeam = 2,
            Budget = 1000,
            NumberDefenders = 3,
        };
        var goodPlayers = Enumerable.Range(0, 3)
            .Select(_ => new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Defender)
                .WithPoints(100)
                .Build())
            .ToList();
        var badPlayers = Enumerable.Range(0, 3)
            .Select(_ => new FplPlayerBuilder(Team.ManchesterUnited, PlayerPosition.Defender).Build())
            .ToList();
        var model = new PickFplTeamModel(goodPlayers.Concat(badPlayers).ToList(), options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().Contain(
            p => p.TeamId == Team.ManchesterUnited,
            $"we can only have {options.MaxPlayersPerTeam} max players per team");
    }

    [Fact]
    public void LotsOfGoodPlayersForSpecificPosition_PicksWorsePlayersToFillQuota()
    {
        var options = new PickFplTeamOptions
        {
            NumberDefenders = 2,
            NumberMidfielders = 1
        };
        var players = new List<FplPlayer>
        {
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Defender).WithPoints(100).Build(),
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Defender).WithPoints(100).Build(),
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Defender).WithPoints(100).Build(),
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Midfielder).WithPoints(0).Build(),
        };
        var model = new PickFplTeamModel(players, options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().Contain(
            p => p.Position == PlayerPosition.Midfielder,
            $"we can only have {options.MaxPlayersPerTeam} max players per team");
    }

    [Fact]
    public void GoodPlayersIsTooExpensive_PicksWorsePlayersToNotExceedBudget()
    {
        var options = new PickFplTeamOptions
        {
            Budget = 100,
            NumberGoalkeepers = 1,
            NumberDefenders = 1,
            NumberMidfielders = 1,
            NumberForwards = 1
        };
        var expensivePlayer = new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Forward)
            .WithCost(26)
            .WithPoints(100)
            .Build();
        var players = new List<FplPlayer>
        {
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Goalkeeper).WithCost(25).Build(),
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Defender).WithCost(25).Build(),
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Midfielder).WithCost(25).Build(),
            expensivePlayer,
            new FplPlayerBuilder(Team.Liverpool, PlayerPosition.Forward).WithCost(25).Build(),
        };
        var model = new PickFplTeamModel(players, options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().NotContain(
            expensivePlayer,
            $"we cannot afford to have the player with a lot of points as it would exceed the budget of {options.Budget}");
    }
}