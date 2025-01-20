import numpy as np

class Game:
    def __init__(self, n=None, player=None):
            self.setGameBoard((n) if n != None else 8)
            self.player = 1 if player ==None else player
    
    def setGameBoard(self, n):
        self.board = np.zeros((n, n), dtype=int)
        self.setPawn(n//2-1, n//2-1, 1)
        self.setPawn(n//2, n//2, 1)
        self.setPawn(n//2-1, n//2, 2)
        self.setPawn(n//2, n//2-1, 2)
    
    def setPawn(self,i, j, player):
        i = np.mod(i, len(self.board))
        j = np.mod(j, len(self.board[0]))
        self.board[i][j] = player
        return self.board
    
    def getPlayer(self):
        return self.player
    
    def getBoard(self):
        return self.board
    
    def printBoard(self):
        # ANSI color codes are used to color the output in the terminal.
        # \033[37;1m sets the color to bright white.
        # \033[30;1m sets the color to bright black (gray).
        # \033[0m resets the color to default.
        colors = {
            0: " ",  # Empty
            1: "\033[37;1mo\033[0m",  # White
            2: "\033[30;1mo\033[0m"   # Black
        }
        for row in self.board:
            print("|".join(colors[cell] for cell in row))
        print("\n")
    
    def endTurn(self):
        self.player = 3 - self.player
        
    #TODO: voir en faisant en sorte que verifmove renvoie qq chose, pour gagner en performance (play, isValidMove) par ex si isValidMove est appelée à chaque fois que play est appelée on peut ne pas re-check les directions
    def play(self, i, j):  #TODO: voir si on peut simplifier la fonction play
        if not self.isValidMove(i, j):
            raise ValueError("Le coup tenté n'est pas valide ? C'est dommage")
        self.setPawn(i, j, self.player)  # Place the pawn
        flipped_pawns = []  # List of flipped pawns
        directions = [(-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)]  # Directions for flipping pawns
        for di, dj in directions:
            ni, nj = (i + di) % len(self.board), (j + dj) % len(self.board[0])
            if self.board[ni][nj] != self.player and self.board[ni][nj] != 0:
                while True:
                    if (ni, nj) == (i, j):  # If we reach the initial position
                        break
                    if self.board[ni][nj] == self.player: # If we find a pawn of the same color as th player
                        while (ni, nj) != (i, j):        # Flip the pawns between the two pawns by going back to initial position
                            ni = (ni - di) % len(self.board)
                            nj = (nj - dj) % len(self.board[0])
                            self.board[ni][nj] = self.player
                            flipped_pawns.append((ni, nj))
                        break
                    if self.board[ni][nj] == 0: # If we find an empty cell
                        break
                    ni = (ni + di) % len(self.board)
                    nj = (nj + dj) % len(self.board[0])
                    
        self.endTurn()
        return flipped_pawns
        
    def isValidMove(self, i, j):  
        directions = [(-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)]  # Added diagonal directions
        for di, dj in directions:
            ni, nj = (i + di) % len(self.board), (j + dj) % len(self.board[0])
            if self.board[ni][nj] != self.player and self.board[ni][nj] != 0:
                while True:
                    if self.board[ni][nj] == self.player:
                        return True
                    if self.board[ni][nj] == 0:
                        break
                    ni = (ni + di) % len(self.board)
                    nj = (nj + dj) % len(self.board[0])
                    if (ni, nj) == (i, j):
                        break
        return False
        
    def getNextMoves(self): 
        moves = []
        for i in range(len(self.board)):
            for j in range(len(self.board[0])):
                if self.board[i][j] == 0 and self.isValidMove(i, j):
                    moves.append((i, j))
        return moves
            
    def isOver(self):
        if len(self.getNextMoves()) != 0:
            return False
        self.endTurn()
        if len(self.getNextMoves()) != 0:
            return False
        else:
            return True
    
    def getResults(self):
        if not self.isOver():
            return -1
        white = 0
        black = 0
        for row in self.board:
            for cell in row:
                if cell == 1:
                    white += 1
                elif cell == 2:
                    black += 1
        return [1 if white > black else 2 if black > white else 0]     
    def copy(self):
        game = Game()
        game.board = self.board.copy()
        game.player = self.player
        return game
#Game(8, 2).printBoard()
