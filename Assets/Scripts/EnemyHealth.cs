using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    Vector3 localScale;
    private float health;
    // Start is called before the first frame update
    void Start()
    {
        localScale = transform.localScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.parent.gameObject.GetComponent<Enemy>())
        {
            health = transform.parent.gameObject.GetComponent<Enemy>().health;
            if (health <= 0) localScale.x = 0;
            else localScale.x = health / 10f;
            transform.localScale = localScale;
        }
        else
        {
            health = transform.parent.gameObject.GetComponent<Boss>().health;
            if (health <= 0) localScale.x = 0;
            else localScale.x = health / 1000f;
            transform.localScale = localScale;
        }
    }
}
