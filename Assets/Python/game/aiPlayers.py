import random
import numpy as np
import Game

def getIA(name, game):
    if name == 'Random':
        return RandomAI(game)
    elif name == 'Greedy':
        return GreedyAI(game)
    elif name == 'Minimax':
        return MinimaxAI(game)
    elif name == 'AlphaBeta':
        return AlphaBetaAI(game)
    elif name == 'MonteCarlo':
        return MonteCarloAI(game)
    return None
def count_differences(matrix1, matrix2):
    matrix1 = np.array(matrix1)
    matrix2 = np.array(matrix2)
    return np.sum(matrix1 != matrix2)

class RandomAI:
    def __init__(self, game):
        self.game = game

    def get_move(self):
        moves = self.game.getNextMoves()
        if moves:
            return random.choice(moves)
        return None

class GreedyAI:
    def __init__(self, game):
        self.game = game

    def get_move(self):
        moves = self.game.getNextMoves()
        if moves:
            best_move = None
            max_flips = -1
            for move in moves:
                flips = self.simulate_move(move)
                if flips > max_flips:
                    max_flips = flips
                    best_move = move
            return best_move
        return None

    def simulate_move(self, move):
        i, j = move
        simulatedGame = self.game.copy()
        simulatedGame.play(i, j)
        flips = count_differences(self.game.getBoard(), simulatedGame.getBoard())
        return flips

# WIP algo mal implémenté (mb j'avais mal compris): à refaire 
class MinimaxAI: # TODO: improve performance
    def __init__(self, game, depth=3):
        self.game = game
        self.depth = depth

    def get_move(self):
        moves = self.game.getNextMoves()
        if moves:
            best_move = None
            max_score = -1
            for move in moves:
                score = self.minimax(move, self.depth, True)
                if score > max_score:
                    max_score = score
                    best_move = move
            return best_move
        return None

    def minimax(self, move, depth, maximizing):
        i, j = move
        gameSimulation = self.game.copy() # Memory overused (new object at each level of recursion --> bad) needs to be refactored
        gameSimulation.play(i, j) # .play() change de joueur actif
        if depth == 0 or gameSimulation.isOver():
            score = gameSimulation.evaluate()
            return score
        moves = gameSimulation.getNextMoves() #
        if maximizing: # on maximise le coup de l'adversaire
            max_score = -1
            for move in moves:
                score = self.minimax(move, depth - 1, False)
                max_score = max(max_score, score)
            return max_score
        else: # 
            min_score = 100000
            for move in moves:
                score = self.minimax(move, depth - 1, True)
                min_score = min(min_score, score)
            return min_score

    def evaluate(self): # not sure if this is a correct score evaluation
        results = self.game.getResults()
        if results == 1:
            return 100
        elif results == 2:
            return -100
        return 0
#WIP basé sur minimax donc j'ai fait faux également: à refaire
class AlphaBetaAI: # TODO: improve performance
    def __init__(self, game, depth=3):
        self.game = game
        self.depth = depth

    def get_move(self):
        moves = self.game.getNextMoves()
        if moves:
            best_move = None
            max_score = -1
            alpha = -1000
            beta = 1000
            for move in moves:
                score = self.alphabeta(move, self.depth, alpha, beta, True)
                if score > max_score:
                    max_score = score
                    best_move = move
                alpha = max(alpha, score)
            return best_move
        return None

    def alphabeta(self, move, depth, alpha, beta, maximizing):
        i, j = move
        original_board = self.game.getBoard().copy()
        self.game.play(i, j)
        if depth == 0 or self.game.isOver():
            score = self.evaluate()
            self.game.board = original_board
            return score
        moves = self.game.getNextMoves()
        if maximizing:
            max_score = -1
            for move in moves:
                score = self.alphabeta(move, depth - 1, alpha, beta, False)
                max_score = max(max_score, score)
                alpha = max(alpha, score)
                if beta <= alpha:
                    break
            self.game.board = original_board
            return max_score
        else:
            min_score = 100000
            for move in moves:
                score = self.alphabeta(move, depth - 1, alpha, beta, True)
                min_score = min(min_score, score)
                beta = min(beta, score)
                if beta <= alpha:
                    break
            self.game.board = original_board
            return min_score

    def evaluate(self):
        results = self.game.getResults()
        if results[0] == 1:
            return 100
        elif results[0] == 2:
            return -100
        return 0
#WIP faut que je relise la définition de l'algo, il est actuellement non-fonctionnel: à refaire
class MonteCarloAI: # TODO: improve performance
    def __init__(self, game, iterations=1000):
        self.game = game
        self.iterations = iterations

    def get_move(self):
        moves = self.game.getNextMoves()
        if moves:
            best_move = None
            max_score = -1
            for move in moves:
                score = self.monte_carlo(move)
                if score > max_score:
                    max_score = score
                    best_move = move
            return best_move
        return None

    def monte_carlo(self, move): 
        i, j = move
        gameSimulation = self.game.copy()
        gameSimulation.play(i, j)
        wins = 0
        for _ in range(self.iterations):
            gameSimulation = self.game.copy()
            while not gameSimulation.isOver():
                moves = gameSimulation.getNextMoves()
                move = random.choice(moves)
                gameSimulation.play(move[0], move[1])
            results = gameSimulation.getResults()
            if results[0] == 1:
                wins += 1
        return wins
    