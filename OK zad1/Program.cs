using System;
using Google.OrTools.Sat;

class SumyMatrix
{
    static void Main()
    {
        // zadanie 10

        CpModel model = new CpModel();

        int rows = 3;
        int cols = 4;
        int NumMin = 1;
        int NumMax = 12;

        IntVar[,] x = new IntVar[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                x[i, j] = model.NewIntVar(NumMin, NumMax, $"x[{i},{j}]");
            }
        }

        model.AddAllDifferent(x.Flatten());

        model.Add(x[0, 0] + x[0, 1] + x[0, 2] + x[0, 3] == 30);
        model.Add(x[1, 0] + x[1, 1] + x[1, 2] + x[1, 3] == 18);
        model.Add(x[2, 0] + x[2, 1] + x[2, 2] + x[2, 3] == 30);

        model.Add(x[0, 0] + x[1, 0] + x[2, 0] == 27);
        model.Add(x[0, 1] + x[1, 1] + x[2, 1] == 16);
        model.Add(x[0, 2] + x[1, 2] + x[2, 2] == 10);
        model.Add(x[0, 3] + x[1, 3] + x[2, 3] == 25);

        model.Add(x[0, 1] == 6); 
        model.Add(x[1, 0] == 8); 
        model.Add(x[2, 1] == 3); 

        CpSolver solver = new CpSolver();

        CpSolverStatus status = solver.Solve(model);

        if (status == CpSolverStatus.Optimal || status == CpSolverStatus.Feasible)
        {
            Console.WriteLine("Rozwiązanie");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"{solver.Value(x[i, j])} ");
                }
                Console.WriteLine();
            }
        }
        else
        {
            Console.WriteLine("Znów nie działa");
        }
    }
}

public static class Extensions
{
    public static T[] Flatten<T>(this T[,] array)
    {
        int rows = array.GetLength(0);
        int cols = array.GetLength(1);
        T[] flat = new T[rows * cols];
        int k = 0;
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                flat[k++] = array[i, j];
            }
        }
        return flat;
    }
}
