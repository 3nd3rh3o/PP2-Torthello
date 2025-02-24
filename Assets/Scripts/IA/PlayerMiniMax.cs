using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Torthello
{
    /// <summary>
    /// implémentation d'un joueur IA qui utilise l'algorithme minimax pour choisir le meilleur coup.
    /// </summary>
    public class PlayerMiniMax : IPlayerAI
    {
        private IGraph graph;
        private Couleur couleur;
        private int maxDepth = 5; // Profondeur maximale de l'exploration
        private int nbEval = 0; // Nombre d'évaluations effectuées
        public PlayerMiniMax(IGraph graph, Couleur couleur, int maxDepth = 50)
        {
            this.graph = graph;
            this.couleur = couleur;
            this.maxDepth = maxDepth;
        }

        public async Awaitable<int> GetBestMove()
        {
            await Awaitable.BackgroundThreadAsync();
            List<int> validMoves = graph.GetValidMoves(couleur);
            int bestMove = -1;
            int bestValue = int.MinValue;
            nbEval = 0;

            foreach (int move in validMoves)
            {
                List<List<int>> pawnsToFlip = new List<List<int>>();
                if(graph.AddPawn(move, couleur, pawnsToFlip))
                {
                    int moveValue = Minimax(graph, maxDepth, false, (couleur == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir, int.MinValue, int.MaxValue);
                    graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                    
                    if (moveValue > bestValue)
                    {
                        bestValue = moveValue;
                        bestMove = move;
                    }
                }
                else {
                    Debug.Log($"Coup invalide tenté par minimax id: {move} pour le joueur {couleur}");
                };
            }
            Debug.Log($"Meilleur coup: {bestMove} avec une valeur de {bestValue} pour le joueur {couleur} avec {nbEval} évaluations");
            return bestMove;
        }

        private int Minimax(IGraph graph, int depth, bool isMaximizingPlayer, Couleur playerColor, int alpha, int beta)
        {
           if (depth <= 0 || graph.NoPlacementAvailable(playerColor))
            {
                if(depth<=0)
                    Debug.Log($"Fin de la récursion de minimax à la profondeur {maxDepth-depth}");
                else Debug.Log($"Fin de la récursion de minimax à l'arrêt du jeu");
                Debug.Log($"Évaluation de la position à la profondeur {maxDepth-depth}, blanc: {graph.GetScore()[0]} noir: {graph.GetScore()[1]}");
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
                        int value = Minimax(graph, depth - 1, false, (playerColor == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir, alpha, beta);
                        graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                        bestValue = Math.Max(bestValue, value);
                        alpha = Math.Max(alpha, bestValue);
                        if (beta <= alpha)
                        {
                            break; // Élagage beta
                        }
                    }
                    else {
                        Debug.Log($"Coup invalide tenté par minimax id: {move} profondeur: {maxDepth-depth} pour le joueur {playerColor}");
                    };
                }
                return bestValue;
            }
            else
            {
                int bestValue = int.MaxValue;
                List<int> validMoves = graph.GetValidMoves(playerColor);
                foreach (int move in validMoves)
                {
                    List<List<int>> pawnsToFlip = new List<List<int>>();
                    if (graph.AddPawn(move, playerColor, pawnsToFlip)){
                        int value = Minimax(graph, depth - 1, true, (playerColor == Couleur.Noir) ? Couleur.Blanc : Couleur.Noir, alpha, beta);
                        graph.RemovePawn(move, pawnsToFlip); // Remettre le graphe en ordre
                        bestValue = Math.Min(bestValue, value);
                        beta = Math.Min(beta, bestValue);
                        if (beta <= alpha)
                        {
                            break; // Élagage alpha
                        }
                    }
                    else {
                        Debug.Log($"Coup invalide tenté par minimax id: {move} profondeur: {maxDepth-depth} pour le joueur {playerColor}");
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

            int score = playerColor == Couleur.Blanc ? scoreBlanc - scoreNoir : scoreNoir - scoreBlanc;
            //TESTS DEBUG
            nbEval++;
            Debug.Log($"Score: {score} Blanc: {scoreBlanc} Noir: {scoreNoir}");
            // Retourner le score en fonction de la couleur du joueur
            return score;
        }
    }
}