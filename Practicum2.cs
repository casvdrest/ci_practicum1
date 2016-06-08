using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace CI_practicum2
{
    class Program
    {
        // Average number of states visted per restart
        private float stateCountAvg = 0;

        // number of restarts
        private int restartCount = 0;

        // Board dimensions. N2 = N * N. Defaults to a 9x9 sudoku
        public static int N = 3;
        public static int N2 = 9;

        // Random generator
        private Random gen;

        // Holds x and y coordinates, ordered by blocks
        public Tuple<int, int>[] blockPositions;

        // Search parameters
        private readonly int RANDOM_RESTART_COUNT = 100;
        private readonly int ILS_RESTART_COUNT    = 10;
        private readonly int ILS_WALK_LENGTH      = 40;

        // Bitmasks
        public static int[] masks;

        // Possible algorithms
        private enum Algorithm
        {
            HC, // Hill climb
            RR, // Random restart local search
            ILS // Iterated local search
        }

        private Algorithm selectedAlgorithm = Algorithm.ILS;

        static void Main(string[] args)
        {
            new Program();
        }

        public Program()
        {
            // Read sudoku dimensions
            Console.Write("Please enter N:");
            N = int.Parse(Console.ReadLine());
            N2 = N * N;

            // Convert position within block to (x, y) position within the puzzle
            blockPositions = new Tuple<int, int>[N2 * N2];
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    blockPositions[((((i / N) * N) + (j / N)) * N2) + (((i % N) * N) + (j % N))] = new Tuple<int, int>(i, j);
                }
            }

            // Create bitmasks
            masks = new int[N2];
            for (int i = 0; i < N2; i++)
            {
                masks[i] = 1 << i;
            }

            gen = new Random();

            int[][] state = randomPuzzle();

            Console.WriteLine();
            Console.WriteLine("Starting position (value " + evaluate(state) + "):");
            printPuzzle(state);

            switch (selectedAlgorithm)
            {
                case Algorithm.HC: Console.Write("Started regular hill climb:"); break;
                case Algorithm.RR: Console.Write("Started random restart local search"); break;
                case Algorithm.ILS: Console.Write("Started iterated local search"); break;
            }

            Console.WriteLine(" ... Awaiting results");

            int[][] result = iteratedLocalSearch(state);

            Console.WriteLine("Found optimum (value " + evaluate(result) + "):");
            printPuzzle(result);

            Console.WriteLine();
            Console.WriteLine("Restarts: " + restartCount);
            Console.WriteLine("Average number states visited per restart: " + stateCountAvg);
            Console.ReadLine();
        }

        // Print puzzle
        public void printPuzzle(int[][] puzzle)
        {
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    if (N > 3 && puzzle[i][j] < 10)
                    {
                        Console.Write(' ');
                    }
                    Console.Write(puzzle[i][j]);
                    if (N > 3)
                    {
                        Console.Write(' ');
                    }
                    if (j % N == N - 1)
                    {
                        if (N > 3)
                        {
                            Console.Write(' ');
                        }
                        Console.Write(' ');
                    }
                }
                Console.Write('\n');
                if (i % N == N - 1)
                {
                    Console.WriteLine();
                }
            }
        }

        // Generates a random puzzle
        public int[][] randomPuzzle()
        {
            int[][] rows = new int[N2][];
            int[][] blocks = new int[N2][];
            int[] lst = new int[N2];

            for (int i = 0; i < N2; i++)
            {
                lst[i] = i + 1;
                rows[i] = new int[N2];
            }

            for (int i = 0; i < N2; i++)
            {
                lst = shuffleList(lst);
                blocks[i] = lst;

                int[] tmprow = new int[N2];
                int[] tmpcol = new int[N2];

                for (int j = 0; j < N2; j++)
                {
                    int row = ((i / N) * N) + (j / N);
                    int col = ((i % N) * N) + (j % N);
                    rows[row][col] = blocks[i][j];
                }
            }
            return rows;
        }

        // Returns a random permutation of a given list
        public int[] shuffleList(int[] list)
        {
            int n = list.Length;
            while (n > 1)
            {
                n--;
                int k = gen.Next(n + 1);
                int value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        // Perform a standard hill climb search, given an initial state
        public int[][] hillClimb(int[][] state)
        {
            int stateCount = 0;
            restartCount++;

            int[][] next = improvingNeighbour(state);

            int stateValue = evaluate(state);
            int nextValue = evaluate(next);

            while (nextValue < stateValue)
            {
                state = next;
                next = improvingNeighbour(next);

                stateValue = nextValue;
                nextValue = evaluate(next);

                stateCount++;
            }
     
            stateCountAvg = (stateCountAvg + stateCount) / (1.0f + (stateCountAvg > 0 ? 1.0f : 0.0f));

            return state;
        }

        // Find a neighbour that improves over the current state
        public int[][] improvingNeighbour(int[][] state)
        {
            int[] stateScores = new int[N2 * 2];
            for (int i = 0; i < N2; i++)
            {
                stateScores[i] = score(state[i]);
                stateScores[i + N2] = score(column(state, i));
            }

            int[][] neighbour;
            for (int i = 0; i < N2; i++)
            {
                for (int s = 0; s < N2 - 1; s++)
                {
                    for (int e = s + 1; e < N2; e++)
                    {
                        Tuple<int, int> P1 = blockPositions[i * N2 + s];
                        Tuple<int, int> P2 = blockPositions[i * N2 + e];

                        neighbour = swap(state, P1, P2);
                        int rowScores = 0;
                        int colScores = 0;

                        if (P1.Item1 != P2.Item1)
                        {
                            rowScores = ((score(neighbour[P1.Item1]) + score(neighbour[P2.Item1])) - (stateScores[P1.Item1] + stateScores[P2.Item1]));
                        }

                        if (P1.Item2 != P2.Item2)
                        {
                            colScores = ((score(column(neighbour, P1.Item2)) + score(column(neighbour, P2.Item2))) - (stateScores[P1.Item2 + N2] + stateScores[P2.Item2 + N2]));
                        }

                        if (rowScores + colScores < 0)
                        {
                            return neighbour;
                        }
                    }
                }
            }
            return state;
        }

        // Perform a random restart local search
        public int[][] randomRestartHillClimb(int[][] startState)
        {
            int[][] bestResult = hillClimb(startState);
            int bestValue = evaluate(bestResult);
            for (int i = 1; i < RANDOM_RESTART_COUNT - 1; i++)
            {
                int[][] state = randomPuzzle();
                int[][] result = hillClimb(state);
                if (evaluate(result) < bestValue)
                {
                    bestValue = evaluate(result);
                    bestResult = result;
                }
            }
            return bestResult;
        }

        // Perform iterated local search
        public int[][] iteratedLocalSearch(int[][] startState)
        {
            int p50 = ILS_RESTART_COUNT / 50;
            int bar = 0;

            int[][] state = hillClimb(startState);
            int bestValue = evaluate(state);
            for (int i = 0; bestValue > 0 && i < ILS_RESTART_COUNT - 1; i++)
            {
                int[][] next = randomWalk(state);
                next = hillClimb(next);
                if (evaluate(next) < evaluate(state))
                {
                    state = next;
                    bestValue = evaluate(next);
                }
            }
            return state;
        }

        // Performs a random walk of S random steps, given an initial state
        public int[][] randomWalk(int[][] state)
        {
            for (int i = 0; i < ILS_WALK_LENGTH; i++)
            {
                state = randomNeighbour(state);
            }
            return state;
        }

        // Returns a random neighbour
        public int[][] randomNeighbour(int[][] state)
        {
            int randomBlock = gen.Next(N2);
            int randomP1 = gen.Next(N2);
            int randomP2 = gen.Next(N2 - 1);
            if (randomP2 == randomP1)
                randomP2 = N2 - 1;
            Tuple<int, int> P1 = blockPositions[randomBlock * N2 + randomP1];
            Tuple<int, int> P2 = blockPositions[randomBlock * N2 + randomP2];

            state = swap(state, P1, P2);
            return state;
        }

        // Perform a tabu search
        public int[][] tabuSearch(int[][] state)
        {
            Stack<int[][]> tabuList = new Stack<int[][]>();
            tabuList.Push(state);
            int[][] best = state;

            List<int[][]> nextList = neighbours(state).Except(tabuList).ToList();
            int maxValue = 0;
            int[][] next = state;

            foreach (int[][] p in nextList)
                if (evaluate(p) > maxValue)
                {
                    maxValue = evaluate(p);
                    next = p;
                }
            while (true)
            {
                if (evaluate(next) <= evaluate(best))
                {
                    best = next;
                }
                state = next;
                tabuList.Push(state);
                if (tabuList.Count > 1)
                    tabuList.Pop();
                nextList = neighbours(state).Except(tabuList).ToList();
                if (nextList.Count == 0)
                    break;
                maxValue = 0;
                foreach (int[][] p in nextList)
                    if (evaluate(p) > maxValue)
                    {
                        maxValue = evaluate(p);
                        next = p;
                    }
            }
            return best;
        }

        public List<int[][]> neighbours(int[][] state)
        {
            List<int[][]> neighbours = new List<int[][]>();
            for (int i = 0; i < N2; i++)
            {
                for (int s = 0; s < N2 - 1; s++)
                {
                    for (int e = s + 1; e < N2; e++)
                    {
                        int[][] neighbour;
                        Tuple<int, int> P1 = blockPositions[i * N2 + s];
                        Tuple<int, int> P2 = blockPositions[i * N2 + e];
                        neighbour = swap(state, P1, P2);
                        neighbours.Add(neighbour);
                    }
                }
            }
            return neighbours;
        }

        // Evaluate puzzle
        public int evaluate(int[][] state)
        {
            int value = 0;
            for (int i = 0; i < N2; i++)
            {
                value += score(state[i]) + score(column(state, i));
            }
            return value;
        }

        // Assign score to row or column
        public int score(int[] list)
        {
            int dups = 0;
            int score = 0; 

            for (int i = 0; i < N2; i++)
            {
                int mask = masks[list[i] - 1];
                score += ((mask & dups) > 0) ? 1 : 0;
                dups |= mask;
            }
            return score;
        }

        // Find column from state;
        public int[] column(int[][] state, int index)
        {
            int[] col = new int[N2];

            for (int i = 0; i < N2; i++)
            {
                col[i] = state[i][index];
            }
            return col;
        }

        // Swap the numbers at positions P1 and P2 in state, returning a NEW STATE
        public int[][] swap(int[][] state, Tuple<int, int> P1, Tuple<int, int> P2)
        {
            int[][] newState = new int[N2][];
            for (int i = 0; i < N2; i++)
            {
                int[] row = new int[N2];
                state[i].CopyTo(row, 0);
                newState[i] = row;
            }

            swapTransform(ref newState, P1, P2);
            return newState; 
        }

        public void swapTransform(ref int[][] state, Tuple<int, int> P1, Tuple<int, int> P2)
        {
            int tmp = state[P1.Item1][P1.Item2];
            state[P1.Item1][P1.Item2] = state[P2.Item1][P2.Item2];
            state[P2.Item1][P2.Item2] = tmp;
        }
    }
}
