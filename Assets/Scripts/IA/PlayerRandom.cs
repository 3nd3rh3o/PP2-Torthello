using System;
using System.Collections.Generic;
using UnityEngine;

namespace Torthello
{
    public class PlayerRandom : IPlayerAI
    {
        private IGraph graph;
        private Couleur couleur;

        public PlayerRandom(IGraph graph, Couleur couleur)
        {
            this.graph = graph;
            this.couleur = couleur;
        }

        public int GetBestMove()
        {
            //retourner un coup aléatoire valide
            List<int> validMoves = graph.GetValidMoves(couleur);
            System.Random random = new System.Random();
            return validMoves[random.Next(0, validMoves.Count)];
        }
    }
}