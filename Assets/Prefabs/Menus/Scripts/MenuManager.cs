using UnityEngine;

public class MenuManager : MonoBehaviour
{
    #region Variables
    public GameObject menuPrincipal;
    public GameObject menuOpciones;
    public GameObject menuInstrucciones;
    public GameObject menuCreditos;
    #endregion

    #region Botones Menu
    public void Jugar()
    {
        GameManager.Instance.LoadScene("Nivel");
    }

    public void Opciones()
    {
        GameManager.Instance.LoadMenu(menuOpciones, menuPrincipal);
    }

    public void Instrucciones()
    {
        GameManager.Instance.LoadMenu(menuInstrucciones, menuPrincipal);
    }

    public void Creditos()
    {
        GameManager.Instance.LoadMenu(menuCreditos, menuPrincipal);
    }

    public void Salir()
    {
        // Esto cierra el .exe final
        Application.Quit();

        // Esto solo se ejecuta adentro del editor de Unity para no romper la compilación
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    #endregion

    public void Regresar()
    {
        GameManager.Instance.LoadMenu(menuPrincipal, menuOpciones);
        GameManager.Instance.LoadMenu(menuPrincipal, menuInstrucciones);
        GameManager.Instance.LoadMenu(menuPrincipal, menuCreditos);
    }
}