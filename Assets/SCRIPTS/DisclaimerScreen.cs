using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisclaimerScreen : MonoBehaviour
{
    public CanvasGroup warningCanvasGroup;
    public AudioSource warningAudio;
    public float displayTime = 8f; // How long the warning stays on screen
    public string nextSceneName = "Scene1_LowOrbit"; // Put your space scene name here

    void Start()
    {
        // Start black, fade in, wait, fade out, load game.
        warningCanvasGroup.alpha = 0;
        StartCoroutine(PlayDisclaimer());
    }

    IEnumerator PlayDisclaimer()
    {
        // 1. Fade in the text
        yield return StartCoroutine(Fade(0, 1, 2f));
        
        // 2. Play the voiceover (if you have one)
        if (warningAudio != null) warningAudio.Play();

        // 3. Wait for the user to read it
        yield return new WaitForSeconds(displayTime);

        // 4. Fade to black
        yield return StartCoroutine(Fade(1, 0, 2f));
        yield return new WaitForSeconds(1f);

        // 5. Load the Space Scene
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            warningCanvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        warningCanvasGroup.alpha = endAlpha;
    }
}