using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Sudoku.Core;

namespace Sudoku.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
               

                Console.WriteLine("Benchmarking Sudoku Solvers");

                while (true)
                {
                    Console.WriteLine("Select Mode: 1-Single Solver Test, 2-Complete Benchmark");
                    var strMode = Console.ReadLine();
                    int intMode;
                    int.TryParse(strMode, out intMode);
                    switch (intMode)
                    {
                        case 1:
                            SingleSolverTest();
                            break;
                        default:
                            var summary = BenchmarkRunner.Run<BenchmarkSolvers>();
                            break;
                        
                    }



                   
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
        }


        private static void SingleSolverTest()
        {
            var solvers = Core.Sudoku.GetSolvers();
            Console.WriteLine("Select difficulty: 1-Easy, 2-Medium, 3-Hard");
            var strDiff = Console.ReadLine();
            int intDiff;
            int.TryParse(strDiff, out intDiff);
            SudokuDifficulty difficulty;
            switch (intDiff)
            {
                case 1:
                    difficulty = SudokuDifficulty.Easy;
                    break;
                case 2:
                    difficulty = SudokuDifficulty.Medium;
                    break;
                default:
                    difficulty = SudokuDifficulty.Hard;
                    break;
            }

            var sudokus = SudokuHelper.GetSudokus(difficulty);

            Console.WriteLine($"Choose a puzzle index between 1 and {sudokus.Count}");
            var strIdx = Console.ReadLine();
            int intIdx;
            int.TryParse(strIdx, out intIdx);
            var targetSudoku = sudokus[intIdx-1];

            Console.WriteLine("Chosen Puzzle:");
            Console.WriteLine(targetSudoku.ToString());

            Console.WriteLine("Choose a solver:");
            for (int i = 0; i < solvers.Count(); i++)
            {
                Console.WriteLine($"{(i + 1).ToString(CultureInfo.InvariantCulture)} - {solvers[i].GetType().FullName}");
            }
            var strSolver = Console.ReadLine();
            int intSolver;
            int.TryParse(strSolver, out intSolver);
            var solver = solvers[intSolver - 1];

            var cloneSudoku = targetSudoku.CloneSudoku();
            var sw = Stopwatch.StartNew();

            var solution = solver.Solve(cloneSudoku);

            var elapsed = sw.Elapsed;
            if (!solution.IsValid(targetSudoku))
            {
                Console.WriteLine($"Invalid Solution : Solution has {solution.NbErrors(targetSudoku)} errors");
                Console.WriteLine("Invalid solution:");
            }
            else
            {
                Console.WriteLine("Valid solution:");
            }
            
            Console.WriteLine(solution.ToString());
            Console.WriteLine($"Time to solution: {elapsed.TotalMilliseconds} ms");

        }

    }
}
