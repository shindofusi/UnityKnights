using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    // Outlets
    //private Image imageHealthBar;
    public Animator anim;
    public AnimationClip death;
    Rigidbody2D rigidbody;
    Transform target;
    public GameObject DamageTextPrefab;
    private Vector3 Offset = new Vector3(.7f, .1f, 0);
    private float time;
    private float attackTime;

    // Stats
    public float speed;
    public float health = 100f;
    public float healthMax = 100f;
    private int direction = 1;
    private bool falling = false;

    // State Tracking
    private bool isSwingingSword = false;
    private float distance;
    public List<PlayerController> players;
    public int spawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        attackTime = Time.time;
        time = Time.time;
        rigidbody = GetComponent<Rigidbody2D>();
        //imageHealthBar = gameObject.Find("Image").GetComponent<Image>();

        // Get all players
        foreach (PlayerController player in GameObject.FindObjectsOfType(typeof(PlayerController)))
        {
            if (player.tag == "Player") players.Add(player);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (health > 0) //&& time + .2f < Time.time)
        {
            //time = Time.time;
            // Do not allow damage collision
            if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("enemyBasicAttack"))
            {
                isSwingingSword = false;
            }

            // Need to find closest player
            ChooseNearestTarget();
            if (distance < 1f) // Attack
            {
                
                anim.SetBool("attackRange", true);
                anim.SetBool("detectRange", false);
                //if (target.position.x > transform.position.x) { transform.eulerAngles = new Vector3(0, 0, 0); direction = 1; }
                //else { transform.eulerAngles = new Vector3(0, 180, 0); direction = -1; }
                //anim.Play("enemyBasicAttack");

            }
            else if (distance < 10f && TouchingGround()) // Walk Toward
            {
                anim.SetBool("detectRange", true);
                anim.SetBool("attackRange", false);
                if (target.position.x > transform.position.x) // Move right
                {
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    rigidbody.velocity = Vector2.right * speed;
                    direction = 1;
                }
                else // Move left
                {
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    rigidbody.velocity = Vector2.left * speed;
                    direction = -1;

                }
            }
            else if(distance < 10f)
            {
                anim.SetBool("detectRange", false);
                anim.SetBool("attackRange", false);
            }
            else if(!falling)// Idle, maybe change to patrol
            {
                anim.SetBool("detectRange", true);
                anim.SetBool("attackRange", false);
                if (!TouchingGround())
                {
                    if (direction == 1)
                    {
                        transform.eulerAngles = new Vector3(0, 180, 0);
                        rigidbody.velocity = Vector2.left * speed;
                        direction = -1;
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 0, 0);
                        rigidbody.velocity = Vector2.right * speed;
                        direction = 1;
                    }
                }
                else
                {
                    if (direction == 1) rigidbody.velocity = Vector2.right * speed;
                    else rigidbody.velocity = Vector2.left * speed;
                }
            }
            
           
        
        }
    }
    bool TouchingGround()
    {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + direction * Offset, -transform.up, 2f);
            Debug.DrawRay(transform.position + direction * Offset, -transform.up * 2f);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    return true;
                }
            }
        return false;
    }
    // Loop through all players pick closest one
    // Maybe do closest path meshgrid if we can figure it out later
    void ChooseNearestTarget() 
    {
        distance = 9999f;
        for (int i=0; i<players.Count; i++)
        {
            if (players[i] != null)
            {
                PlayerController player = players[i];
                if (player.isActiveAndEnabled)
                {
                    Vector2 directionToTarget = player.transform.position - transform.position;

                    // Closest distance and that we don't have awkard y
                    if (directionToTarget.sqrMagnitude < distance && Mathf.Abs(player.transform.position.y - transform.position.y) < 1f)
                    {
                        // Update closest distance for future comparisons
                        distance = directionToTarget.sqrMagnitude;
                        // Store reference to closest target we've seen so far
                        target = player.transform;
                    }
                }
            }
        }
    }

    // Animate, fadeaway, and destroy
    IEnumerator Die()
    {
        //Consider coroutine for anim wait end and fade out
        anim.Play(death.name);
        GameController.instance.DeSpawn(spawnPoint);
        Destroy(gameObject, death.length-.2f);
        yield return new WaitForSeconds(death.length-.2f);
        //yield return new WaitForSeconds(0);
        
        
    }

    // Healthbar and Floating text
    public int TakeDamage(float damageAmount, bool dir)
    {
        // Trigger floating text
        health -= damageAmount;
        if (dir)
        {
            rigidbody.AddForce((Vector2.right + Vector2.up), ForceMode2D.Impulse);

        }
        else
        {
            rigidbody.AddForce((Vector2.left + Vector2.up), ForceMode2D.Impulse);
        }
        // Show floating text
        //Debug.Log("Enemy took damage");
        if (DamageTextPrefab) ShowFloatingText((int)damageAmount);
        if (health <= 0)
        {
            StartCoroutine(Die());
            return 15;
        }
        // Call function to lower health
        //imageHealthBar.fillAmount = health / healthMax;
        
        return 0;
    }
    void ShowFloatingText(int damageAmount)
    {
        var go = Instantiate(DamageTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = damageAmount.ToString();
        if (damageAmount < 13)
        {
            go.GetComponent<TextMesh>().color = Color.white;
        }
        else
        {
            go.GetComponent<TextMesh>().color = Color.black;
        }

    }


    // Is hitting
    public void AnimationEvent(string s)
    {
        if (s == "hit")
        {
            isSwingingSword = true;
            // Damage enemy
        }
        if (s == "Off")
        {
            isSwingingSword = false;
        }
    }
    public void Turn()
    {
        if (target.position.x > transform.position.x) { transform.eulerAngles = new Vector3(0, 0, 0); direction = 1; }
        else { transform.eulerAngles = new Vector3(0, 180, 0); direction = -1; }
    }
    // Check for player damage
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Damage
        if (isSwingingSword && collision is CapsuleCollider2D && collision.gameObject.layer == LayerMask.NameToLayer("Player") && collision.gameObject.GetComponent<PlayerController>().health > 0)
        {
            // Lower enemy hp
            if(direction == 1)  collision.gameObject.GetComponent<PlayerController>().TakeDamage(10f, true);
            else collision.gameObject.GetComponent<PlayerController>().TakeDamage(10f, false);
            isSwingingSword = false;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            falling = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            falling = false;
        }
    }
}
