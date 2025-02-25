using System.Collections.Generic;
using System.Threading.Tasks;
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
        private bool aiThinking = false;


        private float coolDown = 0f;

        /// <summary>
        /// Appelé lorsque le GO ou le script est activé.
        /// </summary>
        public void OnEnable()
        {


            // on init les composants
            MeshRenderer mR = GetComponent<MeshRenderer>();
            if (mR == null) mR = gameObject.AddComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            if (mF == null) mF = gameObject.AddComponent<MeshFilter>();
            settings.yaw = 0f;
            settings.pitch = 120f;
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
            GameLoop();
            MaterialHandler.UpdateMeshRenderer(mR);
        }

        private async void GameLoop()
        {
            int hoveredTile = -1;
            if (settings.isInGame)
            {
                hoveredTile = inputSystem.GetTileHoveredID();

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
                            settings.turn = settings.PlayerBlanc == PlayerType.Human ? "Blanc" : "Blanc(IA)";
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
                else if (settings.PlayerNoir == PlayerType.MiniMax && couleur == Couleur.Noir && coolDown > 2f && !aiThinking)
                {
                    coolDown = 0f;
                    aiThinking = true;
                    int bestMove = await aiPlayerNoir.GetBestMove();
                    aiThinking = false;

                    if (bestMove != -1)
                    {
                        List<List<int>> pionRetourne = new();
                        if (Graph.AddPawn(bestMove, couleur, pionRetourne))
                        {
                            pawnProccessor.SpawnPawn(bestMove, couleur);
                            pawnProccessor.FlipAnimSeq(pionRetourne);
                            couleur = Couleur.Blanc; // Change le joueur actif à l'humain
                            settings.turn = "Blanc";
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
                            settings.turn = settings.PlayerNoir == PlayerType.Human ? "Noir" : "Noir(IA)";
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
                else if (settings.PlayerBlanc == PlayerType.MiniMax && couleur == Couleur.Blanc && coolDown > 2f && !aiThinking)
                {
                    coolDown = 0f;
                    aiThinking = true;
                    int bestMove = await aiPlayerBlanc.GetBestMove();
                    aiThinking = false;
                    if (bestMove != -1)
                    {
                        List<List<int>> pionRetourne = new();
                        if (Graph.AddPawn(bestMove, couleur, pionRetourne))
                        {
                            pawnProccessor.SpawnPawn(bestMove, couleur);
                            pawnProccessor.FlipAnimSeq(pionRetourne);
                            couleur = Couleur.Noir; // Change le joueur actif à l'humain
                            settings.turn = "Noir";
                            if (Graph.NoPlacementAvailable(couleur))
                            {
                                settings.turn = "FINI!";
                            }
                        }
                    }
                }
                if (inputSystem.Reset()) StartGame();
                MaterialHandler.SetHoveredTile(hoveredTile);
            }
            if (settings.startCMD) StartGame();
            if (settings.rebuildBoardCMD)
            {
                settings.isInGame = false;
                settings.rebuildBoardCMD = false;
                settings.yaw = 0f;
                settings.pitch = 120f;
                coolDown = 0f;
                Graph.RemoveAllPawns();
                pawnProccessor.RemoveAllPawns();
                MaterialHandler.SetHoveredTile(-1);
            }
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