class Main:

    from board import Board
    from backtrack import Backtrack
    import timeit
    import time

    sudoku1 = Board(3)
    t = []
    for i in range(0, 9):
        s = input()
        t.append(s)
    sudoku1.read(t)

    t0 = time.clock()
    prog = Backtrack(sudoku1)
    result = prog.run()
    t1 = time.clock()
    if result:
        result.print()
    else:
        print("No solution found")
    print("t0 (before backtracking) = " + str(t0))
    print("t1 (after backtracking) = " + str(t1))