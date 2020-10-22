using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Compression;
using CsvHelper;
using System.Globalization;
using System.Linq;
using Numpy;

namespace Sudoku.NeuralNetwork
{
    public class DataSetHelper
    {
		public static (List<Core.Sudoku> sPuzzles, List<Core.Sudoku> sSols) ParseCSV(string path, int numSudokus)
		{
			var sPuzzles = new List<Core.Sudoku>();
			var sSols = new List<Core.Sudoku>();

			using (var compressedStream = File.OpenRead(GetFullPath(path)))
			using (var decompressedStream = new GZipStream(compressedStream, CompressionMode.Decompress))
			using (var reader = new StreamReader(decompressedStream))
			using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
			{
				csv.Configuration.Delimiter = ",";
				csv.Read();
				csv.ReadHeader();
				var currentNb = 0;
				while (csv.Read() && currentNb < numSudokus)
				{
					sPuzzles.Add(Core.Sudoku.Parse(csv.GetField<string>("quizzes")));
					sSols.Add(Core.Sudoku.Parse(csv.GetField<string>("solutions")));

					currentNb++;
				}
			}
			return (sPuzzles, sSols);
		}

		public static (List<Core.Sudoku> train, List <Core.Sudoku> test) SplitDataSet(List<Core.Sudoku> sudokus, double testPercent)
        {
			int listSize = sudokus.Count;
			int trainSize = (int)((1 - testPercent) * listSize);

			List<Core.Sudoku> train = sudokus.Take(trainSize).ToList();
			List<Core.Sudoku> test = sudokus.Skip(trainSize).Take(listSize - trainSize).ToList();

			return (train, test);
        }

		public static NDarray PreprocessSudokus(List<Core.Sudoku> sudokus)
        {
			List<NDarray> rawRes = new List<NDarray>();
			foreach(Core.Sudoku sudoku in sudokus)
            {
				rawRes.Add(np.array(sudoku.Cells.ToArray()).reshape(9, 9, 1));
			}

			NDarray res = np.array(rawRes.ToArray());
			res = (res / 9) - 0.5;

			return res;
        }

		public static string GetFullPath(string relativePath)
		{
			return System.IO.Path.Combine(Environment.CurrentDirectory, @"..\..\..\..\" + relativePath);
		}
	}
}
