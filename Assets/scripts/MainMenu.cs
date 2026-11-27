using UnityEngine;
using UnityEngine.SceneManagement;

namespace KookKoo.UI
{
    /// <summary>
    /// Main Menu -> Tutorial (both panels in the MainMenu scene).
    /// Tutorial -> Start Game loads the actual gameplay scene.
    /// </summary>
    public class MainMenuUI : MonoBehaviour
    {
        [Header("UI panels in the MainMenu scene")]
        [SerializeField] private GameObject mainMenuPanel;
        [SerializeField] private GameObject tutorialPanel;

        [Header("Gameplay scene name (add it to the Build Profiles scene list)")]
        [SerializeField] private string gameSceneName = "SampleScene";

        private void Start()
        {
            Time.timeScale = 1f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            ShowMainMenu();
        }

        public void Play()
        {
            if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
            if (tutorialPanel != null) tutorialPanel.SetActive(true);
            else Debug.LogError("MainMenuUI: Tutorial Panel was not assigned!", this);
        }

        public void ShowMainMenu()
        {
            if (tutorialPanel != null) tutorialPanel.SetActive(false);
            if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        }

        public void StartGame()
        {
            if (string.IsNullOrWhiteSpace(gameSceneName))
            {
                Debug.LogError("MainMenuUI: Set Game Scene Name in Inspector.", this);
                return;
            }
            Time.timeScale = 1f;
            SceneManager.LoadScene(gameSceneName);
        }

        public void QuitGame()
        {
            Application.Quit();
#if UNITY_EDITOR
            Debug.Log("QuitGame called. Application.Quit has no effect in Unity Editor.");
#endif
        }
    }
}