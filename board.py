class Board:

    def __init__(self, N):
        self.N = N
        self.N2 = N * N
        self.board = {}
        for i in range(0,self.N2):
            for j in range(0,self.N2):
                self.board[i,j] = 0

    def place(self, x, y, n):
        self.board[x,y] = n

    def legal(self):
        for i in range(0,self.N2):
            if len(set([x for x in self.row(i) if self.row(i).count(x) > 1])) > 1:
                return False
            if len(set([x for x in self.column(i) if self.column(i).count(x) > 1])) > 1:
                return False
            if len(set([x for x in self.block(i) if self.block(i).count(x) > 1])) > 1:
                return False
        return True

    def finished(self):
        for i in range(0,self.N2):
            for j in range(0,self.N2):
                if self.board[i,j] == 0:
                    return False
        return self.legal

    def column(self, i):
        res = []
        for j in range(0,self.N2):
            res.append(self.board[i,j])
        return res

    def row(self, i):
        res = []
        for j in range(0,self.N2):
            res.append(self.board[j,i])
        return res

    def block(self, i):
        res = []
        ix = i // self.N
        iy = i % self.N
        for j in range(0,self.N):
            for k in range(0,self.N):
                res.append(self.board[j+ix*self.N,k+iy*self.N])
        return res

    def to_list(self):
        res = []
        for i in range(0,self.N2):
            res.append(self.row(i))
        return res

    def print(self):
        for i in range(0,self.N):
            for j in range(0,self.N):
                st = ''
                for k in range(0,self.N2):
                    st += self.parse(self.board[k,i*self.N+j])
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