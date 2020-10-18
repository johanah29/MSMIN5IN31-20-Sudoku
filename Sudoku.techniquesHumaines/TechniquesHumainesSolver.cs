using System;
using System.Collections.Generic;
using System.Linq;

namespace Sudoku.techniquesHumaines
{
    public class TechniquesHumainesSolver : Sudoku.Core.ISudokuSolver
    {

        private class SolverTechnique
        {
            public readonly Func<Core.Sudoku, bool> Function;
            public readonly bool CanRepeat;

            public SolverTechnique(Func<Core.Sudoku, bool> function, string url, bool canRepeat = false)
            {
                // I know the text is unused
                Function = function;
                CanRepeat = canRepeat;
            }
        }

        public Core.Sudoku Solve(Core.Sudoku s)
        {
            bool solved; // If this is true after a segment, the puzzle is solved and we can break
            do
            {
                solved = true;

                bool changed = false;
                // Check for naked singles or a completed puzzle
                for (int x = 0; x < 9; x++)
                {
                    for (int y = 0; y < 9; y++)
                    {
                        int cell = s.GetCell(x, y);
                        if (cell == 0)
                        {
                            solved = false;
                            // Check for naked singles
                            int[] a = s.GetPossibilities(x,y); // Copy
                            if (a.Length == 1)
                            {
                                s.SetCell(x, y, a[0]);
                                changed = true;
                            }
                        }
                    }
                }
                // Solved or failed to solve
                if (solved || (!changed && !RunTechnique(s)))
                {
                    break;
                }
            } while (true);

            return s;
        }

        static readonly SolverTechnique[] techniques = new SolverTechnique[]
        {
            
        };
        SolverTechnique previousTechnique;

        bool RunTechnique(Core.Sudoku Puzzle)
        {
            foreach (SolverTechnique t in techniques)
            {
                // Skip the previous technique unless it is good to repeat it
                if ((t != previousTechnique || t.CanRepeat) && t.Function.Invoke(Puzzle))
                {
                    previousTechnique = t;
                    return true;
                }
            }
            if (previousTechnique == null)
            {
                return false;
            }
            else
            {
                return previousTechnique.Function.Invoke(Puzzle);
            }
        }


    }
}
