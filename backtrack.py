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
        for x in range(0, state.N2):
            for y in range(0, state.N2):
                if state.board[x, y] == 0:
                    return False

        # check for sum in rows, columns and blocks to discard faulty solutions
        for i in range(0, state.N):
            if sum(state.row(i)) != 45 or sum(state.column(i)) != 45 or sum(state.block(i)) != 45:
                return False

        # check wether every number occurs exactly once in every row, column and block




                        