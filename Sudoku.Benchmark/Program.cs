using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using Sudoku.Core;

namespace Sudoku.Benchmark
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var solvers = GetSolvers();

                Console.WriteLine("Benchmarking Sudoku Solvers");

                while (true)
                {
                    Console.WriteLine("Select difficulty: 1-Easy, 2-Medium, 3-Hard");
                    var strDiff = Console.ReadLine();
                    int intDiff;
                    int.TryParse(strDiff, out intDiff);
                    string fileName;
                    switch (intDiff)
                    {
                        case 1:
                            fileName = "Sudoku_Easy50.txt";
                            break;
                        case 2:
                            fileName = "Sudoku_hardest.txt";
                            break;
                        default:
                            fileName = "Sudoku_top95.txt";
                            break;
                    }

                    string filePath = $@"..\..\..\..\Samples\{fileName}";
                    var sudokus = Core.Sudoku.ParseFile(filePath);

                    Console.WriteLine("Choose a solver:");
                    for (int i = 0; i < solvers.Count(); i++)
                    {
                        Console.WriteLine($"{(i + 1).ToString(CultureInfo.InvariantCulture)} - {solvers[i].GetType().FullName}");
                    }
                    var strSolver = Console.ReadLine();
                    int intSolver;
                    int.TryParse(strSolver, out intSolver);
                    var solver = solvers[intSolver - 1];
                    var targetSudoku = sudokus[0];
                    Console.WriteLine("Sudoku à résoudre:");
                    Console.WriteLine(targetSudoku.ToString());
                    var sw = Stopwatch.StartNew();

                    var solution = solver.Solve(targetSudoku);

                    var elapsed = sw.Elapsed;
                    Console.WriteLine("solution:");
                    Console.WriteLine(solution.ToString());
                    Console.WriteLine($"Time to solution: {elapsed.TotalMilliseconds} ms");
                }
              
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            
            
        }



        private static IList<ISudokuSolver> GetSolvers()
        {
            var solvers = new List<ISudokuSolver>();

            foreach (var file in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory))
            {
                if (file.EndsWith("dll") && !(Path.GetFileName(file).StartsWith("libz3")))
                {
                    try
                    {
                        var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
                        foreach (var type in assembly.GetTypes())
                        {
                            if (typeof(ISudokuSolver).IsAssignableFrom(type) && !(typeof(ISudokuSolver) == type))
                            {
                                var solver = (ISudokuSolver)Activator.CreateInstance(type);
                                solvers.Add(solver);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                    }
                    
                }

               

            }

            return solvers;
        }



    }
}
