using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/**
* Le gameManager s'occupe de la gestion du changement de scène
* Permet de faire des feature différente par panel (exemple géré des annimations spécifique)
**/
public class GameManager : MonoBehaviour
{
    // On déclare un singleton qui sera donc accessible de partout
    // le get qu'on voit en dessous signifie que tout le monde peut le get
    public static GameManager instance { private set; get; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public void ChangeScene(string _sceneName)
    {
        // charge la scène passé en param
        SceneManager.LoadScene(_sceneName);
    }

    public void Quit()
    {
        // quitter le jeux
        Application.Quit();
    }
}