using System;
using System.Collections.Generic;
using System.IO;

namespace Sudoku.Benchmark
{

    public enum SudokuDifficulty
    {
        Easy,
        Medium,
        Hard
    }

    public class SudokuHelper
    {
        private const string PUZZLES_FOLDER_NAME = "Puzzles";

        public static List<Core.Sudoku> GetSudokus(SudokuDifficulty difficulty)
        {
            string fileName;
            switch (difficulty)
            {
                case SudokuDifficulty.Easy:
                    fileName = "Sudoku_Easy50.txt";
                    break;
                case SudokuDifficulty.Medium:
                    fileName = "Sudoku_hardest.txt";
                    break;
                default:
                    fileName = "Sudoku_top95.txt";
                    break;
            }

            
            DirectoryInfo puzzlesDirectory = null;
            var currentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            do
            {
                var subDirectories = currentDirectory.GetDirectories();
                foreach (var subDirectory in subDirectories)
                {
                    if (subDirectory.Name == PUZZLES_FOLDER_NAME)
                    {
                        puzzlesDirectory = subDirectory;
                        break;
                    }
                }
                currentDirectory = currentDirectory.Parent;
                if (currentDirectory == null)
                {
                    throw new ApplicationException("couldn't find puzzles directory");
                }
            } while (puzzlesDirectory == null);
            string filePath = $@"{puzzlesDirectory.ToString()}\{fileName}";
            var sudokus = Core.Sudoku.ParseFile(filePath);
            return sudokus;
        }



    }
}