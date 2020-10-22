using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Perfolizer.Horology;
using Sudoku.Core;

namespace Sudoku.Benchmark
{
    public class FiveMinutesBenchmarkSolvers : BenchmarkSolvers
    {
        public FiveMinutesBenchmarkSolvers()
        {
            MaxSolverDuration = TimeSpan.FromMinutes(5);
        }
    }



    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [Config(typeof(Config))]
    [MinColumn, MaxColumn, MeanColumn, MedianColumn]
    public class BenchmarkSolvers
    {

        private class Config : ManualConfig
        {
            public Config()
            {
#if DEBUG
                Options = Options | ConfigOptions.DisableOptimizationsValidator;
#endif
                this.AddColumn(new RankColumn(NumeralSystem.Arabic));
                AddJob(Job.Dry
                    .WithId("Solving Sudokus")
                    .WithPlatform(Platform.X64)
                    .WithJit(Jit.RyuJit)
                    .WithRuntime(CoreRuntime.Core31)
                    .WithLaunchCount(1)
                    .WithWarmupCount(1)
                    .WithIterationCount(3)
                    .WithInvocationCount(2)
                    
                );
                

            }
        }

        [GlobalSetup]
        public void GlobalSetup()
        {
            AllPuzzles = new Dictionary<SudokuDifficulty, IList<Core.Sudoku>>();
            foreach (var difficulty in Enum.GetValues(typeof(SudokuDifficulty)).Cast<SudokuDifficulty>())
            {
                AllPuzzles[difficulty] = SudokuHelper.GetSudokus(Difficulty);
            }

            
        }

        [IterationSetup]
        public void IterationSetup()
        {
            IterationPuzzles = new List<Core.Sudoku>(NbPuzzles);
            for (int i = 0; i < NbPuzzles; i++)
            {
                IterationPuzzles.Add(AllPuzzles[Difficulty][i].CloneSudoku());
            }

        }

        private static Stopwatch Clock = Stopwatch.StartNew();

        public TimeSpan MaxSolverDuration = TimeSpan.FromSeconds(40);

        public int NbPuzzles { get; set; } = 10;

        [ParamsAllValues]
        public SudokuDifficulty Difficulty { get; set; }

        public IDictionary<SudokuDifficulty, IList<Core.Sudoku>> AllPuzzles { get; set; }
        public IList<Core.Sudoku> IterationPuzzles { get; set; }

        [ParamsSource(nameof(GetSolvers))]
        public SolverPresenter SolverPresenter { get; set; }

        public IEnumerable<SolverPresenter> GetSolvers()
        {
            return Core.Sudoku.GetSolvers().Select(s => new SolverPresenter() { Solver = s });
            //return Core.Sudoku.GetSolvers().Where(s=>s.GetType().Name.ToLowerInvariant().StartsWith("z3")).Select(s=>new SolverPresenter() {Solver = s});
        }


        [Benchmark(Description = "Benchmarking Sudoku Solvers")]
        public void Benchmark()
        {
            foreach (var puzzle in IterationPuzzles)
            {
                Console.WriteLine($"Solver {SolverPresenter.ToString()} solving sudoku: \n {puzzle.ToString()}");
                var startTime = Clock.Elapsed;
                var solution = SolverPresenter.SolveWithTimeLimit( puzzle, MaxSolverDuration);
                if (!solution.IsValid(puzzle))
                {
                    throw new ApplicationException($"sudoku has {solution.NbErrors(puzzle)} errors");
                }

                var duration = Clock.Elapsed - startTime;
                var durationSeconds = (int) duration.TotalSeconds;
                var durationMilliSeconds = duration.TotalMilliseconds - (1000 * durationSeconds);
                Console.WriteLine($"Valid Solution found: \n {solution.ToString()} \n Solver {SolverPresenter.ToString()} found the solution  in {durationSeconds} s {durationMilliSeconds} ms");
            }
        }

    }

    public class SolverPresenter
    {

        

        public ISudokuSolver Solver { get; set; }

        public override string ToString()
        {
            return Solver.GetType().Name;
        }

        public  Core.Sudoku SolveWithTimeLimit(Core.Sudoku puzzle, TimeSpan maxDuration)
        {
            try
            {
                Core.Sudoku toReturn = null;
               
                Task task = Task.Factory.StartNew(() => toReturn = Solver.Solve(puzzle.CloneSudoku()));
                task.Wait(maxDuration);
                if (!task.IsCompleted)
                {
                    throw new ApplicationException($"Solver {ToString()} has exceeded the maximum allowed duration {maxDuration.TotalSeconds} seconds");
                }
                return toReturn;

            }
            catch (AggregateException ae)
            {
                throw ae.InnerExceptions[0];
            }
        }

    }


}