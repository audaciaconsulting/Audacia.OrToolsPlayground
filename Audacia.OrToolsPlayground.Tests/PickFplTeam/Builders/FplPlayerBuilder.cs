using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Audacia.Random.Extensions;

namespace Audacia.OrToolsPlayground.Tests.PickFplTeam.Builders;

public class FplPlayerBuilder(int team, PlayerPosition position)
{
    private int _cost;
    private int _points;

    public FplPlayerBuilder WithCost(int cost)
    {
        _cost = cost;
        return this;
    }

    public FplPlayerBuilder WithPoints(int points)
    {
        _points = points;
        return this;
    }
    
    public FplPlayer Build()
    {
        return new FplPlayer
        {
            Form = 10,
            ChanceOfPlayingNextRound = "None",
            FirstName = new System.Random().Forename(),
            Surname = new System.Random().Surname(),
            Cost = _cost,
            Points = _points,
            TeamId = team,
            Position = position
        };
    }
}

public static class Team
{
    public const int Liverpool = 1;
    public const int ManchesterCity = 2;
    public const int ManchesterUnited = 3;
    public const int Chelsea = 4;
    public const int TottenhamHotspur = 5;
    public const int Arsenal = 6;
    public const int LeicesterCity = 7;
    public const int WestHamUnited = 8;
    public const int Everton = 9;
    public const int LeedsUnited = 10;
    public const int AstonVilla = 11;
    public const int NewcastleUnited = 12;
    public const int WolverhamptonWanderers = 13;
    public const int CrystalPalace = 14;
    public const int Southampton = 15;
    public const int BrightonAndHoveAlbion = 16;
    public const int Burnley = 17;
    public const int Fulham = 18;
    public const int WestBromwichAlbion = 19;
    public const int SheffieldUnited = 20;
}