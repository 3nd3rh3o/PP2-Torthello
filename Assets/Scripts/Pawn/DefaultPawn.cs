using UnityEngine;

namespace Torthello
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
            transform.localPosition = pos + (rot * new Vector3(0f, 1.5f, 0f));
            transform.localRotation = couleur == Couleur.Noir ? rot * Quaternion.Euler(180f, 0f, 0f) : rot * Quaternion.Euler(0f, 0f, 0f);
            gameObject.SetActive(true);
        }

        public void StartFlipAnim()
        {
            couleur = couleur == Couleur.Blanc ? Couleur.Noir : Couleur.Blanc;
            FlipAnimT = 0f;
            transform.localPosition = pos;
            transform.localRotation = couleur == Couleur.Blanc ?  rot * Quaternion.Euler(180f, 0f, 0f) : rot * Quaternion.Euler(0f, 0f, 0f);
            FlipAnim = true;
        }

        public void EndSpawnAnim()
        {
            SpawnAnim = false;
            SpawnAnimT = 0f;
            transform.localPosition = pos;
            transform.localRotation = couleur == Couleur.Noir ? rot * Quaternion.Euler(180f, 0f, 0f) : rot * Quaternion.Euler(0f, 0f, 0f);
        }

        public void EndFlipAnim()
        {
            FlipAnim = false;
            FlipAnimT = 0f;
            
        }

        new void Update()
        {
            if (SpawnAnim)
            {
                SpawnAnimT += Time.deltaTime * 1f;
                transform.localPosition = Vector3.Lerp(pos + (rot * new Vector3(0f, 1f, 0f)), pos, SpawnAnimT);
                 transform.localRotation = couleur == Couleur.Noir ? rot * Quaternion.Euler(180f, 0f, 0f) : rot * Quaternion.Euler(0f, 0f, 0f);
                if (SpawnAnimT > 1f) EndSpawnAnim();
            }
            else if (FlipAnim)
            {
                FlipAnimT += Time.deltaTime * 0.8f;
                transform.localRotation = couleur == Couleur.Blanc ?
                                            (
                                                FlipAnimT < 0.5f ?
                                                    Quaternion.Lerp(rot * Quaternion.Euler(180f, 0f, 0f), rot * Quaternion.Euler(90f, 0f, 0f), FlipAnimT * 2f)
                                                    : Quaternion.Lerp(rot * Quaternion.Euler(90f, 0f, 0f), rot * Quaternion.Euler(0f, 0f, 0f), FlipAnimT * 2f - 1f)
                                            )
                                            :
                                            (
                                                FlipAnimT < 0.5f ?
                                                    Quaternion.Lerp(rot * Quaternion.Euler(0f, 0f, 0f), rot * Quaternion.Euler(90f, 0f, 0f), FlipAnimT * 2f)
                                                    : Quaternion.Lerp(rot * Quaternion.Euler(90f, 0f, 0f), rot * Quaternion.Euler(180f, 0f, 0f), FlipAnimT * 2f - 1f)
                                            );
                transform.localPosition = FlipAnimT < 0.5f ?
                                                Vector3.Lerp(pos, pos +( rot * new Vector3(0f, 1.5f, 0f)), 1f - Mathf.Pow(1f - (FlipAnimT * 2f), 2))
                                                : Vector3.Lerp(pos + (rot * new Vector3(0f, 1.5f, 0f)), pos, Mathf.Pow(2f*FlipAnimT - 1f, 2))
                                            ;
                if (FlipAnimT > 1f) EndFlipAnim();
            }
            else
            {
                transform.localPosition = pos;
                transform.localRotation = couleur == Couleur.Noir ? rot * Quaternion.Euler(180f, 0f, 0f) : rot * Quaternion.Euler(0f, 0f, 0f);
            }
        }
    }
}