using System.Collections.Generic;
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
            if (pawnProccessor is ToreBoardPawnProcessor torePawnProccessorSystem) 
            {
                //TODO create tex if null;
                if(settings.minimapRT == null)
                {
                    settings.minimapRT = new(256, 256, 0, RenderTextureFormat.ARGB32);
                    settings.minimapRT.dimension = UnityEngine.Rendering.TextureDimension.Tex2D;
                    settings.minimapRT.enableRandomWrite = true;
                    settings.minimapRT.Create();
                }
                
            }
            pawnProccessor.StartGame();
        }

        /// <summary>
        /// Appelé à chaque frame.
        /// </summary>
        public async void Update()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.UpdateMesh(mF);
            inputSystem.Update();
            pawnProccessor.Update();
            Graph.UpdateGraph();

            int hoveredTile = -1;
            if (settings.isInGame)
            {
                hoveredTile = inputSystem.GetTileHoveredID();
                // Torus rotation handling.
                if (inputSystem is ToreBoardInputManager toreInputSystem)
                {
                    if (settings.rotAnimU || settings.rotAnimD)
                    {
                        if (settings.rotAnimT > 1f)
                        {
                            settings.rotAnimU = false;
                            settings.rotAnimD = false;
                        }
                        else
                        {
                            settings.rotAnimT += Time.deltaTime * 10f;
                        }
                    }
                    else
                    {
                        if (toreInputSystem.rotateU())
                        {
                            settings.rotationOffset += 10f;
                            settings.rotationOffset %= 360f; // S'assurer que l'offset reste dans [0, 360]
                            settings.rotAnimT = 0f;
                            settings.rotAnimU = true;
                        }
                        else if (toreInputSystem.rotateD())
                        {
                            settings.rotationOffset -= 10f;
                            settings.rotationOffset %= 360f; // S'assurer que l'offset reste dans [0, 360]
                            settings.rotAnimT = 0f;
                            settings.rotAnimD = true;
                        }
                    }
                }
                if (coolDown < 2f) coolDown += Time.deltaTime;
                if (!aiThinking)
                {
                    if ((HumanTurn() && inputSystem.Place()) || (!HumanTurn() && coolDown > 2f))
                    {
                        if (!HumanTurn() && coolDown > 2f)
                        {
                            aiThinking = true;
                            hoveredTile = couleur == Couleur.Noir ? 
                                await aiPlayerNoir.GetBestMove() : 
                                await aiPlayerBlanc.GetBestMove();
                            aiThinking = false;
                        }

                        List<List<int>> pionRetourne = new();

                        if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                        {
                            pawnProccessor.SpawnPawn(hoveredTile, couleur);
                            pawnProccessor.FlipAnimSeq(pionRetourne);
                            couleur = couleur.Inverse();
                            settings.turn = couleur == Couleur.Blanc ?
                                  (settings.PlayerBlanc == PlayerType.Human ? "Blanc" : "Blanc(IA)") :
                                  (settings.PlayerNoir == PlayerType.Human ? "Noir" : "Noir(IA)");
                            if (Graph.NoPlacementAvailable(couleur)) settings.turn = "FINI!" + " Gagnant: "
                                                        + (Graph.GetScore()[0] > Graph.GetScore()[1] ? "Blanc" : 
                                                           Graph.GetScore()[0] < Graph.GetScore()[1] ? "Noir" : 
                                                           "Egalité");
                        }
                        else MaterialHandler.FailedPlacement();
                        
                        coolDown = 0f;
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


        bool HumanTurn()
        {
            return (couleur == Couleur.Noir && settings.PlayerNoir == PlayerType.Human) || (couleur == Couleur.Blanc && settings.PlayerBlanc == PlayerType.Human);
        }
    }
}