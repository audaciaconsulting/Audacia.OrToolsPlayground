using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Exceptions;
using Audacia.OrToolsPlayground.Examples.PickFplTeam.Models;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.PickFplTeam
{
    public class PickFplTeamSolver
    {
        private readonly PickFplTeamModel _request;

        public PickFplTeamSolver(PickFplTeamModel request)
        {
            _request = request;
        }

        public FplTeamSelection Solve()
        {
            var model = new PickFplTeamCpModel();

            InitialiseVars(model);

            AddConstraints(model);

            AddObjective(model);

            return Solve(model);
        }

        private FplTeamSelection Solve(PickFplTeamCpModel model)
        {
            var cpSolver = new CpSolver();
            var status = cpSolver.Solve(model);
            if (status != CpSolverStatus.Optimal)
            {
                throw OptimisationException.SubOptimalCpSolution(status);
            }

            var output = new FplTeamSelection();
            var index = 0;
            foreach (var fplPlayer in _request.Players)
            {
                var selected = cpSolver.BooleanValue(model.SelectedGoalkeepers[index])
                               || cpSolver.BooleanValue(model.SelectedDefenders[index])
                               || cpSolver.BooleanValue(model.SelectedMidfielders[index])
                               || cpSolver.BooleanValue(model.SelectedForwards[index]);
                if (selected)
                {
                    output.SelectedPlayers.Add(fplPlayer);
                }

                index++;
            }

            return output;
        }

        private void InitialiseVars(PickFplTeamCpModel model)
        {
            foreach (var fplPlayer in _request.Players)
            {
                var selection = GetSelectionVar(model, fplPlayer);

                model.Selections.Add(selection);

                var teamSelected = model.NewBoolVar($"{fplPlayer}_selected_{fplPlayer.Team}");

                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedGoalkeeper);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedDefender);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedMidfielder);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedForward);
                if (model.TeamSelectionCounts.ContainsKey(fplPlayer.Team))
                {
                    model.TeamSelectionCounts[fplPlayer.Team].Add(teamSelected);
                }
                else
                {
                    model.TeamSelectionCounts.Add(fplPlayer.Team, [teamSelected]);
                }
            }
        }

        private void AddObjective(PickFplTeamCpModel model)
        {
            // Get the scale product of the selection booleans and multiply by each player's selection, maximizing the total.
            var allPlayerSelections = _request.Players.Select(p => p.PercentSelectedBy).ToList();
            var goalkeepersSelection = LinearExpr.ScalProd(model.SelectedGoalkeepers, allPlayerSelections);
            var defendersSelection = LinearExpr.ScalProd(model.SelectedDefenders, allPlayerSelections);
            var midfieldersSelection = LinearExpr.ScalProd(model.SelectedMidfielders, allPlayerSelections);
            var forwardsSelection = LinearExpr.ScalProd(model.SelectedForwards, allPlayerSelections);

            model.Maximize(goalkeepersSelection + defendersSelection + midfieldersSelection + forwardsSelection);
        }

        private void AddConstraints(PickFplTeamCpModel model)
        {
            AddPositionConstraints(model);

            AddMaxPerTeamConstraint(model);

            AddCostConstraint(model);
        }

        private void AddPositionConstraints(PickFplTeamCpModel model)
        {
            // Sum up the selections for each position, and ensure it matches the number of players required per position.
            model.Add(new SumArray(model.SelectedGoalkeepers) == _request.Options.NumberGoalkeepers);
            model.Add(new SumArray(model.SelectedDefenders) == _request.Options.NumberDefenders);
            model.Add(new SumArray(model.SelectedMidfielders) == _request.Options.NumberMidfielders);
            model.Add(new SumArray(model.SelectedForwards) == _request.Options.NumberForwards);
        }

        private void AddMaxPerTeamConstraint(PickFplTeamCpModel model)
        {
            // Sum up the selections for each team, and ensure it doesn't exceed the maximum number of players per team.
            foreach (var (_, selections) in model.TeamSelectionCounts)
            {
                model.Add(new SumArray(selections) <= _request.Options.MaxPlayersPerTeam);
            }
        }

        private void AddCostConstraint(PickFplTeamCpModel model)
        {
            // Get the scale product of the selection booleans and multiply by each player's cost.
            var allPlayerCosts = _request.Players.Select(p => p.Cost).ToList();
            var goalkeepersCost = LinearExpr.ScalProd(model.SelectedGoalkeepers, allPlayerCosts);
            var defendersCost = LinearExpr.ScalProd(model.SelectedDefenders, allPlayerCosts);
            var midfieldersCost = LinearExpr.ScalProd(model.SelectedMidfielders, allPlayerCosts);
            var forwardsCost = LinearExpr.ScalProd(model.SelectedForwards, allPlayerCosts);

            model.Add(goalkeepersCost + defendersCost + midfieldersCost + forwardsCost <= _request.Options.Budget);
        }

        private static FplPlayerSelectionVar GetSelectionVar(PickFplTeamCpModel model, FplPlayer fplPlayer)
        {
            var selection = new FplPlayerSelectionVar
            {
                SelectedGoalkeeper = model.NewBoolVar($"{fplPlayer}_selected_gk"),
                SelectedDefender = model.NewBoolVar($"{fplPlayer}_selected_def"),
                SelectedMidfielder = model.NewBoolVar($"{fplPlayer}_selected_mid"),
                SelectedForward = model.NewBoolVar($"{fplPlayer}_selected_fwd"),
            };
            var isGoalkeeper = fplPlayer.Position == PlayerPosition.GK;
            var isDefender = fplPlayer.Position == PlayerPosition.DEF;
            var isMidfielder = fplPlayer.Position == PlayerPosition.MID;
            var isForward = fplPlayer.Position == PlayerPosition.FWD;

            // Player cannot be selected for a position if they don't play in that position.
            if (!isGoalkeeper)
            {
                model.Add(selection.SelectedGoalkeeper == 0);
            }

            if (!isDefender)
            {
                model.Add(selection.SelectedDefender == 0);
            }

            if (!isMidfielder)
            {
                model.Add(selection.SelectedMidfielder == 0);
            }

            if (!isForward)
            {
                model.Add(selection.SelectedForward == 0);
            }

            return selection;
        }
    }
}