using System;
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class PlayerMiniMax : IPlayerAI
    {
        private IGraph graph;
        private Couleur couleur;
        private int maxDepth = 2; // Profondeur maximale de l'exploration

        public PlayerMiniMax(IGraph graph, Couleur couleur)
        {
            this.graph = graph;
            this.couleur = couleur;
        }

        public int GetBestMove()
        {
            List<int> validMoves = graph.GetValidMoves(couleur);
            int bestMove = -1;
            int bestValue = int.MinValue;

            foreach (int move in validMoves)
            {
                List<List<int>> pawnsToFlip = new List<List<int>>();
                graph.AddPawn(move, couleur, pawnsToFlip);
                int moveValue = Minimax(graph, maxDepth, false, couleur, int.MinValue, int.MaxValue);
                graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre

                if (moveValue > bestValue)
                {
                    bestValue = moveValue;
                    bestMove = move;
                }
            }
            Debug.Log($"Meilleur coup: {bestMove} avec une valeur de {bestValue}");
            return bestMove;
        }

        private int Minimax(IGraph graph, int depth, bool isMaximizingPlayer, Couleur playerColor, int alpha, int beta)
        {
            if (depth <= 0 || graph.IsGameOver())
            {
                return Evaluate(graph, playerColor);
            }
            if (isMaximizingPlayer)
            {
                int bestValue = int.MinValue;
                List<int> validMoves = graph.GetValidMoves(playerColor);
                foreach (int move in validMoves)
                {
                    List<List<int>> pawnsToFlip = new List<List<int>>();
                    if(graph.AddPawn(move, playerColor, pawnsToFlip)){
                        int value = Minimax(graph, depth - 1, false, playerColor, alpha, beta);
                        graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                        bestValue = Math.Max(bestValue, value);
                        alpha = Math.Max(alpha, bestValue);
                        if (beta <= alpha)
                        {
                            break; // Élagage beta
                        }
                    }
                    else {
                        Debug.Log($"Coup invalide tenté par minimax id: {move} profondeur: {depth} pour le joueur {playerColor}");
                    };
                }
                return bestValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                Couleur opponentColor = (playerColor == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir;
                List<int> validMoves = graph.GetValidMoves(opponentColor);
                foreach (int move in validMoves)
                {
                    List<List<int>> pawnsToFlip = new List<List<int>>();
                    if (graph.AddPawn(move, opponentColor, pawnsToFlip)){
                        int value = Minimax(graph, depth - 1, true, playerColor, alpha, beta);
                        graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                        bestValue = Math.Min(bestValue, value);
                        beta = Math.Min(beta, bestValue);
                        if (beta <= alpha)
                        {
                            break; // Élagage alpha
                        }
                    }
                    else {
                        Debug.Log($"Coup invalide tenté par minimax id: {move} profondeur: {depth} pour le joueur {opponentColor}");
                    };
                }
                return bestValue;
            }
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