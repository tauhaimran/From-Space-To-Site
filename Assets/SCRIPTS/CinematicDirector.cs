using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CinematicDirector : MonoBehaviour
{[Header("Player & Movement")]
    public Transform xrRig; 
    public Transform startPoint;
    public Transform satelliteViewPoint;
    public Transform earthZoomPoint;
    public float movementSpeed = 2f;[Header("UI Elements")]
    public CanvasGroup fadeScreen; 
    public CanvasGroup textPanel1; 
    public CanvasGroup textPanel2; 

    [Header("Voiceover Audio")]
    public AudioSource audioSource; // The AudioSource playing the Voiceovers
    public AudioClip introVoiceover;
    public AudioClip zimbabweVoiceover;

    [Header("Background Audio Ducking")][Tooltip("Drag your background music or space hum AudioSources here.")]
    public AudioSource[] bgSFXSources; 
    public float normalVolume = 1.0f;
    public float duckedVolume = 0.2f; // How quiet it gets during dialogue
    public float audioFadeDuration = 0.5f; // How fast it fades up/down[Header("Scene Management")]
    public string nextSceneName = "Scene2_OnSite"; 

    void Start()
    {
        // Make sure all BG audio starts at normal volume
        SetBGAudioVolume(normalVolume);
        StartCoroutine(PlayCinematicSequence());
    }

    IEnumerator PlayCinematicSequence()
    {
        // 1. SETUP
        fadeScreen.alpha = 1;
        textPanel1.alpha = 0;
        textPanel2.alpha = 0;
        xrRig.position = startPoint.position;
        xrRig.rotation = startPoint.rotation;

        // 2. FADE IN FROM BLACK
        yield return StartCoroutine(FadeCanvas(fadeScreen, 1, 0, 2f));
        yield return new WaitForSeconds(1f); 

        // 3. SHOW TEXT 1 & PLAY AUDIO 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 0, 1, 1.5f));
        
        // Lower BG volume smoothly, then play Voiceover
        StartCoroutine(FadeBGAudio(duckedVolume)); 
        audioSource.clip = introVoiceover;
        audioSource.Play();

        // 4. WAIT FOR AUDIO TO FINISH
        yield return new WaitForSeconds(introVoiceover.length);
        
        // Bring BG volume back up smoothly
        StartCoroutine(FadeBGAudio(normalVolume)); 
        yield return new WaitForSeconds(0.5f);

        // 5. FADE OUT TEXT 1
        yield return StartCoroutine(FadeCanvas(textPanel1, 1, 0, 1f));

        // 6. MOVE TO MAIN SATELLITE 
        yield return StartCoroutine(MovePlayer(satelliteViewPoint));

        // 7. SHOW TEXT 2 & PLAY AUDIO 2 
        yield return StartCoroutine(FadeCanvas(textPanel2, 0, 1, 1.5f));
        
        // Lower BG volume again for the second Voiceover
        StartCoroutine(FadeBGAudio(duckedVolume));
        audioSource.clip = zimbabweVoiceover;
        audioSource.Play();
        
        yield return new WaitForSeconds(zimbabweVoiceover.length);
        
        // Bring BG volume back up smoothly
        StartCoroutine(FadeBGAudio(normalVolume));
        yield return new WaitForSeconds(0.5f);
        
        yield return StartCoroutine(FadeCanvas(textPanel2, 1, 0, 1f));

        // 8. SLOWLY TURN TO FACE ZIMBABWE/EARTH
        yield return StartCoroutine(RotatePlayer(earthZoomPoint.rotation, 2f));

        // 9. ZOOM INTO EARTH
        yield return StartCoroutine(MovePlayer(earthZoomPoint));

        // 10. FADE TO BLACK
        yield return StartCoroutine(FadeCanvas(fadeScreen, 0, 1, 2f));
        yield return new WaitForSeconds(1f);

        // 11. LOAD MILESTONE 2
        SceneManager.LoadScene(nextSceneName);
    }

    // --- Helper Functions Below ---

    // Smoothly fades all background audio sources up or down
    IEnumerator FadeBGAudio(float targetVolume)
    {
        if (bgSFXSources.Length == 0) yield break;

        // Store the starting volume of each source so we can lerp from it
        float[] startVolumes = new float[bgSFXSources.Length];
        for (int i = 0; i < bgSFXSources.Length; i++)
        {
            if (bgSFXSources[i] != null) startVolumes[i] = bgSFXSources[i].volume;
        }

        float elapsed = 0f;
        while (elapsed < audioFadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / audioFadeDuration);

            for (int i = 0; i < bgSFXSources.Length; i++)
            {
                if (bgSFXSources[i] != null)
                {
                    bgSFXSources[i].volume = Mathf.Lerp(startVolumes[i], targetVolume, t);
                }
            }
            yield return null;
        }

        // Ensure we hit the exact target volume at the end
        SetBGAudioVolume(targetVolume);
    }

    // Instantly sets volume for all BG sources (used for setup and safety checks)
    void SetBGAudioVolume(float volume)
    {
        foreach (AudioSource source in bgSFXSources)
        {
            if (source != null) source.volume = volume;
        }
    }

    // Fades UI panels smoothly
    IEnumerator FadeCanvas(CanvasGroup cg, float startAlpha, float endAlpha, float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            cg.alpha = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        cg.alpha = endAlpha;
    }

    // Moves the player to a target position without snapping at the end
    IEnumerator MovePlayer(Transform target)
    {
        Vector3 startPos = xrRig.position;
        Quaternion startRot = xrRig.rotation;
        
        float distance = Vector3.Distance(startPos, target.position);
        if (distance <= 0.01f) yield break; // Safety check

        float timeToMove = distance / movementSpeed;
        float elapsed = 0;

        while (elapsed < timeToMove)
        {
            elapsed += Time.deltaTime;
            
            float t = Mathf.Clamp01(elapsed / timeToMove); 
            float smoothStep = t * t * (3f - 2f * t); 
            
            xrRig.position = Vector3.Lerp(startPos, target.position, smoothStep);
            xrRig.rotation = Quaternion.Slerp(startRot, target.rotation, smoothStep);
            
            yield return null;
        }
        
        xrRig.position = target.position;
    }

    // Slowly rotates the player to face a new direction
    IEnumerator RotatePlayer(Quaternion targetRotation, float duration)
    {
        Quaternion startRot = xrRig.rotation;
        float elapsed = 0;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float smoothStep = t * t * (3f - 2f * t);

            xrRig.rotation = Quaternion.Slerp(startRot, targetRotation, smoothStep);
            yield return null;
        }
        xrRig.rotation = targetRotation; 
    }
}