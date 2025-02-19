using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState { MainMenu, Playing, Paused, Death }

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance { get; private set; }

    [Header("State Configuration")]
    [SerializeField] private GameState _startingState = GameState.MainMenu;

    [Header("UI References")]
    [SerializeField] private GameObject _pauseMenuObject;
    [SerializeField] private GameObject _deathScreenObject;
    [SerializeField] private GameObject _gameplayUIObject;
    [SerializeField] private TheFirstPerson.FPSController fpsCOntroller;

    [Header("Scene Names")]
    [SerializeField] private string _mainMenuSceneName = "MainMenu";

    private GameState _currentState;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
      //  DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        SetState(_startingState);
    }

    public GameState CurrentState => _currentState;

    public void SetState(GameState newState)
    {
        if (_currentState == newState) return;

        ExitState(_currentState);
        _currentState = newState;
        EnterState(newState);
    }

    private void EnterState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SceneManager.LoadSceneAsync(_mainMenuSceneName);
            
                Time.timeScale = 1f;
                break;

            case GameState.Playing:
                fpsCOntroller.enabled = true;
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                SetUIState(_gameplayUIObject, true);
                TimeManager.Instance.UnfreezeGame();
                break;

            case GameState.Paused:
                fpsCOntroller.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
    
                SetUIState(_pauseMenuObject, true); // Keep gameplay UI visible if needed
                TimeManager.Instance.FreezeGame();
                break;

            case GameState.Death:
                fpsCOntroller.enabled = false;
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                SetUIState(_deathScreenObject, true);
                TimeManager.Instance.FreezeGame();
                break;
        }
    }

    private void ExitState(GameState state)
    {
        switch (state)
        {
            case GameState.MainMenu:
              
                break;

            case GameState.Playing:
                SetUIState(_gameplayUIObject, false);
                break;

            case GameState.Paused:
                SetUIState(_pauseMenuObject, false); // Keep gameplay UI visible if needed
                // Add any pause-specific cleanup here
                break;

            case GameState.Death:
                TimeManager.Instance.FreezeGame();
                SetUIState(_deathScreenObject, false);
                break;
        }
    }

    private void SetUIState(GameObject uiElement, bool state)
    {
        if (uiElement != null)
        {
            uiElement.SetActive(state);
        }
    }

    // Public methods for UI buttons and other systems to call
    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void TogglePause()
    {
        if (_currentState!=GameState.Death)
        SetState(_currentState == GameState.Paused ? GameState.Playing : GameState.Paused);
    }

    public void ReturnToMainMenu()
    {
        SetState(GameState.MainMenu);
    }

    public void HandleDeath()
    {
        SetState(GameState.Death);
    }

    public void ReloadGame()
    {
        SceneManager.LoadSceneAsync(SceneManager.GetActiveScene().name);
        SetState(GameState.Playing);
    }
}