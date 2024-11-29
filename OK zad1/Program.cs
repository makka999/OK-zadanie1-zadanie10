using System;
using System.Numerics;

//  Rozwiazanie
// Maksymalna wartość funkcji z: 500915 / 218
// Wartości zmiennych:
// x1 = 0 / 0
// x2 = 209 / 218
// x3 = 0 / 0
// x4 = 207 / 218
// x5 = 721 / 109

public struct Rational
{
    public BigInteger Numerator { get; }
    public BigInteger Denominator { get; }

    public Rational(BigInteger numerator, BigInteger denominator)
    {
        if (denominator == 0)
        {
            if (numerator == 0)
            {
                // Traktuj 0/0 jako specjalny przypadek
                Numerator = 0;
                Denominator = 1; // Traktujemy jako "zero" w logice algorytmu
                return;
            }
            else
            {
                throw new DivideByZeroException($"Denominator cannot be zero. Numerator: {numerator}");
            }
        }

        if (denominator < 0)
        {
            numerator = -numerator;
            denominator = -denominator;
        }

        BigInteger gcd = BigInteger.GreatestCommonDivisor(numerator, denominator);
        Numerator = numerator / gcd;
        Denominator = denominator / gcd;
    }

    // Przeciążenie operatorów matematycznych
    public static Rational operator +(Rational a, Rational b) =>
        new Rational(a.Numerator * b.Denominator + b.Numerator * a.Denominator, a.Denominator * b.Denominator);

    public static Rational operator -(Rational a, Rational b) =>
        new Rational(a.Numerator * b.Denominator - b.Numerator * a.Denominator, a.Denominator * b.Denominator);

    public static Rational operator *(Rational a, Rational b) =>
        new Rational(a.Numerator * b.Numerator, a.Denominator * b.Denominator);

    public static Rational operator /(Rational a, Rational b)
    {
        if (b.Numerator == 0)
            throw new DivideByZeroException("Cannot divide by zero.");

        return new Rational(a.Numerator * b.Denominator, a.Denominator * b.Numerator);
    }

    // Przeciążenie operatora negacji
    public static Rational operator -(Rational a) =>
        new Rational(-a.Numerator, a.Denominator);

    // Porównania
    public static bool operator >(Rational a, Rational b) =>
        a.Numerator * b.Denominator > b.Numerator * a.Denominator;

    public static bool operator <(Rational a, Rational b) =>
        a.Numerator * b.Denominator < b.Numerator * a.Denominator;

    public static bool operator >=(Rational a, Rational b) =>
        a.Numerator * b.Denominator >= b.Numerator * a.Denominator;

    public static bool operator <=(Rational a, Rational b) =>
        a.Numerator * b.Denominator <= b.Numerator * a.Denominator;

    public static bool operator ==(Rational a, Rational b) =>
        a.Numerator * b.Denominator == b.Numerator * a.Denominator;

    public static bool operator !=(Rational a, Rational b) =>
        a.Numerator * b.Denominator != b.Numerator * a.Denominator;

    public override string ToString() => Denominator == 1 ? $"{Numerator}" : $"{Numerator}/{Denominator}";

    public override bool Equals(object? obj) =>
        obj is Rational other && this == other;

    public override int GetHashCode() => HashCode.Combine(Numerator, Denominator);
}

class Simplex
{
    static void Main(string[] args)
    {
        // Funkcja celu
        Rational[] c = {
            new Rational(141, 1),
            new Rational(393, 1),
            new Rational(273, 1),
            new Rational(804, 1),
            new Rational(175, 1)
        };

        // Macierz ograniczeń
        Rational[,] A = {
            { new Rational(3, 1), new Rational(5, 1), new Rational(2, 1), new Rational(5, 1), new Rational(4, 1) },
            { new Rational(7, 1), new Rational(12, 1), new Rational(11, 1), new Rational(10, 1), new Rational(0, 1) },
            { new Rational(0, 1), new Rational(-3, 1), new Rational(12, 1), new Rational(7, 1), new Rational(2, 1) }
        };

        // Wektor prawej strony ograniczeń
        Rational[] b = {
            new Rational(36, 1),
            new Rational(21, 1),
            new Rational(17, 1)
        };

        // Zakres zmiennych decyzyjnych
        Rational[] lowerBound = {
            new Rational(0, 1),
            new Rational(0, 1),
            new Rational(0, 1),
            new Rational(0, 1),
            new Rational(0, 1)
        };
        Rational[] upperBound = {
            new Rational(20, 1),
            new Rational(20, 1),
            new Rational(20, 1),
            new Rational(20, 1),
            new Rational(20, 1)
        };

        // Rozwiązanie problemu
        var result = SolveSimplex(c, A, b, lowerBound, upperBound);

        // Wyświetlanie wyniku
        Console.WriteLine("Maksymalna wartość funkcji z: " + result.Item1);
        Console.WriteLine("Wartości zmiennych:");
        for (int i = 0; i < result.Item2.Length; i++)
        {
            Console.WriteLine($"x{i + 1} = {result.Item2[i]}");
        }
    }

    public static Tuple<Rational, Rational[]> SolveSimplex(
        Rational[] c, Rational[,] A, Rational[] b, Rational[] lowerBound, Rational[] upperBound)
    {
        int numVariables = c.Length;
        int numOriginalConstraints = b.Length;
        int numBounds = upperBound.Length;
        int numConstraints = numOriginalConstraints + numBounds;

        // Tworzenie rozszerzonej macierzy ograniczeń A i wektora b
        Rational[,] A_full = new Rational[numConstraints, numVariables];
        Rational[] b_full = new Rational[numConstraints];

        // Kopiowanie oryginalnych ograniczeń
        for (int i = 0; i < numOriginalConstraints; i++)
        {
            for (int j = 0; j < numVariables; j++)
                A_full[i, j] = A[i, j];
            b_full[i] = b[i];
        }

        // Dodawanie ograniczeń górnych dla zmiennych decyzyjnych
        for (int i = 0; i < numBounds; i++)
        {
            for (int j = 0; j < numVariables; j++)
                A_full[numOriginalConstraints + i, j] = (i == j) ? new Rational(1, 1) : new Rational(0, 1);
            b_full[numOriginalConstraints + i] = upperBound[i];
        }

        int numTotalVariables = numVariables + numConstraints;

        // Tworzenie tablicy Simplex
        Rational[,] tableau = new Rational[numConstraints + 1, numTotalVariables + 1];

        // Lista zmiennych bazowych
        int[] basicVariables = new int[numConstraints];

        // Wypełnianie macierzy ograniczeń i wektora prawej strony
        for (int i = 0; i < numConstraints; i++)
        {
            for (int j = 0; j < numVariables; j++)
                tableau[i, j] = A_full[i, j];
            // Dodawanie zmiennych nadmiarowych
            tableau[i, numVariables + i] = new Rational(1, 1);
            // Wektor prawej strony
            tableau[i, numTotalVariables] = b_full[i];
            // Inicjalizacja zmiennych bazowych
            basicVariables[i] = numVariables + i;
        }

        // Wypełnianie wiersza funkcji celu
        for (int j = 0; j < numVariables; j++)
            tableau[numConstraints, j] = -c[j];
        for (int j = numVariables; j < numTotalVariables; j++)
            tableau[numConstraints, j] = new Rational(0, 1);
        // Wektor prawej strony funkcji celu
        tableau[numConstraints, numTotalVariables] = new Rational(0, 1);

        // Algorytm Simplex
        while (true)
        {
            // Znajdowanie kolumny wchodzącej (najbardziej ujemny współczynnik w wierszu funkcji celu)
            int pivotColumn = -1;
            Rational mostNegative = new Rational(0, 1);
            for (int j = 0; j < numTotalVariables; j++)
            {
                if (tableau[numConstraints, j] < mostNegative)
                {
                    mostNegative = tableau[numConstraints, j];
                    pivotColumn = j;
                }
            }

            if (pivotColumn == -1)
                break; // Optymalne rozwiązanie znalezione

            // Sprawdzenie nieograniczoności
            bool unbounded = true;
            for (int i = 0; i < numConstraints; i++)
            {
                if (tableau[i, pivotColumn] > new Rational(0, 1))
                {
                    unbounded = false;
                    break;
                }
            }

            if (unbounded)
                throw new InvalidOperationException("Problem jest nieograniczony.");

            // Znajdowanie wiersza wychodzącego za pomocą testu ilorazu minimalnego
            int pivotRow = -1;
            Rational minRatio = new Rational(long.MaxValue, 1);
            for (int i = 0; i < numConstraints; i++)
            {
                if (tableau[i, pivotColumn] > new Rational(0, 1))
                {
                    Rational ratio = tableau[i, numTotalVariables] / tableau[i, pivotColumn];
                    if (ratio < minRatio)
                    {
                        minRatio = ratio;
                        pivotRow = i;
                    }
                    else if (ratio == minRatio)
                    {
                        // Zastosowanie reguły Blanda w przypadku degeneracji
                        if (basicVariables[i] < basicVariables[pivotRow])
                        {
                            pivotRow = i;
                        }
                    }
                }
            }

            if (pivotRow == -1)
                throw new InvalidOperationException("Problem jest nieograniczony.");

            // Aktualizacja zmiennych bazowych
            basicVariables[pivotRow] = pivotColumn;

            // Aktualizacja tablicy Simplex
            Rational pivotValue = tableau[pivotRow, pivotColumn];

            // Dzielimy wiersz przestawny przez element przestawny
            for (int j = 0; j <= numTotalVariables; j++)
                tableau[pivotRow, j] /= pivotValue;

            // Zerujemy pozostałe elementy w kolumnie przestawnej
            for (int i = 0; i <= numConstraints; i++)
            {
                if (i != pivotRow)
                {
                    Rational factor = tableau[i, pivotColumn];
                    for (int j = 0; j <= numTotalVariables; j++)
                        tableau[i, j] -= factor * tableau[pivotRow, j];
                }
            }
        }

        // Odczytanie wyników
        Rational[] solution = new Rational[numVariables];
        for (int i = 0; i < numConstraints; i++)
        {
            int basicVar = basicVariables[i];
            if (basicVar < numVariables)
            {
                // Zmienna decyzyjna
                solution[basicVar] = tableau[i, numTotalVariables];
            }
            // Inaczej jest to zmienna nadmiarowa, pomijamy
        }

        Rational maxValue = tableau[numConstraints, numTotalVariables];
        return Tuple.Create(maxValue, solution);
    }
}
