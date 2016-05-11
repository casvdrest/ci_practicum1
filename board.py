class Board:

    from copy import deepcopy

    def __init__(self, N):
        self.N = N
        self.N2 = N * N
        self.board = {}
        self.poss = {}
        self.succvar = [0,0,1]
        for i in range(0,self.N2):
            for j in range(0,self.N2):
                self.board[i,j] = 0
        #for i in range(0,self.N2):
        #    for j in range(0,self.N2):
        #        self.poss[i,j] = [1,2,3,4,5,6,7,8,9]

    def read(self, text):
        for i in range(0, self.N2):
            for j in range(0, self.N2):
                v = int(text[j][i])
                self.board[i,j] = v
                if v != 0:
                    self.poss[i,j] = []

    def elim(self):
        for (i,j) in self.board:
            if self.board[i,j] > 0:
                self.rem(i,j,self.board[i,j])

    def rem(self, i, j, v):
        for x in range(0,self.N2):
            if x == i:
                continue
            if v in self.poss[x,j]:
                self.poss[x,j].remove(v)
        for y in range(0,self.N2):
            if y == j:
                continue
            if v in self.poss[i,y]:
                self.poss[i,y].remove(v)

    def succ(self):
        while self.board[self.succvar[0], self.succvar[1]] > 0:
            if self.succvar[0] < 8:
                self.succvar[0] += 1
            elif self.succvar[1] < 8:
                self.succvar[0] = 0
                self.succvar[1] += 1
            else:
                return None
        res = Board(3)
        res.board = self.board.copy()
        res.place(self.succvar[0],self.succvar[1],self.succvar[2])
        if self.succvar[2] == 9:
            self.succvar[2] = 1
            if self.succvar[0] < 8:
                self.succvar[0] += 1
            elif self.succvar[1] < 8:
                self.succvar[0] = 0
                self.succvar[1] += 1
        else:
            self.succvar[2] += 1
        res.succvar = [0, 0, 1]
        return res

    def place(self, x, y, n):
        self.board[x,y] = n

    def legal_moves(self):
        res = []
        for i in self.board:
            if self.board[i] == 0:
                for j in range(0,self.N2):
                    self.place(i[0], i[1], j)
                    if self.legal():
                        res.append((i, j))
                    self.place(i[0], i[1], 0)
        return res

    def legal(self):
        #INCORRECT
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