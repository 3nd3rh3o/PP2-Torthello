import curses
import argparse
import time
import Game 
import aiPlayers 

def print_menu(stdscr, selected_row_idx, menu):
    stdscr.clear()
    h, w = stdscr.getmaxyx()
    for idx, row in enumerate(menu):
        x = w//2 - len(row)//2
        y = h//2 - len(menu)//2 + idx
        if idx == selected_row_idx:
            stdscr.attron(curses.color_pair(1))
            stdscr.addstr(y, x, row)
            stdscr.attroff(curses.color_pair(1))
        else:
            stdscr.addstr(y, x, row)
    stdscr.refresh()

def print_board(stdscr, game, selected_move_idx):
    stdscr.clear()
    h, w = stdscr.getmaxyx()
    board = game.board
    moves = game.getNextMoves()
    current_player = "White" if game.getPlayer() == 1 else "Black"
    player_msg = f"Player {current_player}'s turn"
    
    # Print the current player message
    x = w//2 - len(player_msg)//2
    y = h//2 - len(board) - 2
    stdscr.addstr(y, x, player_msg)
    
    for i, row in enumerate(board):
        for j, cell in enumerate(row):
            x = w//2 - len(row)*2 + j*4
            y = h//2 - len(board) + i*2
            if (i, j) in moves and moves.index((i, j)) == selected_move_idx:
                stdscr.attron(curses.color_pair(1))
                if cell == 1:
                    stdscr.addstr(y, x, ' o ', curses.color_pair(1))
                elif cell == 2:
                    stdscr.addstr(y, x, ' o ', curses.color_pair(2))
                else:
                    stdscr.addstr(y, x, ' . ')
                stdscr.attroff(curses.color_pair(1))
            else:
                if cell == 1:
                    stdscr.addstr(y, x, ' o ', curses.color_pair(1))
                elif cell == 2:
                    stdscr.addstr(y, x, ' o ', curses.color_pair(2))
                else:
                    stdscr.addstr(y, x, ' . ')
    stdscr.refresh()

def show_ai_move_feedback(stdscr, game, move, flipped_positions):
    h, w = stdscr.getmaxyx()
    i, j = move
    x = w//2 - len(game.board[0])*2 + j*4
    y = h//2 - len(game.board) + i*2

    # Show the AI move (blink)
    for _ in range(4):
        stdscr.attron(curses.color_pair(2))
        stdscr.addstr(y, x, ' o ')
        stdscr.attroff(curses.color_pair(2))
        stdscr.refresh()
        time.sleep(0.25)
        stdscr.addstr(y, x, '   ')
        stdscr.refresh()
        time.sleep(0.25)
    
    # highlight the flipped pawns
    for pos in flipped_positions:
        i, j = pos
        x = w//2 - len(game.board[0])*2 + j*4
        y = h//2 - len(game.board) + i*2
        stdscr.attron(curses.color_pair(1))
        stdscr.addstr(y, x, ' o ', curses.color_pair(1))
        stdscr.attroff(curses.color_pair(1))
    stdscr.refresh()
    time.sleep(0.5)

def show_menu(stdscr, menu):
    current_row = 0
    print_menu(stdscr, current_row, menu)
    while True:
        key = stdscr.getch()
        if key == curses.KEY_UP and current_row > 0:
            current_row -= 1
        elif key == curses.KEY_DOWN and current_row < len(menu) - 1:
            current_row += 1
        elif key == curses.KEY_ENTER or key in [10, 13]:
            return current_row
        elif key == 27:
            return None
        print_menu(stdscr, current_row, menu)

def playGame(stdscr, game, ai1=None, ai2=None):
    selected_move_idx = 0
    while True:
        if game.isOver():
            break
        print_board(stdscr, game, selected_move_idx)
        key = stdscr.getch()
        moves = game.getNextMoves()
        player = game.getPlayer()
        match (ai1,ai2):
            case (None, None):
                if key == curses.KEY_LEFT:
                    selected_move_idx = (len(moves)-1 if selected_move_idx == 0 else selected_move_idx - 1)
                elif key == curses.KEY_RIGHT:
                    selected_move_idx = (0 if selected_move_idx == len(moves)-1 else selected_move_idx + 1)
                elif key == curses.KEY_ENTER or key in [10, 13]:
                    i, j = moves[selected_move_idx]
                    game.play(i, j)
                    selected_move_idx = 0
                    #print_board(stdscr, game, selected_move_idx)  # Refresh the board after player's move
                elif key == 27:  # ESC key to go back to menu
                    break
            case (ai1, None):
                if player == 1:
                    move = ai1.get_move()
                    flipped_positions = game.play(move[0], move[1])
                    #print_board(stdscr, game, selected_move_idx)  # Refresh the board after AI's move
                    show_ai_move_feedback(stdscr, game, move, flipped_positions)
                else:
                    if key == curses.KEY_LEFT:
                        selected_move_idx = (len(moves)-1 if selected_move_idx == 0 else selected_move_idx - 1)
                    elif key == curses.KEY_RIGHT:
                        selected_move_idx = (0 if selected_move_idx == len(moves)-1 else selected_move_idx + 1)
                    elif key == curses.KEY_ENTER or key in [10, 13]:
                        i, j = moves[selected_move_idx]
                        game.play(i, j)
                        selected_move_idx = 0
                        #print_board(stdscr, game, selected_move_idx)  # Refresh the board after player's move
                    elif key == 27:
                        break
            case (ai1, ai2):
                move = ai1.get_move() if player == 1 else ai2.get_move()
                flipped_positions = game.play(move[0], move[1])
                #print_board(stdscr, game, selected_move_idx)  # Refresh the board after AI's move
                show_ai_move_feedback(stdscr, game, move, flipped_positions)

def main(stdscr, args):
    curses.curs_set(0)
    curses.init_pair(1, curses.COLOR_BLACK, curses.COLOR_WHITE)
    current_row = 0
    menu = ['Test', "Against IA", 'Reset', 'Quit']
    menuIA= ['Random',  'Greedy', 'Minimax', 'AlphaBeta', 'MonteCarlo']
    game = Game.Game(args.size if args.size >= 8 else 8)

    current_row = show_menu(stdscr, menu)
    while True:
        match current_row:
            case 0:  # Option 'Test'
                playGame(stdscr, game)
            case 1:  # Option 'Choix IA'
                current_row = show_menu(stdscr, menuIA)
                if current_row:
                    ai = aiPlayers.getIA(menuIA[current_row], game)
                    playGame(stdscr, game, ai)
            case 2:  # Option 'Reset'
                game = Game.Game(args.size)
                current_row = show_menu(stdscr, menu)
            case 3: # Option 'Quit'
                break
            case None:
                break
            
if __name__ == "__main__":
    parser = argparse.ArgumentParser(description='Play Othello')
    parser.add_argument('--size', type=int, default=8, help='Size of the board')
    parser.add_argument('--player', type=int, default=1, help='Color to play (1 for white, 2 for black)')
    args = parser.parse_args()
    curses.wrapper(main, args)