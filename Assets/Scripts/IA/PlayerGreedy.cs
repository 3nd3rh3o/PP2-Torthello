using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    /// <summary>
    /// Implémentation d'un joueur IA qui choisit le coup qui lui donne le plus de pions.
    /// équivalent à minimax avec une profondeur de 1 (et évaluation simplifiée)
    /// </summary>
    public class PlayerGreedy : IPlayerAI 
    {
        private IGraph graph;
        private Couleur couleur;

        public PlayerGreedy(IGraph graph, Couleur couleur)
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
                int moveValue = Evaluate(pawnsToFlip);
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

        private int Evaluate(List<List<int>> pawnsflipped)
        {
            int score = 1;
            foreach (List<int> list in pawnsflipped)
            {
                score += list.Count;
            }
            return score;
        }
    }
}