# runs the backtrack search algorithm

class Backtrack:

    from board import Board

    def __init__(self, board):
        self.board = board

    def run(self):
        return self.backtrack([self.board])

    def backtrack(self, stack): 
        if len(stack) == 0:
            return None
        else:
            t = stack[-1]
            if self.is_goalstate(t):
                return t
            else:
                succ = t.succ()
                while succ: 
                    stack.append(succ)
                    r = self.backtrack(stack)
                    if r:
                        return r
                    succ = t.succ()
                stack.pop()

    def is_goalstate(self, state):

        # check wether the puzzle is fully completed
        for x in range(0, state.N2):
            for y in range(0, state.N2):
                if state.board[x, y] == 0:
                    return False

        # check for sum in rows, columns and blocks to discard faulty solutions
        for i in range(0, state.N2):
            if sum(state.row(i)) != 45 or sum(state.column(i)) != 45 or sum(state.block(i)) != 45:
                return False

        # check wether every number occurs exactly once in every row, column and block
        for i in range(0, state.N2):
            if len(set(state.row[i])) != 9 or len(set(state.column[i])) != 9 or len(set(state.block[i])) != 9:
                return False

        return True




                        