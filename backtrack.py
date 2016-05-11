# runs the backtrack search algorithm

class Backtrack:

    from board import Board

    def __init__(self, board):
        self.board = board
        self.found = []
        self.i = 0

    def run(self):
        return self.backtrack([self.board])

    def backtrack(self, stack):
        """
        o = 'backtrack called with stack: \n'
        for i in range(0, len(stack)):
            oo = str(i)
            for j in range(0,9):
                oo += str(stack[i].row(j))
            o += oo + '\n'
        print(o)
        """
        self.i += 1
        if len(stack) == 0:
            return None
        else:
            t = stack[-1]
            if self.is_goalstate(t):
                print(self.i)
                return t
            else:
                succ = t.succ()
                while succ and not succ.to_list() in self.found:
                    stack.append(succ)
                    self.found.append(succ.to_list())
                    r = self.backtrack(stack)
                    if r:
                        return r
                    succ = t.succ()
                stack.pop()

    def is_goalstate(self, state):

        # check whether the puzzle is fully completed
        for x in range(0, state.N2):
            for y in range(0, state.N2):
                if state.board[x, y] == 0:
                    return False

        # check for sum in rows, columns and blocks to discard faulty solutions
        for i in range(0, state.N2):
            if sum(state.row(i)) != 45 or sum(state.column(i)) != 45 or sum(state.block(i)) != 45:
                return False

        # check whether every number occurs exactly once in every row, column and block
        for i in range(0, state.N2):
            if len(set(state.row(i))) != 9 or len(set(state.column(i))) != 9 or len(set(state.block(i))) != 9:
                return False


        return True