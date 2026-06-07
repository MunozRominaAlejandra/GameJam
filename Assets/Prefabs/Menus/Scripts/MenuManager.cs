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
        GameManager.Instance.LoadScene("Nivel1");
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
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
    #endregion
    public void Regresar()
    {
        GameManager.Instance.LoadMenu(menuPrincipal, menuOpciones);
        GameManager.Instance.LoadMenu(menuPrincipal, menuInstrucciones);
        GameManager.Instance.LoadMenu(menuPrincipal, menuCreditos);
    }
}