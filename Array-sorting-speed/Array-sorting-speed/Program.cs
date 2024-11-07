using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Array_sorting_speed
{
    internal class Program
    {
        static Random random = new Random();
        static void Main(string[] args)
        {
            int[] sizes = { 10, 1000, 10000, 1000000 }; // Можна додати більше елементів
            var sortingAlgorithms = new Dictionary<string, Action<int[]>>
            {
                //Колекція усіз методів сортування, можна збільшити
                { "Insertion Sort", InsertionSort },
                { "Bubble Sort", BubbleSort },
                { "Quick Sort", QuickSort },
                { "Merge Sort", MergeSort },
                { "Timsort", Timsort },
                { "Binary Tree Sort", BinaryTreeSort }
            };

            //Виведення даних в табличному вигляді
            Console.WriteLine("Таблиця часу сортування (мс):\n");
            Console.WriteLine("Метод сортування\t       Рандом. елем. (великий iнтервал)\t\t" +
                                  "Рандом. елем. (малий iнтервал)\t\t" +
                                  "Впоряд. за спадом (великий iнтервал)\t\t" +
                                  "Впоряд. за спадом (малий iнтервал)");
            foreach (var size in sizes)
            {
                Console.WriteLine("---------------------------------------------------------------------------------------------------" +
                                  "---------------------------------------------------------------------------------------------------");
                Console.WriteLine($"Количество елементов: {size}" +
                                  $"\t\tmm:ss:ms" +
                                  $"        \t\tmm:ss:ms" +
                                  $"            \t\t\tmm:ss:ms" +
                                  $"                \t\t\tmm:ss:ms");
                
                foreach (var algo in sortingAlgorithms)
                {
                    // Рандомний масив з великого інтервалу
                    int[] largeRangeArray = GenerateRandomArray(size, int.MinValue, int.MaxValue);
                    TimeSpan largeRangeTime = MeasureSortTime(algo.Value, largeRangeArray);

                    // Рандомний масив з малого інтервалу
                    int[] smallRangeArray = GenerateRandomArray(size, -100, 100);
                    TimeSpan smallRangeTime = MeasureSortTime(algo.Value, smallRangeArray);

                    // Впорядкований за спаданням масив з великого інтервалу
                    int[] largeRangeDescendingArray = GenerateRandomArray(size, int.MinValue, int.MaxValue);
                    Array.Sort(largeRangeDescendingArray);
                    Array.Reverse(largeRangeDescendingArray);
                    TimeSpan largeRangeDescendingTime = MeasureSortTime(algo.Value, largeRangeDescendingArray);

                    // Впорядкований за спаданням масив з малого інтервалу
                    int[] smallRangeDescendingArray = GenerateRandomArray(size, -100, 100);
                    Array.Sort(smallRangeDescendingArray);
                    Array.Reverse(smallRangeDescendingArray);
                    TimeSpan smallRangeDescendingTime = MeasureSortTime(algo.Value, smallRangeDescendingArray);

                    Console.WriteLine($"{algo.Key.PadRight(20)}\t\t\t" +
                                      $"{largeRangeTime.ToString(@"mm\:ss\:fff")}" +
                                      $"            \t\t" +
                                      $"{smallRangeTime.ToString(@"mm\:ss\:fff")}" +
                                      $"            \t\t\t" +
                                      $"{largeRangeDescendingTime.ToString(@"mm\:ss\:fff")}" +
                                      $"                    \t\t\t" +
                                      $"{smallRangeDescendingTime.ToString(@"mm\:ss\:fff")} ");
                }
                Console.WriteLine();
            }
        }

        static int[] GenerateRandomArray(int size, int minValue, int maxValue)
        {
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = random.Next(minValue, maxValue);
            }
            return array;
        }

        static TimeSpan MeasureSortTime(Action<int[]> sortAlgorithm, int[] array)
        {
            int[] arrayCopy = (int[])array.Clone();
            Stopwatch stopwatch = Stopwatch.StartNew();
            sortAlgorithm(arrayCopy);
            stopwatch.Stop();
            return stopwatch.Elapsed;
        }

        // Реалізація алгоритмів сортування

        // Insertion Sort
        static void InsertionSort(int[] array)
        {
            for (int i = 1; i < array.Length; i++)
            {
                int key = array[i];
                int j = i - 1;
                while (j >= 0 && array[j] > key)
                {
                    array[j + 1] = array[j];
                    j--;
                }
                array[j + 1] = key;
            }
        }

        // Bubble Sort
        static void BubbleSort(int[] array)
        {
            for (int i = 0; i < array.Length - 1; i++)
            {
                for (int j = 0; j < array.Length - i - 1; j++)
                {
                    if (array[j] > array[j + 1])
                    {
                        int temp = array[j];
                        array[j] = array[j + 1];
                        array[j + 1] = temp;
                    }
                }
            }
        }

        // Ітеративний Quick Sort
        static void QuickSort(int[] array)
        {
            QuickSortIterative(array, 0, array.Length - 1);
        }

        static void QuickSortIterative(int[] array, int low, int high)
        {
            Stack<(int, int)> stack = new Stack<(int, int)>();
            stack.Push((low, high));

            while (stack.Count > 0)
            {
                (low, high) = stack.Pop();
                if (low < high)
                {
                    int pivotIndex = Partition(array, low, high);

                    // Додаємо індекси частин масиву, які потрібно відсортувати, у стек
                    stack.Push((low, pivotIndex - 1));
                    stack.Push((pivotIndex + 1, high));
                }
            }
        }

        static int Partition(int[] array, int low, int high)
        {
            int pivot = array[high];
            int i = low - 1;
            for (int j = low; j < high; j++)
            {
                if (array[j] < pivot)
                {
                    i++;
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
            int temp1 = array[i + 1];
            array[i + 1] = array[high];
            array[high] = temp1;
            return i + 1;
        }

        // Merge Sort
        static void MergeSort(int[] array)
        {
            if (array.Length <= 1) return;
            int mid = array.Length / 2;
            int[] left = new int[mid];
            int[] right = new int[array.Length - mid];
            Array.Copy(array, 0, left, 0, mid);
            Array.Copy(array, mid, right, 0, array.Length - mid);
            MergeSort(left);
            MergeSort(right);
            Merge(array, left, right);
        }

        static void Merge(int[] array, int[] left, int[] right)
        {
            int i = 0, j = 0, k = 0;
            while (i < left.Length && j < right.Length)
            {
                array[k++] = (left[i] <= right[j]) ? left[i++] : right[j++];
            }
            while (i < left.Length) array[k++] = left[i++];
            while (j < right.Length) array[k++] = right[j++];
        }

        // Timsort (використовує Array.Sort(), що реалізує Timsort для масивів int у .NET)
        static void Timsort(int[] array)
        {
            Array.Sort(array);  // Array.Sort використовує Timsort для сортування
        }

        // Binary Tree Sort using SortedSet
        static void BinaryTreeSort(int[] array)
        {
            SortedSet<int> sortedSet = new SortedSet<int>(array);
            int index = 0;
            foreach (int value in sortedSet)
            {
                array[index++] = value;
            }
        }
    }
}
