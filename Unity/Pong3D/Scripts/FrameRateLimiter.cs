using UnityEngine;

public class FrameRateLimiter : MonoBehaviour
{
     public int targetFrameRate = 240; 
    void Awake()
    {
        Application.targetFrameRate = targetFrameRate;

        QualitySettings.vSyncCount = 0;

        Debug.Log($"Target Frame Rate set to: {Application.targetFrameRate}, VSync Count: {QualitySettings.vSyncCount}");
    }
}
