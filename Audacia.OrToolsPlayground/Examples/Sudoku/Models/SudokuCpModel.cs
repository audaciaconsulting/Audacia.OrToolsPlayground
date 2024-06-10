using System.Collections.Generic;
using Google.OrTools.Sat;

namespace Audacia.OrToolsPlayground.Examples.Sudoku.Models;

public class SudokuCpModel : CpModel
{
    public List<List<IntVar>> Columns { get; set; } = [];

    public List<List<IntVar>> Rows { get; set; } = [];

    public IntVar[,] Cells { get; set; } = new IntVar[9, 9];
}