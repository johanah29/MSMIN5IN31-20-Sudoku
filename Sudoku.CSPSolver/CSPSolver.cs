using System;
using System.Diagnostics;
using aima.core.search.csp;
using Sudoku.Core;



namespace Sudoku.CSPSolver
{
    public class CSPSolver:ISudokuSolver
    {

        public CSPSolver()
        {
            // Définition d'une stratégie de résolution
            var objStrategyInfo = new CSPStrategyInfo();
            objStrategyInfo.EnableLCV = true;
            objStrategyInfo.Inference = CSPInference.ForwardChecking;
            objStrategyInfo.Selection = CSPSelection.MRVDeg;
            objStrategyInfo.StrategyType = CSPStrategy.ImprovedBacktrackingStrategy;
            objStrategyInfo.MaxSteps = 5000;
            _Strategy = objStrategyInfo.GetStrategy();
        }


        private SolutionStrategy _Strategy;


        public Core.Sudoku Solve(Core.Sudoku s)
        {
			//Construction du CSP

            var objCSP = SudokuCSPHelper.GetSudokuCSP(s);

            // Résolution du Sudoku
            var objChrono = Stopwatch.StartNew();
            var assignation = _Strategy.solve(objCSP);
            Console.WriteLine($"Pure solve Time : {objChrono.Elapsed.TotalMilliseconds} ms");


            //Traduction du Sudoku
            SudokuCSPHelper.SetValuesFromAssignment(assignation, s);

            return s;
		}
    }

    

}
