using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransfer : MonoBehaviour
{
    public Text score;
    // Start is called before the first frame update
    void Start()
    {
        Hide();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    // Update is called once per frame
    void Update()
    {
    }
    public void OnMouseUp(string scene)
    {
        SceneManager.LoadScene(scene);
    }
}
