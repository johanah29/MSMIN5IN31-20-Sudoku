using System;
using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Mathematics;
using BenchmarkDotNet.Order;
using Sudoku.Core;

namespace Sudoku.Benchmark
{
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
                    
                    //.WithInvocationCount(5)
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

        public int NbPuzzles { get; set; } = 10;

        [ParamsAllValues]
        public SudokuDifficulty Difficulty { get; set; }

        public IDictionary<SudokuDifficulty, IList<Core.Sudoku>> AllPuzzles { get; set; }
        public IList<Core.Sudoku> IterationPuzzles { get; set; }

        [ParamsSource(nameof(GetSolvers))]
        public SolverPresenter SolverPresenter { get; set; }

        public IEnumerable<SolverPresenter> GetSolvers()
        {
            return Core.Sudoku.GetSolvers().Select(s=>new SolverPresenter() {Solver = s});
        }

        [Benchmark(Description = "Benchmarking Sudoku Solvers")]
        public void Benchmark()
        {
            foreach (var puzzle in IterationPuzzles)
            {
                var solution = SolverPresenter.Solver.Solve( puzzle.CloneSudoku());
                if (!solution.IsValid(puzzle))
                {
                    throw new ApplicationException($"sudoku has {solution.NbErrors(puzzle)} errors");
                }
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
    }


}