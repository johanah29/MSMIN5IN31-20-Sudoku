﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace Sudoku.Core
{
    /// <summary>
    /// Class that represents a Sudoku, fully or partially completed
    /// Holds a list of 81 int for cells, with 0 for empty cells
    /// Can parse strings and files from most common formats and displays the sudoku in an easy to read format
    /// </summary>
    public class Sudoku:ICloneable
    {


        public static readonly int[] Indices = Enumerable.Range(0, 9).ToArray();



        /// <summary>
        /// constructor that initializes the board with 81 cells
        /// </summary>
        /// <param name="cells"></param>
        public Sudoku(IEnumerable<int> cells)
        {
            var enumerable = cells.ToList();
            if (enumerable.Count != 81)
            {
                throw new ArgumentException("Sudoku should have exactly 81 cells", nameof(cells));
            }
            Cells = new List<int>(enumerable);
        }

        public Sudoku()
        {
        }



        // The List property makes it easier to manipulate cells,
        public List<int> Cells { get; set; } = Enumerable.Repeat(0, 81).ToList();

        /// <summary>
        /// Easy access by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <returns>value of the cell</returns>
        public int GetCell(int x, int y)
        {
            return Cells[(9 * x) + y];
        }

        /// <summary>
        /// Easy setter by row and column number
        /// </summary>
        /// <param name="x">row number (between 0 and 8)</param>
        /// <param name="y">column number (between 0 and 8)</param>
        /// <param name="value">value of the cell to set</param>
        public void SetCell(int x, int y, int value)
        {
            Cells[(9 * x) + y] = value;
        }

        /// <summary>
        /// Displays a Sudoku in an easy-to-read format
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var lineSep = new string('-', 31);
            var blankSep = new string(' ', 8);

            var output = new StringBuilder();
            output.Append(lineSep);
            output.AppendLine();

            for (int row = 1; row <= 9; row++)
            {
                output.Append("| ");
                for (int column = 1; column <= 9; column++)
                {

                    var value = Cells[(row - 1) * 9 + (column - 1)];

                    output.Append(value);
                    if (column % 3 == 0)
                    {
                        output.Append(" | ");
                    }
                    else
                    {
                        output.Append("  ");
                    }
                }

                output.AppendLine();
                if (row % 3 == 0)
                {
                    output.Append(lineSep);
                }
                else
                {
                    output.Append("| ");
                    for (int i = 0; i < 3; i++)
                    {
                        output.Append(blankSep);
                        output.Append("| ");
                    }
                }
                output.AppendLine();
            }

            return output.ToString();
        }

        /// <summary>
        /// Displays a Sudoku in an easy-to-read format
        /// </summary>
        /// <returns></returns>
        public string ToStringGenetic()
        {
            var output = new StringBuilder();

            for (int row = 1; row <= 9; row++)
            {
                for (int column = 1; column <= 9; column++)
                {

                    var value = Cells[(row - 1) * 9 + (column - 1)];

                    output.Append(value);
                }
            }
            return output.ToString();
        }


        public int[] GetPossibilities(int x, int y)
        {
            if (x < 0 || x >= 9 || y < 0 || y >= 9)
            {
                throw new ApplicationException("Invalid Coodrindates");
            }

            bool[] used = new bool[9];
            for (int i = 0; i < 9; i++)
            {
                used[i] = false;
            }

            for (int ix = 0; ix < 9; ix++)
            {
                if (GetCell(ix, y) != 0)
                {
                    used[GetCell(ix, y) - 1] = true;
                }
            }

            for (int iy = 0; iy < 9; iy++)
            {
                if (GetCell(x, iy) != 0)
                {
                    used[GetCell(x, iy) - 1] = true;
                }
            }

            int sx = (x / 3) * 3;
            int sy = (y / 3) * 3;

            for (int ix = 0; ix < 3; ix++)
            {
                for (int iy = 0; iy < 3; iy++)
                {
                    if (GetCell(sx + ix, sy + iy) != 0)
                    {
                        used[GetCell(sx + ix, sy + iy) - 1] = true;
                    }
                }
            }

            List<int> res = new List<int>();

            for (int i = 0; i < 9; i++)
            {
                if (used[i] == false)
                {
                    res.Add(i + 1);
                }
            }

            return res.ToArray();
        }



        /// <summary>
        /// Parses a single Sudoku
        /// </summary>
        /// <param name="sudokuAsString">the string representing the sudoku</param>
        /// <returns>the parsed sudoku</returns>
        public static Sudoku Parse(string sudokuAsString)
        {
            return ParseMulti(new[] { sudokuAsString })[0];
        }


        /// <summary>
        /// Parses a file with one or several sudokus
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseFile(string fileName)
        {
            return ParseMulti(File.ReadAllLines(fileName));
        }

        /// <summary>
        /// Parses a list of lines into a list of sudoku, accounting for most cases usually encountered
        /// </summary>
        /// <param name="lines">the lines of string to parse</param>
        /// <returns>the list of parsed Sudokus</returns>
        public static List<Sudoku> ParseMulti(string[] lines)
        {
            var toReturn = new List<Sudoku>();
            var cells = new List<int>(81);
            // we ignore lines not starting with a sudoku character
            foreach (var line in lines.Where(l => l.Length > 0
                                                 && IsSudokuChar(l[0])))
            {
                foreach (char c in line)
                {
                    //we ignore lines not starting with cell chars
                    if (IsSudokuChar(c))
                    {
                        if (char.IsDigit(c))
                        {
                            // if char is a digit, we add it to a cell
                            cells.Add((int)Char.GetNumericValue(c));
                        }
                        else
                        {
                            // if char represents an empty cell, we add 0
                            cells.Add(0);
                        }
                    }
                    // when 81 cells are entered, we create a sudoku and start collecting cells again.
                    if (cells.Count == 81)
                    {
                        toReturn.Add(new Sudoku() { Cells = new List<int>(cells) });
                        // we empty the current cell collector to start building a new Sudoku
                        cells.Clear();
                    }

                }
            }

            return toReturn;
        }


        /// <summary>
        /// identifies characters to be parsed into sudoku cells
        /// </summary>
        /// <param name="c">a character to test</param>
        /// <returns>true if the character is a cell's char</returns>
        private static bool IsSudokuChar(char c)
        {
            return char.IsDigit(c) || c == '.' || c == 'X' || c == '-';
        }

        public object Clone()
        {
            return CloneSudoku();
        }

        public Core.Sudoku CloneSudoku()
        {
            return new Sudoku(new List<int>(this.Cells));
        }


        public static IList<ISudokuSolver> GetSolvers()
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
                        Debug.WriteLine(e);
                    }

                }

            }

            return solvers;
        }


        public int NbErrors(Sudoku originalPuzzle)
        {
            // We use a large lambda expression to count duplicates in rows, columns and boxes
            var cellsToTest = this.Cells.Select((c, i) => new { index = i, cell = c }).ToList();
            var toTest = cellsToTest.GroupBy(x => x.index / 9).Select(g => g.Select(c => c.cell)) // rows
                .Concat(cellsToTest.GroupBy(x => x.index % 9).Select(g => g.Select(c => c.cell))) //columns
                .Concat(cellsToTest.GroupBy(x => x.index / 27 * 27 + x.index % 9 / 3 * 3).Select(g => g.Select(c => c.cell))); //boxes
            var toReturn = toTest.Sum(test => test.GroupBy(x => x).Select(g => g.Count() - 1).Sum()); // Summing over duplicates
            toReturn += cellsToTest.Count(x => originalPuzzle.Cells[x.index] > 0 && originalPuzzle.Cells[x.index] != x.cell); // Mask
            return toReturn;
        }

        public bool IsValid(Sudoku originalPuzzle)
        {
            return NbErrors(originalPuzzle) == 0;
        }

    }
}
