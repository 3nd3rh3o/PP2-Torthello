using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

namespace Torthello
{
    public class TorBoard : Board
    {
        public TorBoardSettings settings;
        public InputActionAsset actionMap;
        private IPlayerAI aiPlayerNoir;
        private IPlayerAI aiPlayerBlanc;

        new void OnEnable()
        {
            MeshGenerator = new TorBoardMeshGenerator(settings);
            MaterialHandler = new TorBoardMaterialHandler(settings);
            Graph = new TorBoardGraph(settings);
            inputSystem = new TorBoardInputSystem(settings, transform, actionMap, (TorBoardMeshGenerator)MeshGenerator);
            pawnProccessor = new TorBoardPawnProcessor(transform, settings, (TorBoardMeshGenerator)MeshGenerator);
            aiPlayerNoir = new PlayerMiniMax(Graph, Couleur.Noir);
            aiPlayerBlanc = new PlayerMiniMax(Graph, Couleur.Blanc);
            base.OnEnable();
        }

        void Update()
        {
            //viré pck évidemment que le jeu est terminé avant l'initialisation
            /*
            if (Graph.IsGameOver())
            {
                return; // Arrêter le jeu si celui-ci est terminé
            }
            */
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.UpdateMesh(mF);
            inputSystem.Update();
            int hoveredTile = inputSystem.GetTileHoveredID();
            MaterialHandler.SetHoveredTile(hoveredTile);
            Debug.Log("Couleur: " + couleur);
            Debug.Log("PlayerNoir: " + settings.PlayerNoir);
            if (settings.PlayerNoir == PlayerType.Human && couleur == Couleur.Noir)
            {
                if (inputSystem.Place())
                {
                    List<List<int>> pionRetourne = new();
                    if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                    {
                        pawnProccessor.SpawnPawn(hoveredTile, couleur);
                        pawnProccessor.FlipAnimSeq(pionRetourne);
                        couleur = Couleur.Blanc; // Change le joueur actif à l'IA
                        if (Graph.NoPlacementAvailable(couleur))
                        {
                            List<int> score = Graph.GetScore();
                            Debug.Log("Score Blanc: " + score[0] + " Score Noir: " + score[1]);
                        }
                    }
                    else
                    {
                        MaterialHandler.FailedPlacement();
                    }
                }
            }
            else if (settings.PlayerNoir == PlayerType.MiniMax && couleur == Couleur.Noir)
            {
                int bestMove = aiPlayerNoir.GetBestMove();
                if (bestMove != -1)
                {
                    List<List<int>> pionRetourne = new();
                    if (Graph.AddPawn(bestMove, couleur, pionRetourne))
                    {
                        pawnProccessor.SpawnPawn(bestMove, couleur);
                        pawnProccessor.FlipAnimSeq(pionRetourne);
                        couleur = Couleur.Blanc; // Change le joueur actif à l'humain
                        if (Graph.NoPlacementAvailable(couleur))
                        {
                            List<int> score = Graph.GetScore();
                            Debug.Log("Score Blanc: " + score[0] + " Score Noir: " + score[1]);
                        }
                    }
                }
            }
            else if (settings.PlayerBlanc == PlayerType.Human && couleur == Couleur.Blanc)
            {
                if (inputSystem.Place())
                {
                    List<List<int>> pionRetourne = new();
                    if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                    {
                        pawnProccessor.SpawnPawn(hoveredTile, couleur);
                        pawnProccessor.FlipAnimSeq(pionRetourne);
                        couleur = Couleur.Noir; // Change le joueur actif à l'IA
                        if (Graph.NoPlacementAvailable(couleur))
                        {
                            List<int> score = Graph.GetScore();
                            Debug.Log("Score Blanc: " + score[0] + " Score Noir: " + score[1]);
                        }
                    }
                    else
                    {
                        MaterialHandler.FailedPlacement();
                    }
                }
            }
            else if (settings.PlayerBlanc == PlayerType.MiniMax && couleur == Couleur.Blanc)
            {
                int bestMove = aiPlayerBlanc.GetBestMove();
                if (bestMove != -1)
                {
                    List<List<int>> pionRetourne = new();
                    if (Graph.AddPawn(bestMove, couleur, pionRetourne))
                    {
                        pawnProccessor.SpawnPawn(bestMove, couleur);
                        pawnProccessor.FlipAnimSeq(pionRetourne);
                        couleur = Couleur.Noir; // Change le joueur actif à l'humain
                        if (Graph.NoPlacementAvailable(couleur))
                        {
                            List<int> score = Graph.GetScore();
                            Debug.Log("Score Blanc: " + score[0] + " Score Noir: " + score[1]);
                        }
                    }
                }
            }

            MaterialHandler.UpdateMeshRenderer(mR);
            if (inputSystem.Reset()) StartGame();
        }
    }
}
