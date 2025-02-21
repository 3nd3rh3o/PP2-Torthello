using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace Torthello
{
    /// <summary>
    /// Doit contenir un générateur de forme, un générateur de materiau, un gestionnaire d'input et un graphe.
    /// </summary>

    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public abstract class Board : MonoBehaviour
    {
        /// <summary>
        /// Ce qui génère la forme du plateau.
        /// </summary>
        public IMeshGenerator MeshGenerator;
        public int hoveredTile;

        /// <summary>
        /// Ce qui défini le renderer du plateau.
        /// </summary>
        public IMaterialHandler MaterialHandler;

        public IBoardInputSystem inputSystem;
        public IPawnProccessor pawnProccessor;
        public IGraph Graph;
        public Couleur couleur = Couleur.Blanc;
        public Settings settings;
        private IPlayerAI aiPlayerNoir;
        private IPlayerAI aiPlayerBlanc;


        private float coolDown = 0f;

        /// <summary>
        /// Appelé lorsque le GO ou le script est activé.
        /// </summary>
        public void OnEnable()
        {


            // on init les composants
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.InitMesh(mF);
            MaterialHandler.InitMeshRenderer(mR);
            inputSystem.Init();
            Graph.InitGraph();
            pawnProccessor.Init();
            settings.isInGame = false;
            settings.startCMD = false;
            aiPlayerNoir = new PlayerMiniMax(Graph, Couleur.Noir);
            aiPlayerBlanc = new PlayerMiniMax(Graph, Couleur.Blanc);
            //StartGame();
        }

        public void StartGame()
        {
            settings.startCMD = false;
            settings.isInGame = true;
            coolDown = 0f;
            Graph.RemoveAllPawns();
            pawnProccessor.RemoveAllPawns();
            couleur = Couleur.Noir;
            settings.turn = "Noir";
            settings.PlayerBlanc = settings.IA ? PlayerType.MiniMax : PlayerType.Human;
            Graph.StartGame();
            pawnProccessor.StartGame();
        }

        /// <summary>
        /// Appelé à chaque frame.
        /// </summary>
        public void Update()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.UpdateMesh(mF);
            inputSystem.Update();
            Graph.UpdateGraph();

            int hoveredTile = -1;
            if (settings.isInGame)
            {
                hoveredTile = inputSystem.GetTileHoveredID();
                if (hoveredTile != -1)
                {
                    if (coolDown < 2f) coolDown += Time.deltaTime;
                    if (settings.PlayerNoir == PlayerType.Human && couleur == Couleur.Noir)
                    {
                        coolDown = 0f;
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
                                    settings.turn = "FINI!";
                                }
                            }
                            else
                            {
                                MaterialHandler.FailedPlacement();
                            }
                        }
                    }
                    else if (settings.PlayerNoir == PlayerType.MiniMax && couleur == Couleur.Noir && coolDown > 2f)
                    {
                        coolDown = 0f;
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
                                    settings.turn = "FINI!";
                                }
                            }
                        }
                    }
                    else if (settings.PlayerBlanc == PlayerType.Human && couleur == Couleur.Blanc)
                    {
                        coolDown = 0f;
                        if (inputSystem.Place())
                        {
                            List<List<int>> pionRetourne = new();
                            if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                            {
                                pawnProccessor.SpawnPawn(hoveredTile, couleur);
                                pawnProccessor.FlipAnimSeq(pionRetourne);
                                couleur = Couleur.Noir; // Change le joueur actif à l'IA
                                settings.turn = "Noir";
                                if (Graph.NoPlacementAvailable(couleur))
                                {
                                    settings.turn = "FINI!";
                                }
                            }
                            else
                            {
                                MaterialHandler.FailedPlacement();
                            }
                        }
                    }
                    else if (settings.PlayerBlanc == PlayerType.MiniMax && couleur == Couleur.Blanc && coolDown > 2f)
                    {
                        coolDown = 0f;
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
                                    settings.turn = "FINI!";
                                }
                            }
                        }
                    }
                }
            }
            MaterialHandler.SetHoveredTile(hoveredTile);
            if (settings.startCMD) StartGame();
            MaterialHandler.UpdateMeshRenderer(mR);
        }

        /// <summary>
        /// Appelé toute les Time.fixedDeltaTime(intervale constant).
        /// </summary>
        void FixedUpdate()
        {

        }

        /// <summary>
        /// Appelé lorsque le GO ou le script est désactivé.
        /// </summary>
        void OnDisable()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.Destroy(mF);
            MaterialHandler.Destroy(mR);
            inputSystem.Destroy();
            pawnProccessor.Destroy();
        }
    }
}