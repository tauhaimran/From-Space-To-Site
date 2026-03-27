using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // Attach this to your button's OnClick() event
    public void ChangeScene(string sceneName)
    {
        Debug.Log("Switching to scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }
}