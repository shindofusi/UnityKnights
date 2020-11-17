using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public abstract class Player : MonoBehaviour
{
    // Other Player
    public int otherPlayerNum;

    // Outlet
    public Image imageHealthBar;
    public Image imageManaBar;
    public Text gold;
    public GameObject basicAttackProjectile;
    public GameObject ultAttackProjectile;
    public GameObject basicDefenseProjectile;

    // Stats
    public int money = 0;
    public float healthMax = 100f;
    public float health = 100f;
    public float mana = 100f;
    public float manaMax = 100f;
    public float speed;
    public float jump;
    public float damage;
    public float damageMultiplier = 0;
    public float blockingAmount = 0;
    public float runningSpeed;
    public float dist = .95f;
    public bool teleport = true;
    public float teleportDistance = .01f;

    // Combo variables
    public float allowedTimeBetweenButtons = 0.2f;
    public float timeLastButtonPressed;
    public int currentIndex = 0;

    // Movements
    public KeyCode left;
    public KeyCode right;
    public KeyCode up;
    public KeyCode down;
    public KeyCode attack;
    public KeyCode block;
    public KeyCode[] basicAttack;
    public KeyCode[] ultAttack;
    public KeyCode[] basicDefense;

    // Aesthetics
    public Animator anim;
    public AnimationClip death;
    public Color color;
    public GameObject DamageTextPrefab;
    public GameObject SmokePrefab;

    // Components
    public List<GameObject> players;
    protected Rigidbody2D rb;
    public float moveHorizontal;

    // Trackers
    public int jumpsLeft = 2;
    public float runLeft = 0;
    public float runRight = 0;
    public bool isSwingingSword = false;
    public bool isDead = false;
    public bool isBlocking = false;
    public bool teleMove = true;
    public bool downToggle = false;
    public bool leftToggle = false;
    public bool upToggle = false;
    public bool rightToggle = false;

    public float basicTime = 0f;
    public float ultTime = 0f;
    public float specialCD = 0.3f;
    public float basicMana = 5f;
    public float ultMana = 20f;

    public float teleTime = 0f;
    public float moveCD = 0.3f;
    public bool isDying = false;


    // Start is called before the first frame update
    public virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(Mana());
        moveHorizontal = Input.GetAxis("Horizontal");
        foreach (GameObject player in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (player.tag == "Player") players.Add(player);
        }

        basicAttack = new KeyCode[] { down, attack };
        basicDefense = new KeyCode[] { left, right, block };
        ultAttack = new KeyCode[] { block, up, down, attack };
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // If not dead
        if (health > 0 && !isDead)
        {

            if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("isAttacking")) isSwingingSword = false; // Reset hit check
            if (!this.anim.GetCurrentAnimatorStateInfo(0).IsName("isBlocking")) isBlocking = false; // Reset hit check

            // Attack Animation
            if (Input.GetKey(attack))
            {
                anim.SetBool("isAttacking", true);

            }
            else if (Input.GetKeyUp(attack)) anim.SetBool("isAttacking", false);

            // Defense Animation
            if (Input.GetKey(block))
            {
                if ((leftToggle || rightToggle) && Time.time - basicTime > specialCD)
                {
                    //basic anim do
                    if (mana >= basicMana)
                    {
                        mana -= basicMana;
                        imageManaBar.fillAmount = mana / manaMax;
                        if (transform.rotation.eulerAngles.y != 0) Instantiate(basicAttackProjectile, transform.position + new Vector3(dist * -1, 0), transform.rotation);
                        else Instantiate(basicAttackProjectile, transform.position + new Vector3(dist, 0), transform.rotation);
                    }
                    basicTime = Time.time;
                }
                if (downToggle && Time.time - ultTime > specialCD)
                {
                    //special anim do
                    if (mana >= ultMana)
                    {
                        mana -= ultMana;
                        imageManaBar.fillAmount = mana / manaMax;
                        if (transform.rotation.eulerAngles.y != 0) Instantiate(ultAttackProjectile, transform.position + new Vector3(dist * -1 - .3f, 0), transform.rotation);
                        else Instantiate(ultAttackProjectile, transform.position + new Vector3(dist + .3f, 0), transform.rotation);
                    }
                    ultTime = Time.time;
                }
                else
                {
                    anim.SetBool("isBlocking", true);
                }

            }
            else if (Input.GetKeyUp(block)) anim.SetBool("isBlocking", false);

            // Jump, with double jump
            if (Input.GetKeyDown(up))
            {
                if (jumpsLeft > 0)
                {
                    rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
                    jumpsLeft--;
                    anim.SetBool("isWalking", true); // This is also jump anim, sorry
                }
            }

            // Down, used for combos, no realistic use by itself
            if (Input.GetKey(down))
            {
                downToggle = true;
                transform.position += new Vector3(0, -speed, 0);

            }
            else if (Input.GetKeyUp(down))
            {
                downToggle = false;
            }

            // Walk left, with double tap running
            if (Input.GetKey(left))
            {
                leftToggle = true;
                // teleport 
                if (teleport && downToggle && Time.time - teleTime > moveCD)
                {
                    ShowSmoke(false);
                    if (transform.eulerAngles.y != 180) transform.position += transform.right * -teleportDistance;
                    else transform.position += transform.right * teleportDistance;
                    teleTime = Time.time;
                    teleport = false;
                }

                // move left
                else if (Time.time - teleTime > moveCD && Time.time - basicTime > moveCD)
                {
                    transform.position += new Vector3(-speed, 0, 0);
                    transform.eulerAngles = new Vector3(0, 180, 0);
                    if (!anim.GetBool("isRunning"))
                    {
                        if (Time.time - runLeft <= 0.2f) // Double tap based on .2f delay, fix anim later
                        {
                            anim.SetBool("isWalking", false);
                            anim.SetBool("isRunning", true);
                            speed += runningSpeed;
                        }
                        else
                        {
                            anim.SetBool("isWalking", true);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(left))
            {
                leftToggle = false;
                if (anim.GetBool("isRunning"))
                {
                    speed -= runningSpeed;
                    anim.SetBool("isRunning", false);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    runLeft = Time.time;
                    teleport = true;
                }
            }

            // Walk right, same as above
            if (Input.GetKey(right))
            {
                rightToggle = true;
                //Teleport
                if (teleport && downToggle && Time.time - teleTime > moveCD)
                {
                    ShowSmoke(true);
                    teleport = false;
                    teleMove = false;
                    if (transform.eulerAngles.y != 0) transform.position += transform.right * -teleportDistance;
                    else transform.position += transform.right * teleportDistance;
                    teleTime = Time.time;
                }
                //Move
                else if (Time.time - teleTime > moveCD && Time.time - basicTime > moveCD)
                {
                    transform.position += new Vector3(speed, 0, 0);
                    transform.eulerAngles = new Vector3(0, 0, 0);
                    if (!anim.GetBool("isRunning"))
                    {
                        if (Time.time - runRight <= 0.2f)
                        {
                            anim.SetBool("isWalking", false);
                            anim.SetBool("isRunning", true);
                            speed += runningSpeed;

                        }
                        else
                        {
                            anim.SetBool("isWalking", true);
                        }
                    }
                }
            }
            else if (Input.GetKeyUp(right))
            {
                rightToggle = false;
                if (anim.GetBool("isRunning"))
                {
                    speed -= runningSpeed;
                    anim.SetBool("isRunning", false);
                }
                else
                {
                    anim.SetBool("isWalking", false);
                    runRight = Time.time;
                    teleport = true;
                }
            }
        }

    }
    public bool CheckCombo(KeyCode[] combo)
    {
        if (Time.time > timeLastButtonPressed + allowedTimeBetweenButtons) currentIndex = 0;
        {
            if (currentIndex < combo.Length)
            {
                if (Input.GetKeyDown(combo[currentIndex]))
                {
                    timeLastButtonPressed = Time.time;
                    currentIndex++;
                }

                if (currentIndex >= combo.Length)
                {
                    currentIndex = 0;
                    return true;
                }
                else return false;
            }
        }

        return false;
    }
    // Mana up
    IEnumerator Mana()
    {
        yield return new WaitForSeconds(2f);
        if (mana <= 95) mana += 5;
        else if (mana < 100) mana = 100;
        imageManaBar.fillAmount = mana / manaMax;
        StartCoroutine(Mana());
    }

    // Fade Out
    // Considerations for other player and what happens to this player for later
    protected abstract IEnumerator Die();

    public void TakeDamage(float damageAmount, bool direction)
    {
        if (isBlocking) damageAmount -= blockingAmount;
        if (damageAmount > 0)
        {
            health -= damageAmount;
            if (health <= 0)
            {
                health = 0;
                StartCoroutine(Die());
            }
            if (direction)
            {
                rb.AddForce((Vector2.right + Vector2.up) * 2f, ForceMode2D.Impulse);

            }
            else
            {
                rb.AddForce((Vector2.left +Vector2.up)* 2f, ForceMode2D.Impulse);
            }
            imageHealthBar.fillAmount = health / healthMax;
        }
        else damageAmount = 0;
        if (DamageTextPrefab) ShowFloatingText((int)damageAmount);
    }
    public void ShowFloatingText(int damageAmount)
    {
        var go = Instantiate(DamageTextPrefab, transform.position, Quaternion.identity, transform);
        go.GetComponent<TextMesh>().text = damageAmount.ToString();
        go.GetComponent<TextMesh>().color = Color.red;

    }
    // Teleport, true = right, false = left
    public void ShowSmoke(bool face)
    {

        Vector3 pos;
        if (face)
        {
            pos = transform.position + new Vector3(-.35f, -.35f, 0);
        }
        else pos = transform.position + new Vector3(.35f, -.35f, 0);
        var go = Instantiate(SmokePrefab, pos, Quaternion.identity, transform);
        if (face) go.transform.eulerAngles = new Vector3(180, 0, -3f);
        else go.transform.eulerAngles = new Vector3(180, 180, -3f);
        Destroy(go, .3f);
    }
    // Animation Event
    public void AnimationAttack(float amount)
    {
        isSwingingSword = true;
        damageMultiplier = amount; // Depending on animation stage, up to 3x multiplier
    }
    public void AnimationBlock(float amount)
    {
        isBlocking = true;
        blockingAmount = amount; // Depending on animation stage, range is 15-50
    }
}

