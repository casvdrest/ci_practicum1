using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CI_practicum2
{
    class Program
    {
        // Found solution (null if no solution found)
        int[][] result = null;

        // Board dimensions. N2 = N * N. Defaults to a 9x9 sudoku
        public static int N = 3;
        public static int N2 = 9;

        // Holds x and y coordinates, ordered by blocks
        public static Tuple<int, int>[] blockPositions;

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

            blockPositions = new Tuple<int, int>[N2 * N2];

            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    blockPositions[(((i / N) * N) + (j / N)) + (((i % N) * N) + (j % N))] = new Tuple<int, int>(i, j);
                }
            }

            // TODO generate random starting position
            Puzzle startState = randomPuzzle();

            Puzzle result = hillClimb(startState);
            
            // Print solution
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    Console.Write(result.Rows[i][j]);
                    if (j % N == N - 1) Console.Write(' ');
                }
                Console.Write('\n');
                if (i % N == N - 1) Console.WriteLine();
            }
            Console.ReadLine();
        }

        // Generates a random puzzle
        public Puzzle randomPuzzle()
        {
            return null;
        }

        // Perform a standard hill climb search, given an initial state
        public Puzzle hillClimb(Puzzle state)
        {
            Puzzle next = improvingNeighbour(state);
            while (next.Value != state.Value)
            {
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
                        neighbour.updateScores(P1, P2);
                        if (neighbour.Value > state.Value)
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
                if ((mask & dups) > 0)
                {
                    score++;
                }
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
        public Puzzle(int[][] rows, int[][] columns, int[][] blocks, int[] scores)
        {
            this.Rows = rows;
            this.Columns = columns;
            this.Blocks = blocks;

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
        }

        public void updateScores(Tuple<int, int> P1, Tuple<int, int> P2)
        {
            this.Value -= this.Scores[P1.Item1] + this.Scores[P2.Item1] + this.Scores[P1.Item2 + Program.N2] + this.Scores[P2.Item2 + Program.N2];

            // update rows
            this.Scores[P1.Item1] = Program.score(this.Rows[P1.Item1]);
            this.Scores[P2.Item1] = Program.score(this.Rows[P2.Item1]);

            // update columns
            this.Scores[P1.Item2 + Program.N2] = Program.score(this.Columns[P1.Item2]);
            this.Scores[P2.Item2 + Program.N2] = Program.score(this.Columns[P2.Item2]);

            this.Value += this.Scores[P1.Item1] + this.Scores[P2.Item1] + this.Scores[P1.Item2 + Program.N2] + this.Scores[P2.Item2 + Program.N2];
        }
    }
}
