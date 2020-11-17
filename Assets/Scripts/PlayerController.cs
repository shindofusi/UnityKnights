using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : Player
{
    public KeyCode fuse;
    public bool isPaused = false;

    // Update is called once per frame
    public override void Update()
    {
        if (isPaused) return;
        base.Update();
        // If not dead
        if (health > 0 && !isDead)
        {
            if (Input.GetKey(KeyCode.Space) && !players[otherPlayerNum].activeSelf && !players[otherPlayerNum].GetComponent<PlayerController>().isDead)
            {
                if (players[otherPlayerNum].GetComponent<PlayerController>().health > 0)
                {
                    players[otherPlayerNum].SetActive(true);
                    players[otherPlayerNum].transform.position = transform.position;
                    GetComponent<SpriteRenderer>().color = color;
                    damage /= 2;
                }
            }
            // Fuse
            if (Input.GetKey(fuse) && players[otherPlayerNum].activeSelf)
            {
                if (anim.GetBool("isRunning")) speed -= runningSpeed; // avoid glitch of speed multiplying
                gameObject.SetActive(false);
                players[otherPlayerNum].GetComponent<SpriteRenderer>().color = new Color(255, 255, 255);
                players[otherPlayerNum].GetComponent<PlayerController>().damage *= 2;
            }
        }

    }
    protected override IEnumerator Die()
    {
        anim.Play(death.name);
        yield return new WaitForSeconds(death.length - .2f);
        isDead = true;
        gameObject.SetActive(false);
        if (players[otherPlayerNum] == null || !players[otherPlayerNum].activeSelf) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnTriggerStay2D(Collider2D collision)
    {
        // Damage
        if (isSwingingSword && collision is CapsuleCollider2D && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            // Lower enemy hp, consider crit damage
            if (collision.gameObject.GetComponent<Enemy>() && collision.gameObject.GetComponent<Enemy>().health > 0)
            {
                money += collision.gameObject.GetComponent<Enemy>().TakeDamage(damage * damageMultiplier, transform.eulerAngles.y != 180);
            }
            else if (collision.gameObject.GetComponent<Boss>() && collision.gameObject.GetComponent<Boss>().health > 0)
            {
                money += collision.gameObject.GetComponent<Boss>().TakeDamage(damage * damageMultiplier, transform.eulerAngles.y != 180);
            }
            else return;
            gold.text = "Score: " + (money).ToString();
            isSwingingSword = false;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {

        // Double Jump
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, .1f, 0), -transform.up, jump);
            Debug.DrawRay(transform.position, -transform.up * jump);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    jumpsLeft = 2;
                    anim.SetBool("isWalking", false);
                }
            }

        }
    }
}
