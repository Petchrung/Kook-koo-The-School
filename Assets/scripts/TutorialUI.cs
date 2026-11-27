using UnityEngine;
using UnityEngine.SceneManagement;

public class TutorialUI : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject mainMenuPanel;
    [SerializeField] private GameObject tutorialPanel;

    [Header("Gameplay Scene")]
    [SerializeField] private string gameplaySceneName = "SampleScene";

    // กด START GAME เพื่อเข้าสู่ Gameplay
    public void StartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameplaySceneName);
    }

    // กด BACK เพื่อกลับหน้า Main Menu
    public void Back()
    {
        tutorialPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
}