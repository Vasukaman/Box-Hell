using System.Collections;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    #region Singleton Setup
    public static TimeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // Initialize time scale
            timeScale = 1f;

        }
    }
    #endregion

    [SerializeField] private float defaultTimeScale = 1f;
    private float timeScale;
    private bool isTimePaused = false;
    private Coroutine currentFreezeCoroutine;

    
    public float TimeScale
    {
        get { return timeScale; }
        set { timeScale = value; UpdateTimeScale(); }
    }

    private void UpdateTimeScale()
    {
        if (!isTimePaused)
        {
            Time.timeScale = timeScale;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    public void FreezeFrame(float duration)
    {
        if (currentFreezeCoroutine != null)
        {
            StopCoroutine(currentFreezeCoroutine);
        }
        currentFreezeCoroutine = StartCoroutine(SlowMotion(duration));
    }

    private IEnumerator SlowMotion(float duration)
    {
        float originalScale = timeScale;
        timeScale = 0f; // Adjust this value for slower or faster effect
        UpdateTimeScale();
        yield return new WaitForSecondsRealtime(duration);
        timeScale = originalScale;
        UpdateTimeScale();
    }

    public void FreezeGame()
    {
        isTimePaused = true;
        Time.timeScale = 0f;
        Time.fixedDeltaTime = 0f;
    }

    public void UnfreezeGame()
    {
        isTimePaused = false;
        Time.timeScale = defaultTimeScale;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }
}