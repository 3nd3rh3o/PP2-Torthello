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

        public IGraph Graph;
        public Couleur couleur = Couleur.Blanc;


        /// <summary>
        /// Appelé lorsque le GO ou le script est activé.
        /// </summary>
        public void OnEnable()
        {
            Cursor.lockState = CursorLockMode.Confined;
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.InitMesh(mF);
            MaterialHandler.InitMeshRenderer(mR);
            inputSystem.Init();
            Graph.InitGraph();
        }

        /// <summary>
        /// Appelé à chaque frame.
        /// </summary>
        void Update()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.UpdateMesh(mF);            
            inputSystem.Update();
            int hoveredTile = inputSystem.GetTileHoveredID();
            MaterialHandler.SetHoveredTile(hoveredTile);
            if (inputSystem.Place()) {
                List<List<int>> pionRetourne = new();
                if (Graph.AddPawn(hoveredTile, couleur, pionRetourne))
                {
                    // AJOUTER PION + ANIMER RETOURNEMENT
                }
                else
                {
                    MaterialHandler.FailedPlacement();
                }
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
        }
    }
}