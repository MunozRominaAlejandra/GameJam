using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public int score;
    public int level;

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
    #region Configuración Menu
    public void LoadScene(string GameScene)
    {
        SceneManager.LoadScene(GameScene);
    }
    public void LoadMenu(GameObject activeMenu, GameObject desactiveMenu)
    {
        activeMenu.SetActive(true);
        desactiveMenu.SetActive(false);
    }
    #endregion
}
