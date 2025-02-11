using UnityEngine;

namespace Tortello
{
    /// <summary>
    /// Doit contenir un générateur de forme, un générateur de materiau, un gestionnaire d'input et un graphe.
    /// </summary>
    
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class Board : MonoBehaviour
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


        /// <summary>
        /// Appelé lorsque le GO ou le script est activé.
        /// </summary>
        public void OnEnable()
        {
            MeshRenderer mR = GetComponent<MeshRenderer>();
            MeshFilter mF = GetComponent<MeshFilter>();
            MeshGenerator.InitMesh(mF);
            MaterialHandler.InitMeshRenderer(mR);
            inputSystem.Init();
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
            MaterialHandler.SetHoveredTile(inputSystem.GetTileHoveredID());
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