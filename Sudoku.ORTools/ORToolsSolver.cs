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
            //Sudoku -> Tableau
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

            //Création de la grille de solution
            IntVar[,] grid = solver.MakeIntVarMatrix(n, n, 1, 9, "grid");
            IntVar[] grid_flat = grid.Flatten();

            //Tableau -> Solver
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

            //Un chiffre ne figure qu'une seule fois par ligne/colonne/cellule
            foreach (int i in RANGE)
            {
                // Lignes
                solver.Add((from j in RANGE
                            select grid[i, j]).ToArray().AllDifferent());

                // Colonnes
                solver.Add((from j in RANGE
                            select grid[j, i]).ToArray().AllDifferent());
            }
            
            //Cellules
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

            //Début de la résolution
            DecisionBuilder db = solver.MakePhase(grid_flat,
                                                  Solver.INT_VAR_SIMPLE,
                                                  Solver.INT_VALUE_SIMPLE);
            solver.NewSearch(db);

            // Solver -> Liste
            var gridToSudoku = new List<int>();
            while (solver.NextSolution())
            {
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        gridToSudoku.Add((int)grid[i, j].Value());
                    }
                }
            }
            solver.EndSearch();

            //Liste -> Sudoku
            return new Sudoku.Core.Sudoku(gridToSudoku);
        }
        public static void Main(String[] args)
        {

        }
    }
}
