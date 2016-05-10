# runs the backtrack search algorithm

class BackTrack:

    def __init__(self, puzzle):
        self.puzzle = puzzle

    def run(self):
        self.stack = [puzzle]
        backtrack()

    def backtrack(self):
        if len(stack) == 0:
            return []
        else:
            t = stack.pop()

            if isGoalState(t):
                return t
            else:
                successors = getSuccessors(t)
                
                found = False
                for s in successors:
                    stack.append(s)
                    backtrack()

                stack.pop()


    def getSuccessors(self, state):
        successors = []

        for i in range(0, 9):
            for j in range(0, 9):
                if state[i, j] == 0: 
                    for n in range(0, 9):
                        