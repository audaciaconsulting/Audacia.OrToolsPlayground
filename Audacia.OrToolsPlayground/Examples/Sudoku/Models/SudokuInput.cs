namespace Audacia.OrToolsPlayground.Examples.Sudoku.Models;

public class SudokuInput
{
    /// <summary>
    /// The unsolved sudoku puzzle, with known values and unknown values represented as nullable integers.
    /// Null = unknown value.
    /// Not null = known value.
    /// </summary>
    public required ushort?[][] KnownValues { get; set; }
}