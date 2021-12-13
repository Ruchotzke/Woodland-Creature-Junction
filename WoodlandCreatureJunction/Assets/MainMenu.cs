using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] GameObject StartPanel;
    [SerializeField] GameObject OptionsPanel;

    #region CALLBACKS
    public void OnPythonPathSubmission(string path)
    {
        Debug.Log("Setting python path to: " + path);
        GenText.PYTHON_PATH = path;
    }

    public void OnOptions()
    {
        StartPanel.SetActive(false);
        OptionsPanel.SetActive(true);
    }

    public void OnBack()
    {
        StartPanel.SetActive(true);
        OptionsPanel.SetActive(false);
    }

    public void OnStart()
    {
        SceneManager.LoadScene("ProceduralTerrain", LoadSceneMode.Single);
    }
    #endregion
}
