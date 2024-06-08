using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Audacia.Random.Extensions;

namespace Audacia.OrToolsPlayground.Tests.PickFplTeam.Builders;

public class FplPlayerBuilder(string team, PlayerPosition position)
{
    private decimal _rawCost;
    private int _percentage;

    public FplPlayerBuilder WithCost(decimal cost)
    {
        _rawCost = cost;
        return this;
    }

    public FplPlayerBuilder WithSelectedByPercentage(int percentage)
    {
        _percentage = percentage;
        return this;
    }

    public FplPlayer Build()
    {
        return new FplPlayer
        {
            Name = new System.Random().Surname(),
            CostMillions = _rawCost,
            Team = team,
            Position = position,
            PercentSelectedBy = _percentage
        };
    }
}