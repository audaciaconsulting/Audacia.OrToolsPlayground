using System.Collections.Generic;
using System.Linq;
using Audacia.OrToolsPlayground.Common.Exceptions;
using Audacia.OrToolsPlayground.Examples.Sudoku.Models;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.Sudoku;

public class SudokuSolver
{
    private readonly SudokuInput _input;

    public SudokuSolver(SudokuInput input)
    {
        _input = input;
    }

    public ushort?[][] Solve()
    {
        var model = new SudokuCpModel();

        InitialiseVars(model);
        AddKnownValues(model);
        AddConstraints(model);

        return GetSolution(model);
    }

    private ushort?[][] GetSolution(SudokuCpModel model)
    {
        var solver = new CpSolver();
        var result = solver.Solve(model);

        if (result == CpSolverStatus.Optimal)
        {
            for (var columnIndex = 0; columnIndex < 9; columnIndex++)
            {
                for (var rowIndex = 0; rowIndex < 9; rowIndex++)
                {
                    _input.KnownValues[columnIndex][rowIndex] = (ushort)solver.Value(model.Cells[rowIndex, columnIndex]);
                }
            }

            return _input.KnownValues;
        }

        throw OptimisationException.NoSolution();
    }

    private static void AddConstraints(SudokuCpModel model)
    {
        foreach (var row in model.Rows)
        {
            model.AddAllDifferent(row);
        }

        foreach (var column in model.Columns)
        {
            model.AddAllDifferent(column);
        }

        foreach (var rowSquareIndex in Enumerable.Range(0, 3))
        {
            foreach (var columnRowIndex in Enumerable.Range(0, 3))
            {
                var startRow = rowSquareIndex * 3;
                var startColumn = columnRowIndex * 3;
                List<IntVar> cells =
                [
                    model.Cells[startRow, startColumn],
                    model.Cells[startRow + 1, startColumn],
                    model.Cells[startRow + 2, startColumn],
                    model.Cells[startRow, startColumn + 1],
                    model.Cells[startRow + 1, startColumn + 1],
                    model.Cells[startRow + 2, startColumn + 1],
                    model.Cells[startRow, startColumn + 2],
                    model.Cells[startRow + 1, startColumn + 2],
                    model.Cells[startRow + 2, startColumn + 2],
                ];
                model.AddAllDifferent(cells);
            }
        }
    }

    private void AddKnownValues(SudokuCpModel model)
    {
        for (var columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            for (var rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                var knownValue = _input.KnownValues[columnIndex][rowIndex];
                if (knownValue != null)
                {
                    model.Add(model.Cells[rowIndex, columnIndex] == (long)knownValue);
                }
            }
        }
    }

    private static void InitialiseVars(SudokuCpModel model)
    {
        for (var columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            List<IntVar> row = [];
            for (var rowIndex = 0; rowIndex < 9; rowIndex++)
            {
                var intVar = model.NewIntVar(1, 9, $"cell_{columnIndex}_{rowIndex}");
                model.Cells[rowIndex, columnIndex] = intVar;
                row.Add(intVar);
            }

            model.Rows.Add(row);
        }

        for (var columnIndex = 0; columnIndex < 9; columnIndex++)
        {
            model.Columns.Add(model.Rows.Select(r => r.ElementAt(columnIndex)).ToList());
        }
    }
}