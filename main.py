class Main:

    from board import Board

    sudoku1 = Board(3)
    sudoku1.place(0,0,1)
    sudoku1.place(1,1,2)
    sudoku1.place(2,2,3)
    sudoku1.place(3,4,4)
    sudoku1.place(4,3,5)
    sudoku1.place(5,5,6)
    sudoku1.place(6,8,7)
    sudoku1.place(7,7,8)
    sudoku1.place(8,6,9)
    sudoku1.print()
    print(sudoku1.to_list())
