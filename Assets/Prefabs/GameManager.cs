using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public int score;
    public int cows;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddCow()
    {
        cows++;
        score += 50;
    }

    public void RemoveCow()
    {
        if (cows > 0)
        {
            cows--;
            score -= 50;

            if (score < 0)
                score = 0;
        }

        if (cows <= 0)
        {
            GameOver();
        }
    }

    public void GameOver()
    {
        score = 0;
        cows = 0;

        Time.timeScale = 1f;
        SceneManager.LoadScene("Menus");
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenu(GameObject activeMenu, GameObject desactiveMenu)
    {
        activeMenu.SetActive(true);
        desactiveMenu.SetActive(false);
    }
}