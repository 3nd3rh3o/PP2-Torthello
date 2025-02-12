using UnityEngine;

namespace Tortello
{
    public class DefaultPawn : Pawn
    {
        private bool SpawnAnim = true;
        private bool FlipAnim = false;

        private float SpawnAnimT = 0f;
        private float FlipAnimT = 0f;
        public void StartSpawnAnim()
        {
            SpawnAnim = true;
            SpawnAnimT = 0f;
            transform.localPosition = pos + new Vector3(0f, 1f, 0f);
            transform.localRotation = couleur == Couleur.Noir ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            gameObject.SetActive(true);
            
        }

        public void StartFlipAnim()
        {
            couleur = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
            FlipAnimT = 0f;
            transform.localPosition = pos + new Vector3(0f, 1f, 0f);
            transform.localRotation = couleur == Couleur.Blanc ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);
            FlipAnim = true;
        }

        public void EndSpawnAnim()
        {
            SpawnAnim = false;
            SpawnAnimT = 0f;
            transform.localPosition = pos;
            transform.localRotation = couleur == Couleur.Noir ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);
        }

        public void EndFlipAnim()
        {
            FlipAnim = false;
            FlipAnimT = 0f;
            transform.localPosition = pos;
            transform.localRotation = couleur == Couleur.Noir ? Quaternion.Euler(180f, 0f, 0f) : Quaternion.Euler(0f, 0f, 0f);
        }

        new void Update()
        {
            if (SpawnAnim)
            {
                SpawnAnimT += Time.deltaTime * 0.8f;
                transform.localPosition = Vector3.Lerp(pos + new Vector3(0f, 1f, 0f), pos, SpawnAnimT);
                if (SpawnAnimT > 1f) EndSpawnAnim();
            }
            else if (FlipAnim)
            {
                FlipAnimT += Time.deltaTime * 0.9f;
                transform.localRotation = couleur == Couleur.Blanc ? 
                                            (
                                                FlipAnimT < 0.5f?
                                                    Quaternion.Lerp(Quaternion.Euler(180f, 0f, 0f), Quaternion.Euler(90f, 0f, 0f), FlipAnimT * 2f)
                                                    : Quaternion.Lerp(Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(0f, 0f, 0f), (FlipAnimT - 0.5f) * 2f)
                                            )
                                            : 
                                            (
                                                FlipAnimT < 0.5f?
                                                    Quaternion.Lerp(Quaternion.Euler(0f, 0f, 0f), Quaternion.Euler(90f, 0f, 0f), FlipAnimT * 2f)
                                                    : Quaternion.Lerp(Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(180f, 0f, 0f), (FlipAnimT - 0.5f) * 2f)
                                            );
                transform.localPosition = FlipAnimT < 0.8f?
                                                Vector3.Lerp(pos, pos + new Vector3(0f, 1f, 0f), FlipAnimT * 2f)
                                                : Vector3.Lerp(pos + new Vector3(0f, 1f, 0f), pos, (FlipAnimT - 0.5f) * 2f)
                                            ;
                if (FlipAnimT > 1f) EndFlipAnim();
            }
        }
    }
}