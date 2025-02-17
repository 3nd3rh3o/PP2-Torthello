using System;
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class PlayerMiniMax : IPlayerAI
    {
        private IGraph graph;
        private Couleur couleur;
        private int maxDepth = 3; // Profondeur maximale de l'exploration

        public PlayerMiniMax(IGraph graph, Couleur couleur)
        {
            this.graph = graph;
            this.couleur = couleur;
        }

        public int GetBestMove()
        {
            List<int> validMoves = GetValidMoves();
            int bestMove = -1;
            int bestValue = int.MinValue;

            foreach (int move in validMoves)
            {
                List<List<int>> pawnsToFlip = new List<List<int>>();
                graph.AddPawn(move, couleur, pawnsToFlip);
                int moveValue = Minimax(graph, maxDepth, false, couleur);
                graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }

            return bestMove;
        }

        private int Minimax(IGraph graph, int depth, bool isMaximizingPlayer, Couleur playerColor)
        {
            if (depth == 0 || graph.IsGameOver())
            {
                return Evaluate(graph, playerColor);
            }

            List<int> validMoves = GetValidMoves();
            if (isMaximizingPlayer)
            {
                int bestValue = int.MinValue;
                foreach (int move in validMoves)
                {
                    List<List<int>> pawnsToFlip = new List<List<int>>();
                    graph.AddPawn(move, playerColor, pawnsToFlip);
                    int value = Minimax(graph, depth - 1, false, playerColor);
                    graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                    bestValue = Math.Max(bestValue, value);
                }
                return bestValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Couleur opponentColor = (playerColor == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir;
                foreach (int move in validMoves)
                {
                    List<List<int>> pawnsToFlip = new List<List<int>>();
                    graph.AddPawn(move, opponentColor, pawnsToFlip);
                    int value = Minimax(graph, depth - 1, true, playerColor);
                    graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                    bestValue = Math.Min(bestValue, value);
                }
                return bestValue;
            }
        }

        private List<int> GetValidMoves()
        {
            List<List<int>> dummyList = new List<List<int>>();
            List<int> validMoves = new List<int>();
            for (int i = 0; i < graph.GetBoardSize(); i++)
            {
                if (graph.IsValidMove(i, couleur, dummyList))
                {
                    validMoves.Add(i);
                }
            }
            return validMoves;
        }

        private int Evaluate(IGraph graph, Couleur playerColor)
        {
            // Utiliser la méthode GetScore pour évaluer le score
            List<int> scores = graph.GetScore();
            int scoreBlanc = scores[0];
            int scoreNoir = scores[1];

            // Retourner le score en fonction de la couleur du joueur
            return playerColor == Couleur.Blanc ? scoreBlanc - scoreNoir : scoreNoir - scoreBlanc;
        }
    }
}