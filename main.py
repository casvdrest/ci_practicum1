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

    #HAAL DIT WEG ALS JE WIL BACKTRACKEN
    a = sudoku1
    while a:
        s0 = time.clock()
        a = sudoku1.succ()
        s1 = time.clock()
        print(s1 - s0)

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