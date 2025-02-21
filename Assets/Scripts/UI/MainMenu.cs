using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class playButton : MonoBehaviour
{
    public void PlayOthello()
    {
        Debug.Log("yay! c'est la scène du jeu Classique");
        SceneManager.LoadSceneAsync("Othello");
    }
    
    public void PlayTorthello()
    {
        Debug.Log("yay! c'est la scène du jeu TOR");
        SceneManager.LoadSceneAsync("Torthello");
    }
}
