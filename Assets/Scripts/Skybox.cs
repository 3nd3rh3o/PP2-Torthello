using UnityEngine;
[ExecuteInEditMode]
public class Skybox : MonoBehaviour
{
    public Material skyboxMaterial;
    

    // Update is called once per frame
    void Update()
    {
        skyboxMaterial?.SetVector("_LightDir", transform.forward);
    }
}
