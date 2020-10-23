using System;
using Keras;
using Keras.Models;
using Python.Runtime;
using Sudoku.Core;


namespace Sudoku.NeuralNetwork
{
    public class NeuralNetworkSolver : ISudokuSolver
    {

        public NeuralNetworkSolver()
        {
            //var distributionPath = @"C:\ProgramData\Anaconda3\envs\ml38"; // mettre votre chemin vers votre python 3.8;
            //string path = $@"{distributionPath};" + Environment.GetEnvironmentVariable("PATH", EnvironmentVariableTarget.Machine);
            //Environment.SetEnvironmentVariable("PATH", path, EnvironmentVariableTarget.Process);
            //PythonEngine.PythonHome = distributionPath;
            //Setup.UseTfKeras();
        }

        public Core.Sudoku Solve(Core.Sudoku s)
        {
            var solution = (Core.Sudoku)s.Clone();
            var model = BaseModel.LoadModel(DataSetHelper.GetFullPath(@"Sudoku.NeuralNetwork\Models\sudoku.model"));

            solution = Model.SolveSudoku(solution, model);
            Console.WriteLine(solution);

            return solution;
        }
    }
}

