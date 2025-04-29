using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ButtonManager : MonoBehaviour
{
    static public ButtonManager Instance;

    [SerializeField]
    public GameObject _esc;

    [SerializeField]
    private GameObject _manual;

    [SerializeField]
    private TextMeshProUGUI _text;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        if (_text != null) _text.text = $"stage : {PlayerPrefs.GetInt("SpawnPoint")}";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_manual != null && _manual.activeSelf)
            {
                _manual.SetActive(false);
            }
            else
            {
                GameManager.Instance._pc.OnSlow(false);
                Time.timeScale = _esc.activeSelf ? 1 : 0;
                _esc.SetActive(!_esc.activeSelf);
            }
        }
    }

    public void ResetAndStart()
    {
        Time.timeScale = 1;
        PlayerPrefs.SetInt("SpawnPoint", 0);
        SceneManager.LoadScene("InGameScene");
    }

    public void GameStart()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("InGameScene");
    }

    public void OpneManual()
    {
        _manual.SetActive(true);
    }

    public void GoBack()
    {
        _esc.SetActive(false);
    }

    public void GameExit()
    {
        Application.Quit();
    }

    public void Title()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("TitleScene");
    }
}
