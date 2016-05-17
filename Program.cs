using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BacktrackTest
{
    class Program
    {
        // input puzzle
        int[][] board;

        // Found solution (null if no solution found)
        int[][] result = null;

        // Counts the amount of node expansions
        int expansionCount = 0;

        // Board dimensions. N2 = N * N. Defaults to a 9x9 sudoku
        int N = 3;
        int N2 = 9;

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

            // Initialize board
            board = new int[N2][];

            // Read board input
            for (int i = 0; i < N2; i++)
            {
                board[i] = new int[N2];

                // Assume different input formats for N = 3 and N > 3
                if (N == 3)
                {
                    char[] line = Console.ReadLine().ToCharArray();
                    for (int j = 0; j < N2; j++) board[i][j] = line[j] - 48;
                }
                else
                {
                    string[] line = Console.ReadLine().Split();
                    for (int j = 0; j < N2; j++) board[i][j] = int.Parse(line[j]);
                }
            }

            // Initialize stack
            Stack<int[][]> s = new Stack<int[][]>();

            // Push root to stack
            s.Push(board);

            // Call backtrack procedure
            Console.WriteLine("\nStarted backtrack search, awaiting results ...\n");
            backtrack(s);

            // Check whether a solution has been found
            if (result != null)
            {
                // Print solution
                for (int i = 0; i < N2; i++)
                {
                    for (int j = 0; j < N2; j++)
                    {
                        Console.Write(result[i][j]);
                        if (j % N == N - 1) Console.Write(' ');
                    }
                    Console.Write('\n');
                    if (i % N == N - 1) Console.WriteLine();
                }
                Console.WriteLine(expansionCount + " nodes expanded");
            }
            else
            {
                Console.WriteLine("No Solution found");
            }
            Console.ReadLine();
        }

        // Backtrack procedure
        public void backtrack(Stack<int[][]> stack)
        {
            // Increment expansion counter
            expansionCount++;

            // Exit if stack is empty
            if (stack.Count == 0)
            {
                return;
            }

            else {
                // Pop next node from stack
                int[][] t = stack.Pop();

                // Suspend search if a solution is found
                if (isGoalState(t))
                {
                    result = t;
                    return;
                }
                else {
                    // Retrieve successors
                    List<int[][]> successors = getSuccessors(t);

                    // push successors on stack and continue searching
                    foreach (int[][] s in successors)
                    {
                        stack.Push(s);
                        backtrack(stack);
                        if (result != null)
                        {
                            return;
                        }
                    }
                }
            }
        }

        // Gets the successor states of a given state
        public List<int[][]> getSuccessors(int[][] state)
        {
            List<int[][]> successors = new List<int[][]>();
            
            // Find first empty slot
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    if (state[i][j] == 0)
                    {
                        // Generate successors
                        for (int n = 1; n < N2 + 1; n++)
                        {
                            // Discard successors if they're known to not be feasible
                            if (contains(state[i], n) || contains(column(j, state), n) || 
                                contains(block((int)(Math.Floor(i / (float)N) + Math.Floor(j / (float)N) * N), state), n))
                            {
                                continue;
                            }

                            // create successor state 
                            int[][] sstate = new int[N2][];
                            for (int x = 0; x < N2; x++)
                            {
                                sstate[x] = new int[N2];
                                for (int y = 0; y < N2; y++)
                                {
                                    sstate[x][y] = state[x][y];
                                }
                            }
                            sstate[i][j] = n;
                            successors.Add(sstate);
                        }
                        return successors;
                    }
                }
            }

            // Return empty list if no successors
            return successors;
        }

        // Determines whether a given state is a goal state
        public bool isGoalState(int[][] state)
        {
            // Check whether the state is completely filled out
            for (int i = 0; i < N2; i++)
            {
                for (int j = 0; j < N2; j++)
                {
                    if (state[i][j] == 0)
                        return false;
                }
            }

            // Check whether all rows, columns and blocks sum to 45
            for (int i = 0; i < 9; i++)
            {
                if (listSum(state[i]) != 45 || listSum(column(i, state)) != 45 || listSum(block(i, state)) != 45)
                    return false;
            }

            // TODO add final constraint
            return true;
        }

        // Find the sum of a list
        public int listSum(int[] list)
        {
            int sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += list[i];
            }
            return sum;
        }

        // Retrieve block at index from state
        public int[] block(int index, int[][] state)
        {
            int[] result = new int[9];
            int x = index % 3;
            int y = index / 3;
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    result[j + i * 3] = state[i + x * 3][j + y * 3];
                }
            }

            return result;
        }

        // Retrieve column at index from state
        public int[] column(int index, int[][] state)
        {
            int[] res = new int[9];
            for (int i = 0; i < 9; i++)
            {
                res[i] = state[i][index];
            }
            return res;
        }

        // Check whether a list contains integer n
        public bool contains(int[] lst, int n)
        {
            for (int i = 0; i < lst.Length; i++)
            {
                if (lst[i] == n) return true;
            }
            return false;
        }
    }
}
