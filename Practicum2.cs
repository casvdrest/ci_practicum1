using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_practicum2
{
    class Program
    {
        // number of states visited
        int stateCount = 0; 

        // Board dimensions. N2 = N * N. Defaults to a 9x9 sudoku
        public static int N = 3;
        public static int N2 = 9;

        private Random gen;

        // Holds x and y coordinates, ordered by blocks
        public Tuple<int, int>[] blockPositions;

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

            gen = new Random();

            blockPositions = new Tuple<int, int>[N2 * N2];

            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    blockPositions[((((i / N) * N) + (j / N)) * N2) + (((i % N) * N) + (j % N))] = new Tuple<int, int>(i, j);
                }
            }

            Puzzle startState = randomPuzzle();

            Console.WriteLine();
            Console.WriteLine("Starting position: (value " + startState.Value + "):");
            printPuzzle(startState);
            Console.WriteLine();

            Puzzle result = iteratedLocalSearch(startState);

            Console.WriteLine("Found optimum (value " + result.Value + "):");
            printPuzzle(result);

            Console.ReadLine();
        }

        public void printPuzzle(Puzzle puzzle)
        {
            // Print solution
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    Console.Write(puzzle.Rows[i][j]);
                    if (j % N == N - 1) Console.Write(' ');
                }
                Console.Write('\n');
                if (i % N == N - 1) Console.WriteLine();
            }
        }

        // Generates a random puzzle
        public Puzzle randomPuzzle()
        {
            int[][] blocks = new int[N2][];
            int[][] rows = new int[N2][];
            int[][] columns = new int[N2][];
            int[] lst = new int[N2];

            for (int i = 0; i < N2; i++)
            {
                lst[i] = i + 1;
                rows[i] = new int[N2];
                columns[i] = new int[N2];
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
                    columns[col][row] = blocks[i][j];
                }
            }

            Puzzle state = new Puzzle(rows, columns, blocks);
            state.setScores(evaluate(state));
            return state;
        }

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

        public Puzzle randomRestartHillClimb(Puzzle startState)
        {
            Puzzle bestResult = hillClimb(startState);
            int bestValue = bestResult.Value;
            for (int i = 1; i < 10000; i++)
            {
                Puzzle state = randomPuzzle();
                Puzzle result = hillClimb(state);
                if (result.Value < bestValue)
                {
                    bestValue = result.Value;
                    bestResult = result;
                }
            }
            return bestResult;
        }

        public Puzzle iteratedLocalSearch(Puzzle startState)
        {
            Puzzle state = hillClimb(startState);
            int bestValue = state.Value;
            for (int i = 0; i < 10000; i++)
            {
                Puzzle next = randomWalk(state);
                next = hillClimb(next);
                if (next.Value < state.Value)
                {
                    state = next;
                    bestValue = next.Value;
                }
            }
            return state;
        }

        //Performs a random walk of S random steps, given an initial state
        public Puzzle randomWalk(Puzzle state)
        {
            for (int i = 0; i < 100; i++)
            {
                state = randomNeighbour(state);
            }
            return state;
        }

        public Puzzle randomNeighbour(Puzzle state)
        {
            int randomBlock = gen.Next(N2);
            int randomP1 = gen.Next(N2);
            int randomP2 = gen.Next(N2 - 1);
            if (randomP2 == randomP1)
                randomP2 = N2 - 1;
            Tuple<int, int> P1 = blockPositions[randomBlock * N2 + randomP1];
            Tuple<int, int> P2 = blockPositions[randomBlock * N2 + randomP2];
            state.Swap(P1, P2);
            state.updateScores(P1, P2);
            return state;
        }

        // Perform a standard hill climb search, given an initial state
        public Puzzle hillClimb(Puzzle state)
        {
            Puzzle next = improvingNeighbour(state);
            while (next.Value != state.Value)
            {
                stateCount++;
                state = next;
                next = improvingNeighbour(state);
            }
            return state;
        }

        public Puzzle improvingNeighbour(Puzzle state)
        {
            // Loop through blocks
            for (int i = 0; i < N2; i++)
            {
                for (int s = 0; s < N2 - 1; s++)
                {
                    for (int e = s + 1; e < N2; e++)
                    {
                        Puzzle neighbour = state;
                        Tuple<int, int> P1 = blockPositions[i * N2 + s];
                        Tuple<int, int> P2 = blockPositions[i * N2 + e];
                        neighbour.Swap(P1, P2);
                        if (neighbour.Value < state.Value)
                        {
                            return neighbour;
                        }
                    }
                }
            }
            return state;
        }

        // evaluate a state
        public int[] evaluate(Puzzle state)
        {
            int[] eval = new int[N2 * 2];
            for (int i = 0; i < N2; i++)
            {
                eval[i] = score(state.Rows[i]); 
                eval[i + N2] = score(state.Columns[i]);
            }
            return eval;
        }
        
        // assign score to row or column
        public static int score(int[] list)
        {
            int dups = 0;
            int score = 0; 

            for (int i = 0; i < N2; i++)
            {
                int mask = 1 << (list[i] - 1);
                score += ((mask & dups) > 0) ? 1 : 0;
                dups |= mask;
            }
            return score;
        }
    }

    // Struct to hold a sudoku puzzle
    struct Puzzle
    {
        public int[][] Rows;
        public int[][] Columns;
        public int[][] Blocks;

        public int[] Scores;
        public int Value;

        // Create new puzzle
        public Puzzle(int[][] rows, int[][] columns, int[][] blocks)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Blocks = blocks;

            this.Scores = new int[0];
            this.Value = 0;
        }

        public void setScores(int[] scores)
        {
            this.Scores = scores;

            this.Value = 0;
            for (int i = 0; i < scores.Length; i++)
            {
                this.Value += scores[i];
            }
        }

        // Swap operator
        public void Swap(Tuple<int, int> P1, Tuple<int, int> P2)
        {
            // retrieve values
            int v1 = Rows[P1.Item1][P1.Item2];
            int v2 = Rows[P2.Item1][P2.Item2];

            // update rows
            Rows[P1.Item1][P1.Item2] = v2;
            Rows[P2.Item1][P2.Item2] = v1;

            // update columns
            Columns[P1.Item2][P1.Item1] = v2;
            Columns[P2.Item2][P2.Item1] = v1;

            // update blocks
            Blocks[(P1.Item1 / Program.N * Program.N) + (P1.Item2 / Program.N)][((P1.Item1 % Program.N) * Program.N) + (P1.Item2 % Program.N)] = v2;
            Blocks[(P2.Item1 / Program.N * Program.N) + (P2.Item2 / Program.N)][((P2.Item1 % Program.N) * Program.N) + (P2.Item2 % Program.N)] = v1;

            updateScores(P1, P2);
        }

        public void updateScores(Tuple<int, int> P1, Tuple<int, int> P2)
        {
            // update rows
            this.Scores[P1.Item1] = Program.score(this.Rows[P1.Item1]);
            this.Scores[P2.Item1] = Program.score(this.Rows[P2.Item1]);

            // update columns
            this.Scores[P1.Item2 + Program.N2] = Program.score(this.Columns[P1.Item2]);
            this.Scores[P2.Item2 + Program.N2] = Program.score(this.Columns[P2.Item2]);

            Value = 0;
            for (int i = 0; i < Program.N2 * 2; i++)
            {
                Value += Scores[i];
            }
        }
    }
}
