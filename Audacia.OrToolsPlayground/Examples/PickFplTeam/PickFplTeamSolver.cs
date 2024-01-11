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

                var teamSelected = model.NewBoolVar($"{fplPlayer}_selected_{fplPlayer.TeamId}");

                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedGoalkeeper);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedDefender);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedMidfielder);
                model.Add(teamSelected == 1).OnlyEnforceIf(selection.SelectedForward);
                if (model.TeamSelectionCounts.ContainsKey(fplPlayer.TeamId))
                {
                    model.TeamSelectionCounts[fplPlayer.TeamId].Add(teamSelected);
                }
                else
                {
                    model.TeamSelectionCounts.Add(fplPlayer.TeamId, new List<IntVar> { teamSelected });
                }
            }
        }

        private void AddObjective(PickFplTeamCpModel model)
        {
            var points = _request.Players.Select(r => r.Points).ToList();
            var goalkeeperPoints = LinearExpr.ScalProd(model.SelectedGoalkeepers, points);
            var defenderPoints = LinearExpr.ScalProd(model.SelectedDefenders, points);
            var midfielderPoints = LinearExpr.ScalProd(model.SelectedMidfielders, points);
            var forwardPoints = LinearExpr.ScalProd(model.SelectedForwards, points);
            model.Maximize(goalkeeperPoints + defenderPoints + midfielderPoints + forwardPoints);
        }

        private void AddConstraints(PickFplTeamCpModel model)
        {
            AddMaxPerTeamConstraint(model);

            AddPositionConstraints(model);

            AddCostConstraint(model);
        }

        private void AddCostConstraint(PickFplTeamCpModel model)
        {
            var allCosts = _request.Players.Select(r => r.Cost).ToList();

            var goalkeeperCost = LinearExpr.ScalProd(model.SelectedGoalkeepers, allCosts);
            var defenderCost = LinearExpr.ScalProd(model.SelectedDefenders, allCosts);
            var midfielderCost = LinearExpr.ScalProd(model.SelectedMidfielders, allCosts);
            var forwardCost = LinearExpr.ScalProd(model.SelectedForwards, allCosts);
            model.Add((goalkeeperCost + defenderCost + midfielderCost + forwardCost) <= _request.Options.Budget);
        }

        private void AddPositionConstraints(PickFplTeamCpModel model)
        {
            var numberSelectedGoalkeepers = new SumArray(model.SelectedGoalkeepers);
            var numberSelectedDefenders = new SumArray(model.SelectedDefenders);
            var numberSelectedMidfielders = new SumArray(model.SelectedMidfielders);
            var numberSelectedForwards = new SumArray(model.SelectedForwards);
            model.Add(numberSelectedGoalkeepers == _request.Options.NumberGoalkeepers);
            model.Add(numberSelectedDefenders == _request.Options.NumberDefenders);
            model.Add(numberSelectedMidfielders == _request.Options.NumberMidfielders);
            model.Add(numberSelectedForwards == _request.Options.NumberForwards);
        }

        private void AddMaxPerTeamConstraint(PickFplTeamCpModel model)
        {
            foreach (var (_, selectedPlayers) in model.TeamSelectionCounts)
            {
                model.Add(new SumArray(selectedPlayers) <= _request.Options.MaxPlayersPerTeam);
            }
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
            var isGoalkeeper = fplPlayer.Position == PlayerPosition.Goalkeeper;
            var isDefender = fplPlayer.Position == PlayerPosition.Defender;
            var isMidfielder = fplPlayer.Position == PlayerPosition.Midfielder;
            var isForward = fplPlayer.Position == PlayerPosition.Forward;
            
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