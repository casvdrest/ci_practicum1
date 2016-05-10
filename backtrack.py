# runs the backtrack search algorithm

class BackTrack:

    def __init__(self, board):
        self.board = board

    def backtrack(self, stack): 
        if len(stack) == 0:
            return None
        else
            if is_goalstate(t)
                return t
            else
                t = stack[-1]
                while : # unexplored successors and not found
                    stack.append(t.next_successor())
                    backtrack(stack)
                stack.pop()

    def is_goalstate(self, state):

        # check wether the puzzle is fully completed
        for x in range(0, state.N):
            for y in range(0, state.N):
                if state.board[x, y] == 0:
                    return False

        # check wether the completed puzzle fulfills the sudoku constraints
        





                        