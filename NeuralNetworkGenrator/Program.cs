using System;
using Sudoku.NeuralNetwork;

namespace NeuralNetworkGenrator
{
    class Program
    {
        static void Main(string[] args)
        {
            var model = Model.GenerateModel();
            double accuracy;

            (model, accuracy) = Model.TrainAndTest(model);

            Console.WriteLine("Accuracy : " + accuracy + "%");
        }
    }
}
