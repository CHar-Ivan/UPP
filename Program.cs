using System;

public class MatrixProcessor
{
    public double ProcessMatrix(int[,] matrix)
    {
        if (matrix == null)
        {
            throw new ArgumentNullException(nameof(matrix), "Матрица не может быть null.");
        }

        int rows = matrix.GetLength(0);
        int cols = matrix.GetLength(1);

        if (rows == 0 || cols == 0)
        {
            throw new ArgumentException("Матрица не может быть пустой.");
        }

        if (rows != cols)
        {
            throw new ArgumentException("Матрица должна быть квадратной.");
        }

        // 1. Вычисление среднего арифметического элементов главной диагонали
        double sum = 0;
        for (int i = 0; i < rows; i++)
        {
            sum += matrix[i, i];
        }
        double average = sum / rows;

        // 2. Модификация матрицы на основе вычисленного среднего значения
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (matrix[i, j] < average)
                {
                    matrix[i, j] = 0;
                }
            }
        }

        return average;
    }
}
class Program
{
    static void Main(string[] args)
    {
        // Оставляем пустым, так как проект запускается через тесты
    }
}