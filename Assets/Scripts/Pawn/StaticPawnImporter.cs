using System;
using UnityEngine;

namespace Tortello
{
    /// <summary>
    /// Classe qui permet de charger n'importe quels pions.
    /// <br/>
    /// 
    /// </summary>
    [Serializable]
    public class StaticPawnImporter : MonoBehaviour
    {
        public Mesh DefaultPawnMesh;
        public Material[] DefaultPawnMats;
        internal DefaultPawn SpawnDefaultPawn()
        {
            GameObject go = new("Pawn");
            go.SetActive(false);
            MeshFilter mF = go.AddComponent<MeshFilter>();
            mF.sharedMesh = DefaultPawnMesh;
            MeshRenderer mR = go.AddComponent<MeshRenderer>();
            mR.sharedMaterials = DefaultPawnMats;
            return go.AddComponent<DefaultPawn>();
        }
    }
}