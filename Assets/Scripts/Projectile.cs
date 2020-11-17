using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Projectile : MonoBehaviour
{
    public int damage = 50;
    public string name;
    public float speed = 4f;
    Rigidbody2D rigidbody;
    Transform target;
    // Start is called before the first frame update
    void Start()
    {
        if (transform.rotation.eulerAngles.y != 0) speed *= -1;
        rigidbody = GetComponent<Rigidbody2D>();
        rigidbody.velocity = new Vector2(speed, 0f);
        if(SceneManager.GetActiveScene().name == "PVP")
        {
            damage = 5;
            if(name == "UltProjectile")
            {
                damage = 25;

            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 3f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (collision.gameObject.GetComponent<Enemy>())
        {
            collision.gameObject.GetComponent<Enemy>().TakeDamage(damage, transform.rotation.eulerAngles.y != 180);
            if (name == "UltProjectile") Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<Boss>())
        {
            collision.gameObject.GetComponent<Boss>().anim.Play("boss_block");
            collision.gameObject.GetComponent<Boss>().TakeDamage(0, transform.rotation.eulerAngles.y != 180);
            if (name == "UltProjectile") Destroy(gameObject);
        }
        else if (collision.gameObject.GetComponent<PVP>())
        {
                collision.gameObject.GetComponent<PVP>().TakeDamage(damage, transform.rotation.eulerAngles.y != 180);
                if (name == "UltProjectile") Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
        
    }
}
