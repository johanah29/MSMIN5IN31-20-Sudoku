using System;
using System.Threading;

namespace Sudoku.Core
{
    public class EmptySolver : ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            return s;
        }
    }
}