using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Loader;
using System.Text;

namespace Sudoku.Core
{
    public class Sudoku:ICloneable
    {


        private static readonly int[] Indices = Enumerable.Range(0, 9).ToArray();


        public Sudoku(int[] grid)
        {

        }

        public Sudoku(List<int> lsol)
        {
            this.Cells = lsol;
        }

        public Sudoku()
        {
        }



        // The List property makes it easier to manipulate cells,
        public List<int> Cells { get; set; } = Enumerable.Repeat(0, 81).ToList();

        public int GetCell(int x, int y)
        {
            return Cells[(9 * x) + y];
        }

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
            foreach (var line in lines)
            {
                if (line.Length > 0)
                {
                    if (char.IsDigit(line[0]) || line[0] == '.' || line[0] == 'X' || line[0] == '-')
                    {
                        foreach (char c in line)
                        {
                            int? cellToAdd = null;
                            if (char.IsDigit(c))
                            {
                                var cell = (int)Char.GetNumericValue(c);
                                cellToAdd = cell;
                            }
                            else
                            {
                                if (c == '.' || c == 'X' || c == '-')
                                {
                                    cellToAdd = 0;
                                }
                            }

                            if (cellToAdd.HasValue)
                            {
                                cells.Add(cellToAdd.Value);
                                if (cells.Count == 81)
                                {
                                    toReturn.Add(new Sudoku() { Cells = new List<int>(cells) });
                                    cells.Clear();
                                }
                            }
                        }
                    }
                }
            }

            return toReturn;
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


    }
}
