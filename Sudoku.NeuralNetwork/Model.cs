using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CsvHelper;
using Keras;
using Keras.Layers;
using Keras.Models;
using Numpy;


namespace Sudoku.NeuralNetwork
{
    public class Model
    {
        public static BaseModel GenerateModel()
        {
            var model = new Sequential();

            model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(), activation: "relu", padding: "same", input_shape: (9, 9, 1)));
            model.Add(new BatchNormalization());
            model.Add(new Conv2D(64, kernel_size: (3, 3).ToTuple(), activation: "relu", padding: "same"));
            model.Add(new BatchNormalization());
            model.Add(new Conv2D(128, kernel_size: (1, 1).ToTuple(), activation: "relu", padding: "same"));

            model.Add(new Flatten());
            model.Add(new Dense(81 * 9));
            model.Add(new Reshape((-1, 9)));
            model.Add(new Activation("softmax"));

            return model;
        }

        public static (BaseModel model, double accuracy) TrainAndTest(BaseModel model)
        {
            // Global parameters
            string datasetPath = @"Dataset\sudoku.csv.gz";
            int numSudokus = 1000;

            // ML parameters
            double testPercent = 0.2;
            float learningRate = .001F;
            int batchSize = 32;
            int epochs = 2;

            // Initialize dataset
            var (sPuzzles, sSols) = DataSetHelper.ParseCSV(datasetPath, numSudokus);
            var (_sPuzzzlesTrain, _sPuzzlesTest) = DataSetHelper.SplitDataSet(sPuzzles, testPercent);
            var (_sSolsTrain, _sSolsTest) = DataSetHelper.SplitDataSet(sSols, testPercent);

            // Preprocess data
            var sPuzzzlesTrain = DataSetHelper.PreprocessSudokus(_sPuzzzlesTrain);
            var sSolsTrain = DataSetHelper.PreprocessSudokus(_sSolsTrain);
            var sPuzzlesTest = DataSetHelper.PreprocessSudokus(_sPuzzlesTest);
            var sSolsTest = DataSetHelper.PreprocessSudokus(_sSolsTest);

            // Add optimizer
            var adam = new Keras.Optimizers.Adam(learningRate);
            model.Compile(loss: "sparse_categorical_crossentropy", optimizer: adam);

            // Train model
            model.Fit(sPuzzzlesTrain, sSolsTrain, batch_size: batchSize, epochs: epochs);

            // Test model
            int correct = 0;
            for(int i = 0; i < sPuzzlesTest.size; i++)
            {
                // Predict result
                var prediction = Solve(sPuzzlesTest[i], model);

                // Convert to sudoku
                var sPredict = new Core.Sudoku() { Cells = prediction.flatten().astype(np.int32).GetData<int>().ToList() };
                var sSol = new Core.Sudoku() { Cells = sSolsTest[i].flatten().astype(np.int32).GetData<int>().ToList() };

                // Compare sudoku
                var same = true;
                for(int j = 0; j < 9; j++)
                {
                    for(int k = 1; k <= 9; k++)
                    {
                        if(sPredict.GetCell(j, k) != sSol.GetCell(j, k))
                        {
                            same = false;
                        }
                    }
                }

                if(same)
                {
                    correct += 1;
                }
            }

            // Calculate accuracy
            var accuracy = (correct / sPuzzlesTest.size) * 100;

            // Return
            return (model, accuracy);
        }

        public static NDarray Solve(NDarray sFeatures, BaseModel model)
        {
            NDarray prediction;

            while (true)
            {
                var output = model.Predict(sFeatures.reshape(1, 9, 9, 1));
                output = output.squeeze();
                prediction = np.argmax(output, axis: 2).reshape(9, 9) + 1;
                var proba = np.around(np.max(output, axis: new[] { 2 }).reshape(9, 9), 2);

                sFeatures = (sFeatures + 0.5) * 9;

                var mask = sFeatures.@equals(0);
                if (((int)mask.sum()) == 0)
                {
                    break;
                }

                var probNew = proba * mask;
                var ind = (int)np.argmax(probNew);
                var (x, y) = ((ind / 9), ind % 9);

                var val = prediction[x][y];

                sFeatures[x][y] = val;
                sFeatures = (sFeatures / 9) - 0.5;
            }

            return prediction;
        }

        public static Core.Sudoku SolveSudoku(Core.Sudoku s, BaseModel model)
        {
            NDarray sFeatures = np.array(s.Cells.ToArray()).reshape(9, 9);
            sFeatures = (sFeatures / 9) - 0.5;

            var prediction = Solve(sFeatures, model);

            return new Core.Sudoku() { Cells = prediction.flatten().astype(np.int32).GetData<int>().ToList() };
        }
    }
}
