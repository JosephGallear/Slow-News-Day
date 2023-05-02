using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenu;

    private bool paused;

    // Start is called before the first frame update
    void Start()
    {
        paused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused || Manager.Instance.canPause)
            {
                TogglePause(!paused);
            }
        }
    }

    public void TogglePause(bool pause)
    {
        paused = pause;
        pauseMenu.SetActive(pause);
        Time.timeScale = pause ? 0 : 1;

        if (pause)
        {
            MusicPlayer.Instance.StopMusic();
        }
        else
        {
            MusicPlayer.Instance.PlayMusic();
        }
    }

    public void RetryGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReturnToMenu()
    {
        MusicPlayer.Instance.PlayMusic();
        SceneManager.LoadScene(0);
    }
}
