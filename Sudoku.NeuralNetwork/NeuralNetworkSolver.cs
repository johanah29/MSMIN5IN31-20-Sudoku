using System;
using Keras.Models;
using Sudoku.Core;


namespace Sudoku.NeuralNetwork
{
    public class NeuralNetworkSolver : ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            var solution = (Core.Sudoku)s.Clone();
            var model = BaseModel.LoadModel(DataSetHelper.GetFullPath(@"Sudoku.NeuralNetwork\Models\sudoku.model"));

            solution = Model.SolveSudoku(solution, model);
            Console.WriteLine(solution);

            return solution;
        }
    }
}

