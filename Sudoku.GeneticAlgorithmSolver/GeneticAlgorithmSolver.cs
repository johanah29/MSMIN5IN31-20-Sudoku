using System;
using GeneticSharp.Extensions.Sudoku;
using GeneticSharp.Extensions.UnitTests.Sudoku;
using Sudoku.Core;
using Sudoku = Sudoku.Core.Sudoku;

namespace Sudoku.GeneticAlgorithmSolver
{
    public class GeneticAlgorithmSolver: ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            
            var sudoku =  (Core.Sudoku)s.Clone();
            var stringsolution = sudoku.ToString();
            Console.WriteLine(sudoku);
            var sudokuboard =SudokuTestHelper.CreateBoard(stringsolution);
            var chromosome = new SudokuCellsChromosome(sudokuboard);
            //var solution = SudokuTestHelper.Eval()

            return sudoku;
            
        }
    }
}
