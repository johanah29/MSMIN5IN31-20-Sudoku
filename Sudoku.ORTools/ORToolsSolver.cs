using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Google.OrTools.ConstraintSolver;
using Sudoku.Core;

namespace Sudoku.ORTools
{
    public class ORToolsSolver : ISudokuSolver
    {
        public Sudoku.Core.Sudoku Solve(Sudoku.Core.Sudoku sudoku)
        {

            int[,] sudokuInGrid = new int[9, 9];

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sudokuInGrid[i, j] = sudoku.GetCell(i, j);
                }
            }

            Solver solver = new Solver("Sudoku");

            int cell_size = 3;
            IEnumerable<int> CELL = Enumerable.Range(0, cell_size);
            int n = cell_size * cell_size;
            IEnumerable<int> RANGE = Enumerable.Range(0, n);

            // 0 marks an unknown value
            /*int[,] initial_grid = {{0,0,0,0,9,4,0,3,0},
                               {0,0,0,5,1,0,0,0,7},
                               {0,8,9,0,0,0,0,4,0},
                               {0,0,0,0,0,0,2,0,8},
                               {0,6,0,2,0,1,0,5,0},
                               {1,0,2,0,0,0,0,0,0},
                               {0,7,0,0,0,0,5,2,0},
                               {9,0,0,0,6,5,0,0,0},
                               {0,4,0,9,7,0,0,0,0}};*/




            //
            // Decision variables
            //
            IntVar[,] grid = solver.MakeIntVarMatrix(n, n, 1, 9, "grid");
            IntVar[] grid_flat = grid.Flatten();

            //
            // Constraints
            //

            // init
            foreach (int i in RANGE)
            {
                foreach (int j in RANGE)
                {
                    if (sudokuInGrid[i, j] > 0)
                    {
                        solver.Add(grid[i, j] == sudokuInGrid[i, j]);
                    }
                }
            }


            foreach (int i in RANGE)
            {

                // rows
                solver.Add((from j in RANGE
                            select grid[i, j]).ToArray().AllDifferent());

                // cols
                solver.Add((from j in RANGE
                            select grid[j, i]).ToArray().AllDifferent());

            }

            // cells
            foreach (int i in CELL)
            {
                foreach (int j in CELL)
                {
                    solver.Add((from di in CELL
                                from dj in CELL
                                select grid[i * cell_size + di, j * cell_size + dj]
                                 ).ToArray().AllDifferent());
                }
            }


            //
            // Search
            //
            DecisionBuilder db = solver.MakePhase(grid_flat,
                                                  Solver.INT_VAR_SIMPLE,
                                                  Solver.INT_VALUE_SIMPLE);

            solver.NewSearch(db);

            while (solver.NextSolution())
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        Console.Write("{0} ", grid[i, j].Value());
                    }
                    Console.WriteLine();
                }

                Console.WriteLine();
            }

            /*Console.WriteLine("\nSolutions: {0}", solver.Solutions());
            Console.WriteLine("WallTime: {0}ms", solver.WallTime());
            Console.WriteLine("Failures: {0}", solver.Failures());
            Console.WriteLine("Branches: {0} ", solver.Branches());*/

            solver.EndSearch();


            var gridToSudoku = new List<int>(81);

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    gridToSudoku.Add((int)grid[i, j].Value());
                }
            }
            return new Sudoku.Core.Sudoku(gridToSudoku);

        }


        public static void Main(String[] args)
        {
            //Solve();
        }
    }
}
