import time

# define globals
N  = 0
N2 = 0
digits = list()
rows = list()
cols = list()
rblocks = list()
cblocks = list()
squares = list()
units = dict()
peers = dict()


def cross(A, B):
    "Cross product of elements in A and elements in B."
    return [a+b for a in A for b in B]

def read_N():
    n = int(input("Please enter N: "))
    return n if n in [3, 4, 5] else 3

def read_puzzle():
    puzzle = list()
    for i in range(N2):
        if N > 3:
            puzzle = puzzle + input().split()
        else:
            puzzle = puzzle + list(input())
    return [chr(int(x) + 97) for x in puzzle]

def parse_grid(grid):
    """Convert grid to a dict of possible values, {square: digits}, or
    return False if a contradiction is detected."""
    ## To start, every square can be any digit; then assign values from the grid.
    values = dict((s, digits) for s in squares)

    for s,d in grid_values(grid).items():
        if d in digits and not assign(values, s, d):
            return False ## (Fail if we can't assign d to square s.)
    return values

def grid_values(grid):
    return dict(zip(squares, grid))

def assign(values, s, d):
    """Eliminate all the other values (except d) from values[s] and propagate.
    Return values, except return False if a contradiction is detected."""
    other_values = values[s].replace(d, '')
    if all(eliminate(values, s, d2) for d2 in other_values):
        return values
    else:
        return False

def eliminate(values, s, d):
    """Eliminate d from values[s]; propagate when values or places <= 2.
    Return values, except return False if a contradiction is detected."""
    if d not in values[s]:
        return values ## Already eliminated
    values[s] = values[s].replace(d,'')
    ## (1) If a square s is reduced to one value d2, then eliminate d2 from the peers.
    if len(values[s]) == 0:
        return False ## Contradiction: removed last value
    elif len(values[s]) == 1:
        d2 = values[s]
        if not all(eliminate(values, s2, d2) for s2 in peers[s]):
            return False
    ## (2) If a unit u is reduced to only one place for a value d, then put it there.
    for u in units[s]:
        dplaces = [s for s in u if d in values[s]]
        if len(dplaces) == 0:
            return False ## Contradiction: no place for this value
        elif len(dplaces) == 1:
            # d can only be in one place in unit; assign it there
                if not assign(values, dplaces[0], d):
                    return False
    return values

def display(values):
    width = 2
    if N > 3:
        width = width + 1

    cdelim = ''.join([cblocks[i][-1] for i in range(N)])
    rdelim = ''.join([rblocks[i][-1] for i in range(N)])

    vline =  '+' + '+'.join(['-'*N * width]*N) + '+'
    print(vline)
    for r in rows:
        line = '|'
        for c in cols:
            intval = ord(values[r + c]) - 97
            line = line + (" " if intval > 9 or N == 3 else "  ") + str(intval)
            if c in cdelim:
                line = line + '|'
        print(line)
        if r in rdelim:
            print(vline)

def solve(grid): return search(parse_grid(grid))

def search(values):
    "Using depth-first search and propagation, try all possible values."
    if values is False:
        return False ## Failed earlier
    if all(len(values[s]) == 1 for s in squares): 
        return values ## Solved!
    ## Chose the unfilled square s with the fewest possibilities
    n,s = min((len(values[s]), s) for s in squares if len(values[s]) > 1)
    return some(search(assign(values.copy(), s, d)) for d in values[s])

def some(seq):
    "Return some element of seq that is true."
    for e in seq:
        if e: return e
    return False

def run():
    global N, N2

    # initialize puzzle dimensions
    N        = read_N()
    N2       = N**2

    # initialize other variables
    init_vars()

    # read input puzzle, solve it and display results
    display(solve(read_puzzle()))

# initialize variables
def init_vars():
    global digits, rows, cols, rblocks, cblocks, squares, units, peers

    digits   = ''.join([chr(c) for c in range(98, 98 + N2)])
    rows     = ''.join([chr(c) for c in range(65, 65 + N2)])
    cols     = digits

    rblocks  = [rows[i * N : i * N + N] for i in range(0, N)]
    cblocks  = [cols[i * N : i * N + N] for i in range(0, N)]

    squares  = cross(rows, cols)
    unitlist = ([cross(rows, c) for c in cols] +
                [cross(r, cols) for r in rows] +
                [cross(rs, cs) for rs in rblocks for cs in cblocks])

    units    = dict((s, [u for u in unitlist if s in u]) 
                    for s in squares)
    peers    = dict((s, set(sum(units[s],[]))-set([s]))
                    for s in squares)

# run test puzzles
def run_tests(n):
    global N, N2

    N  = n
    N2 = n**2

    init_vars()

    count = int(input("Testpuzzle count: "))

    puzzles = list()
    #read test puzzles. Assume a horizontal delimiter before every puzzle
    for i in range(count):
        input()
        puzzles.append(read_puzzle())

    totalSolvingTime = 0
    for p in puzzles:
        start = time.clock()
        result = solve(p)
        diff = time.clock() - start
        print("Solved puzzle in " + str(diff) + " seconds. ")
        display(result)
        totalSolvingTime = totalSolvingTime + diff

    print("Solved " + str(count) + " " + str(N2) + "x" + str(N2) + " puzzles in " + str(totalSolvingTime) + " seconds, averaging " + str(totalSolvingTime / count) + " seconds per puzzle.") 

    
