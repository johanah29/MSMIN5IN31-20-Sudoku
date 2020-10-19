using System;
using Sudoku.Core;
namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmSolver: ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            
            var sudokuTab =  (Core.Sudoku)s.Clone();
            Console.WriteLine("Begin solving Sudoku using combinatorial evolution");
            Console.WriteLine("The Sudoku is:");

            var sudoku = Sudoku.Easy;
            Console.WriteLine(sudoku.ToString());

            const int numOrganisms = 200;
            const int maxEpochs = 5000;
            const int maxRestarts = 40;
            Console.WriteLine($"Setting numOrganisms: {numOrganisms}");
            Console.WriteLine($"Setting maxEpochs: {maxEpochs}");
            Console.WriteLine($"Setting maxRestarts: {maxRestarts}");

            var solver = new SudokuSolver();
            var solvedSudoku = solver.Solve(sudoku, numOrganisms, maxEpochs, maxRestarts);

            Console.WriteLine("Best solution found:");
            Console.WriteLine(solvedSudoku.ToString());
            Console.WriteLine(solvedSudoku.Error == 0 ? "Success" : "Did not find optimal solution");
            Console.WriteLine("End Sudoku using combinatorial evolution");

            return sudokuTab;
            
        }
    }
}
