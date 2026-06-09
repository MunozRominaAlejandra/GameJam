using UnityEngine;

public class PauseManager : MonoBehaviour
{
    #region Variables
    public GameObject pauseMenu;
    public GameObject options;
    public bool isPaused = false;
    #endregion
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                Reanudar();
            }
            else
            {
                Pausar();
            }
        }
    }

    public void Pausar()
    {
        pauseMenu.SetActive(true);
        isPaused = true;
        Time.timeScale = 0f;
    }

    public void Reanudar()
    {
        pauseMenu.SetActive(false);
        isPaused = false;
        Time.timeScale = 1f;
    }

    public void Menu()
    {
        Time.timeScale = 1f;
        GameManager.Instance.LoadScene("Menus");
    }
    public void Opciones()
    {
        GameManager.Instance.LoadMenu(pauseMenu, options);
    }
    public void Regresar()
    {
        GameManager.Instance.LoadMenu(options, pauseMenu);
    }
}
