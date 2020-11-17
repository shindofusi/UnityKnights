using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public static MenuController instance;
    public PlayerController p1;
    public PlayerController p2;
    public Text curScore;
    public Text highScore;
    public int score;

    private void Awake()
    {
        instance = this;
        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }
    public void Show()
    {
        gameObject.SetActive(true);
        Time.timeScale = 0;
        if (p1) p1.isPaused = true;
        if (p2) p2.isPaused = true;
        if (p1) {
            curScore.text = "Your Score: " + p1.money;
            score = p1.money;
        }
        else {
            curScore.text = "Your Score: " + p2.money;
            score = p2.money;
        }

        if (score > PlayerPrefs.GetInt("Score"))
        {
            PlayerPrefs.SetInt("Score", score);
        }
        highScore.text = "High Score: " + PlayerPrefs.GetInt("Score");
    }
    public void OnMouseUp(string scene)
    {
        SceneManager.LoadScene(scene);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
