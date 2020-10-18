using System;
using Sudoku.Core;
using Sudoku = Sudoku.Core.Sudoku;

namespace Sudoku.BruteForceSolver
{
    public class BruteForceSolver:ISudokuSolver
    {
        public Core.Sudoku Solve(Core.Sudoku s)
        {
            var solution = (Core.Sudoku) s.Clone();
            CheckAndSet(solution, 0);
            return solution;
        }


        public bool NotOnRow(int k, Core.Sudoku s, int i)
        {
            for (int j = 0; j < 9; j++)
                if (s.GetCell(i, j) == k)
                {
                    return false;
                }

            return true;
        }

        public bool NotOnColumn(int k, Core.Sudoku s, int j)
        {
            for (int i = 0; i < 9; i++)
                if (s.GetCell(i, j) == k)
                    return false;
            return true;
        }

        public bool NotInBox(int k, Core.Sudoku s, int i, int j)
        {
            int _i = i - (i % 3), _j = j - (j % 3);  // ou encore : _i = 3*(i/3), _j = 3*(j/3);
            for (i = _i; i < _i + 3; i++)
                for (j = _j; j < _j + 3; j++)
                    if (s.GetCell(i, j) == k)
                        return false;
            return true;
        }




        public bool CheckAndSet(Core.Sudoku s, int position)
        {
            // Si on est à la 82e case (on sort du tableau)
            if (position == 9 * 9)
                return true;

            // On récupère les coordonnées de la case
            int i = position / 9, j = position % 9;

            // Si la case n'est pas vide, on passe à la suivante (appel récursif)
            if (s.GetCell(i, j) != 0)
                return CheckAndSet(s, position + 1);

            // énumération des valeurs possibles
            for (int k = 1; k <= 9; k++)
            {
                // Si la valeur est absente, donc autorisée
                if (NotOnRow(k, s, i) && NotOnColumn(k, s, j) && NotInBox(k, s, i, j))
                {
                    // On enregistre k dans la grid
                    s.SetCell(i, j, k) ;
                    // On appelle récursivement la fonction estValide(), pour voir si ce choix est bon par la suite
                    if (CheckAndSet(s, position + 1))
                        return true;  // Si le choix est bon, plus la peine de continuer, on renvoie true :)
                }
            }
            // Tous les chiffres ont été testés, aucun n'est bon, on réinitialise la case
            s.SetCell(i, j, 0);
            // Puis on retourne false :(
            return false;

        }





    }
}
