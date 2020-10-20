using System;
using Sudoku.Core;


namespace Sudoku.NeuralNetwork
{
    public class NeuralNetworkSolver : ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            var solution = (Core.Sudoku)s.Clone();
            // InferenceSudoku(solution, 0);
            return solution;
        }
    }
}

