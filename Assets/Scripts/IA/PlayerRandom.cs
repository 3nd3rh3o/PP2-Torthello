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
            List<int> validMoves = GetValidMoves();
            System.Random random = new System.Random();
            return validMoves[random.Next(0, validMoves.Count)];
        }

        private List<int> GetValidMoves()
        {
            Debug.Log("GetValidMoves");
            List<List<int>> dummylist = new List<List<int>>();
            List<int> validMoves = new List<int>();
            for (int i = 0; i < graph.GetBoardSize(); i++)
            {
                if (graph.IsValidMove(i, couleur, dummylist))
                {
                    validMoves.Add(i);
                }
            }
            Debug.Log("ValidMoves: " + validMoves.Count);
            return validMoves;
        }
    }
}