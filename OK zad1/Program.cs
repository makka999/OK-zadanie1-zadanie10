using System;
using Google.OrTools.LinearSolver;

class Program
{
    static void Main()
    {
        // Mateusz Kłaptocz 
        // Ok - Optymalizacja kombinatoryczna
        // z pliku zad1.pdf
        // zadanie 5
        // Wyznacz dokładne rozwiązanie poniższego problemu liniowego.
        // max 𝑧 = 141𝑥1 + 393𝑥2 + 273𝑥3 + 804𝑥4 + 175𝑥5
        // 3𝑥1 + 5𝑥2 + 2𝑥3 + 5𝑥4 + 4𝑥5 ≤ 36
        // 7𝑥1 + 12𝑥2 + 11𝑥3 + 10𝑥4 ≤ 21
        // − 3𝑥2 + 12𝑥3 + 7𝑥4 + 2𝑥5 ≤ 17
        // 0 ≤ 𝑥1, 𝑥2, 𝑥3, 𝑥4, 𝑥5 ≤ 20



        Solver solver = Solver.CreateSolver("SCIP");

        // zmienne
        Variable x1 = solver.MakeNumVar(0.0, 20.0, "x1"); 
        Variable x2 = solver.MakeNumVar(0.0, 20.0, "x2"); 
        Variable x3 = solver.MakeNumVar(0.0, 20.0, "x3"); 
        Variable x4 = solver.MakeNumVar(0.0, 20.0, "x4"); 
        Variable x5 = solver.MakeNumVar(0.0, 20.0, "x5"); 

        // z = 141x1 + 393x2 + 273x3 + 804x4 + 175x5
        Objective objective = solver.Objective();
        objective.SetCoefficient(x1, 141);
        objective.SetCoefficient(x2, 393);
        objective.SetCoefficient(x3, 273);
        objective.SetCoefficient(x4, 804);
        objective.SetCoefficient(x5, 175);
        objective.SetMaximization(); //max z

        // ograniczenia
        // 3x1 + 5x2 + 2x3 + 5x4 + 4x5 <= 36
        Constraint c1 = solver.MakeConstraint(double.NegativeInfinity, 36);
        c1.SetCoefficient(x1, 3);
        c1.SetCoefficient(x2, 5);
        c1.SetCoefficient(x3, 2);
        c1.SetCoefficient(x4, 5);
        c1.SetCoefficient(x5, 4);

        // 7x1 + 12x2 + 11x3 + 10x4 <= 21
        Constraint c2 = solver.MakeConstraint(double.NegativeInfinity, 21);
        c2.SetCoefficient(x1, 7);
        c2.SetCoefficient(x2, 12);
        c2.SetCoefficient(x3, 11);
        c2.SetCoefficient(x4, 10);

        // -3x2 + 12x3 + 7x4 + 2x5 <= 17
        Constraint c3 = solver.MakeConstraint(double.NegativeInfinity, 17);
        c3.SetCoefficient(x2, -3);
        c3.SetCoefficient(x3, 12);
        c3.SetCoefficient(x4, 7);
        c3.SetCoefficient(x5, 2);

        // start solvera
        Solver.ResultStatus resultStatus = solver.Solve();

        // wyswielt wynik
        if (resultStatus == Solver.ResultStatus.OPTIMAL)
        {
            Console.WriteLine("Rozwiązanie:");
            Console.WriteLine("x1 = " + x1.SolutionValue());
            Console.WriteLine("x2 = " + x2.SolutionValue());
            Console.WriteLine("x3 = " + x3.SolutionValue());
            Console.WriteLine("x4 = " + x4.SolutionValue());
            Console.WriteLine("x5 = " + x5.SolutionValue());
            Console.WriteLine("Maksymalna wartość funkcji z = " + objective.Value());
        }
        else
        {
            Console.WriteLine("Problem nie ma rozwiązania.");
        }
    }
}
