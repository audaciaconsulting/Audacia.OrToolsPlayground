using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Examples.PickFplTeam;
using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Audacia.OrToolsPlayground.Tests.PickFplTeam.Builders;
using FluentAssertions;
using Xunit;

namespace Audacia.OrToolsPlayground.Tests.PickFplTeam;

public class PickFplTeamSolverTest// for some reason we lose intellisense if we call this PickFplTeamSolverTests...
{
    [Fact]
    public void RealWorldScenario_CanFindASolution()
    {
        var solver = new PickFplTeamSolver(PickFplTeamModel.FromCsv(new PickFplTeamOptions
        {
            MaxPlayersPerTeam = 3,
            BudgetMillions = 100,
            NumberGoalkeepers = 2,
            NumberDefenders = 5,
            NumberMidfielders = 5,
            NumberForwards = 3
        }));

        var output = solver.Solve();

        output.SelectedPlayers.Should().HaveCount(15, "we should have 2 GK + 5 DEF + 5 MID + 3 FWD");
    }

    [Fact]
    public void LotsOfGoodPlayersPlayForTeam_PicksWorsePlayersToNotExceedMaxPlayersRule()
    {
        var options = new PickFplTeamOptions
        {
            MaxPlayersPerTeam = 2,
            BudgetMillions = 100,
            NumberDefenders = 3,
        };
        var goodPlayers = Enumerable.Range(0, 3)
            .Select(_ => new FplPlayerBuilder("ENG", PlayerPosition.DEF)
                .WithSelectedByPercentage(100)
                .Build())
            .ToList();
        var badPlayers = Enumerable.Range(0, 3)
            .Select(_ => new FplPlayerBuilder("GER", PlayerPosition.DEF).Build())
            .ToList();
        var model = new PickFplTeamModel(goodPlayers.Concat(badPlayers).ToList(), options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().Contain(
            p => p.Team == "GER",
            $"we can only have {options.MaxPlayersPerTeam} max players per team");
    }

    [Fact]
    public void LotsOfGoodPlayersForSpecificPosition_PicksWorsePlayersToFillQuota()
    {
        var options = new PickFplTeamOptions
        {
            NumberDefenders = 2,
            NumberMidfielders = 1,
            BudgetMillions = 100
        };
        var players = new List<FplPlayer>
        {
            new FplPlayerBuilder("ENG", PlayerPosition.DEF).WithSelectedByPercentage(100).Build(),
            new FplPlayerBuilder("ENG", PlayerPosition.DEF).WithSelectedByPercentage(100).Build(),
            new FplPlayerBuilder("ENG", PlayerPosition.DEF).WithSelectedByPercentage(100).Build(),
            new FplPlayerBuilder("ENG", PlayerPosition.MID).WithSelectedByPercentage(0).Build(),
        };
        var model = new PickFplTeamModel(players, options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().Contain(
            p => p.Position == PlayerPosition.MID,
            $"we can only have {options.MaxPlayersPerTeam} max players per team");
    }

    [Fact]
    public void GoodPlayersIsTooExpensive_PicksWorsePlayersToNotExceedBudget()
    {
        var options = new PickFplTeamOptions
        {
            BudgetMillions = 10,
            NumberGoalkeepers = 1,
            NumberDefenders = 1,
            NumberMidfielders = 1,
            NumberForwards = 1
        };
        var expensivePlayer = new FplPlayerBuilder("ENG", PlayerPosition.FWD)
            .WithCost(2.6m)
            .WithSelectedByPercentage(100)
            .Build();
        var players = new List<FplPlayer>
        {
            new FplPlayerBuilder("ENG", PlayerPosition.GK).WithCost(2.5m).Build(),
            new FplPlayerBuilder("ENG", PlayerPosition.DEF).WithCost(2.5m).Build(),
            new FplPlayerBuilder("ENG", PlayerPosition.MID).WithCost(2.5m).Build(),
            expensivePlayer,
            new FplPlayerBuilder("ENG", PlayerPosition.FWD).WithCost(2.5m).Build(),
        };
        var model = new PickFplTeamModel(players, options);
        var solver = new PickFplTeamSolver(model);

        var output = solver.Solve();

        output.SelectedPlayers.Should().NotContain(
            expensivePlayer,
            $"we cannot afford to have the player with a lot of points as it would exceed the budget of {options.Budget}");
    }
}