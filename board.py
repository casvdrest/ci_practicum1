class Board:

    def __init__(self, N):
        self.N = N
        self.board = {}
        for i in range(0,N*N):
            for j in range(0,N*N):
                self.board[i,j] = 0

    def place(self, x, y, n):
        self.board[x,y] = n

    def illegal(self):
        

    def print(self):
        for i in range(0,self.N):
            for j in range(0,self.N):
                st = ''
                for k in range(0,self.N*self.N):
                    st += self.parse(self.board[i*self.N+j,k])
                    st += ' '
                    if k % self.N == self.N - 1:
                        st += ' '
                print(st)
            print()

    def parse(self, n):
        if n > 0:
            return str(n)
        else:
            return '_'

