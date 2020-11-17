using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PVPController : MonoBehaviour
{
    public static PVPController instance;
    public PVP blue;
    public PVP red;
    public Text blueScore;
    public Text redScore;
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
        if (blue) blue.isPaused = true;
        if (red) red.isPaused = true;
        if (blue)
        {
            blueScore.text = "Blue Kills: " + blue.kills;
            redScore.text = "Red Kills: " + (3 - blue.lives);
            score = blue.kills;
        }
        else
        {
            redScore.text = "Red Kills: " + red.kills;
            blueScore.text = "Blue Kills: " + (3 - red.lives);
            score = red.kills;
        }
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
