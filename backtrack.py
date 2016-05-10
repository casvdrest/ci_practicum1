# runs the backtrack search algorithm

class BackTrack:

    def __init__(self, puzzle):
        self.puzzle = puzzle

    def backtrack(self, stack): 
        if len(stack) == 0:
            return None
        else
            if is_goalstate(t)
                return t
            else
                t = stack[-1]
                while : # unexplored successors and not found
                    stack.append(next_successor(t))
                    backtrack(stack)
                stack.pop()

    def next_successor(self, t):
        return None

    def is_goalstate(self, t):
        return None


                        