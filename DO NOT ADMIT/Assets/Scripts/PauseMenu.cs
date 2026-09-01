using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject pausePanel;

    [Header("Player")]
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerLook playerLook;
    [SerializeField] private PlayerFlashlight playerFlashlight;

    [Header("Settings")]
    [SerializeField] private Slider sensitivitySlider;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private Toggle fullscreenToggle;

    [Header("Scene")]
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    private bool paused;

    private void Start()
    {
        paused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        Time.timeScale = 1f;

        if (volumeSlider != null)
        {
            volumeSlider.value =
                AudioListener.volume;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn =
                Screen.fullScreen;
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (paused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        if (paused)
            return;

        paused = true;

        if (pausePanel != null)
            pausePanel.SetActive(true);

        if (playerMovement != null)
            playerMovement.enabled = false;

        if (playerLook != null)
            playerLook.enabled = false;

        if (playerFlashlight != null)
            playerFlashlight.enabled = false;

        Cursor.lockState =
            CursorLockMode.None;

        Cursor.visible = true;

        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        if (!paused)
            return;

        paused = false;

        Time.timeScale = 1f;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (playerLook != null)
            playerLook.enabled = true;

        if (playerFlashlight != null)
            playerFlashlight.enabled = true;

        Cursor.lockState =
            CursorLockMode.Locked;

        Cursor.visible = false;
    }

    public void SetVolume(float value)
    {
        AudioListener.volume =
            Mathf.Clamp01(value);
    }

    public void SetSensitivity(float value)
    {
        if (playerLook == null)
            return;

        playerLook.SetSensitivity(value);
    }

    public void SetFullscreen(bool fullscreen)
    {
        Screen.fullScreen =
            fullscreen;
    }

    public void ReturnToMainMenu()
    {
        Time.timeScale = 1f;

        SceneManager.LoadScene(
            mainMenuSceneName
        );
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;

        Debug.Log("Quitting game...");

        Application.Quit();
    }
}