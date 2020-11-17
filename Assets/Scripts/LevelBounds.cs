using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelBounds : MonoBehaviour
{
    public List<GameObject> players;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("PVP"))
        {
            collision.gameObject.GetComponent<PVP>().TakeDamage(1000, false);
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //check other player exists
            collision.gameObject.GetComponent<PlayerController>().TakeDamage(1000, false);
            if (!players[0].activeSelf && !players[1].activeSelf)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
        }
        else if(collision.gameObject.GetComponent<Boss>())
        {
            collision.gameObject.GetComponent<Boss>().TakeDamage(10000, false);
        }
        else
        {
            Destroy(collision.gameObject);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject player in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (player.tag == "Player") players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
