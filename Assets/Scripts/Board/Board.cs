using System.Collections.Generic;
using UnityEngine;

namespace Tortello
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


        /// <summary>
        /// Ce qui défini le renderer du plateau.
        /// </summary>
        public IMaterialHandler MaterialHandler;

        public IBoardInputSystem inputSystem;
        public IPawnProccessor pawnProccessor;
        public IGraph Graph;
        public Couleur couleur = Couleur.Blanc;
        public Settings settings;


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
            //StartGame();
        }

        public void StartGame()
        {
            settings.startCMD = false;
            settings.isInGame = true;
            Graph.RemoveAllPawns();
            pawnProccessor.RemoveAllPawns();
            couleur = Couleur.Blanc;
            settings.turn = "Blanc";
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
            Graph.UpdateGraph();
            inputSystem.Update();
            if (settings.isInGame)
            {
                int hoveredTile = inputSystem.GetTileHoveredID();
                MaterialHandler.SetHoveredTile(hoveredTile);
                if (inputSystem.Place())
                {
                    List<List<int>> pionRetourne = new();
                    if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                    {
                        pawnProccessor.SpawnPawn(hoveredTile, couleur);
                        pawnProccessor.FlipAnimSeq(pionRetourne);
                        couleur = couleur == Couleur.Noir ? Couleur.Blanc : Couleur.Noir;
                        settings.turn = couleur == Couleur.Noir? "Noir" : "Blanc";
                        if (Graph.NoPlacementAvailable(couleur))
                        {
                            List<int> score = Graph.GetScore();
                            Debug.Log("Score Blanc: " + score[0] + " Score Noir: " + score[1]);
                            settings.turn = "Fini!";
                        }
                    }
                    else
                    {
                        MaterialHandler.FailedPlacement();
                    }
                }
            }

            MaterialHandler.UpdateMeshRenderer(mR);
            if ((settings.isInGame && inputSystem.Reset()) || settings.startCMD) StartGame();
            if (settings.rebuildBoardCMD)
            {
                settings.rebuildBoardCMD = false;
                
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