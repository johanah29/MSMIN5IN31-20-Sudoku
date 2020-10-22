﻿using Sudoku.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Z3;
using System.Security.Cryptography;
using System.Globalization;
using System.Diagnostics;

namespace sudoku.Z3
{

    public class Z3Solver : ISudokuSolver
    {
        Context _ctx = new Context();
        
        // 9x9 matrix of integer variables
        IntExpr[][] X = new IntExpr[9][];
        private BoolExpr sudoku_c;
        public Z3Solver()
        {
            PrepareGenericConstraints();
        }

        private void PrepareGenericConstraints()
        {
           
            for (uint i = 0; i < 9; i++)
            {
                X[i] = new IntExpr[9];
                for (uint j = 0; j < 9; j++)
                    X[i][j] = (IntExpr)_ctx.MkConst(_ctx.MkSymbol("x_" + (i + 1) + "_" + (j + 1)), _ctx.IntSort);
            }

            // each cell contains a value in {1, ..., 9}
            Expr[][] cells_c = new Expr[9][];
            for (uint i = 0; i < 9; i++)
            {
                cells_c[i] = new BoolExpr[9];
                for (uint j = 0; j < 9; j++)
                    cells_c[i][j] = _ctx.MkAnd(_ctx.MkLe(_ctx.MkInt(1), X[i][j]),
                        _ctx.MkLe(X[i][j], _ctx.MkInt(9)));
            }

            // each row contains a digit at most once
            BoolExpr[] rows_c = new BoolExpr[9];
            for (uint i = 0; i < 9; i++)
                rows_c[i] = _ctx.MkDistinct(X[i]);


            // each column contains a digit at most once
            BoolExpr[] cols_c = new BoolExpr[9];
            for (uint j = 0; j < 9; j++)
            {
                IntExpr[] column = new IntExpr[9];
                for (uint i = 0; i < 9; i++)
                    column[i] = X[i][j];

                cols_c[j] = _ctx.MkDistinct(column);
            }

            // each 3x3 square contains a digit at most once
            BoolExpr[][] sq_c = new BoolExpr[3][];
            for (uint i0 = 0; i0 < 3; i0++)
            {
                sq_c[i0] = new BoolExpr[3];
                for (uint j0 = 0; j0 < 3; j0++)
                {
                    IntExpr[] square = new IntExpr[9];
                    for (uint i = 0; i < 3; i++)
                        for (uint j = 0; j < 3; j++)
                            square[3 * i + j] = X[3 * i0 + i][3 * j0 + j];
                    sq_c[i0][j0] = _ctx.MkDistinct(square);
                }
            }

            sudoku_c = _ctx.MkTrue();
            foreach (BoolExpr[] t in cells_c)
                sudoku_c = _ctx.MkAnd(_ctx.MkAnd(t), sudoku_c);
            sudoku_c = _ctx.MkAnd(_ctx.MkAnd(rows_c), sudoku_c);
            sudoku_c = _ctx.MkAnd(_ctx.MkAnd(cols_c), sudoku_c);
            foreach (BoolExpr[] t in sq_c)
                sudoku_c = _ctx.MkAnd(_ctx.MkAnd(t), sudoku_c);
        }

        Sudoku.Core.Sudoku SudokuExample(Sudoku.Core.Sudoku instance)
        {
            BoolExpr instance_c = _ctx.MkTrue();
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    instance_c = _ctx.MkAnd(instance_c,
                        (BoolExpr)
                        _ctx.MkITE(_ctx.MkEq(_ctx.MkInt(instance.GetCell(i,j)), _ctx.MkInt(0)),
                            _ctx.MkTrue(),
                            _ctx.MkEq(X[i][j], _ctx.MkInt(instance.GetCell(i, j)))));

            Solver s = _ctx.MkSolver();
            s.Assert(sudoku_c);
            s.Assert(instance_c);

            if (s.Check() == Status.SATISFIABLE)
            {
                Model m = s.Model;
                for (int i = 0; i < 9; i++)
                    for (int j = 0; j < 9; j++){                
                        instance.SetCell(i, j, ((IntNum)m.Evaluate(X[i][j])).Int);
                    }
                return instance;


            }
            else
            {
                Console.WriteLine("Failed to solve sudoku");
                return instance;
            }
        }
     

        public Sudoku.Core.Sudoku Solve(Sudoku.Core.Sudoku s)
        {
         //   Z3Solver z3 = new Z3Solver();
            return SudokuExample(s);
        }
    }
}