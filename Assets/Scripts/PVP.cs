using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PVP : Player
{
    public Vector3 spawn;
    public PVP other;
    public int lives = 3;
    public int kills = 0;
    public bool isPaused = false;
    public override void Update()
    {
        if (isPaused) return;
        if (health == 0)
        {
            base.transform.position = new Vector3(5f, 15f, 0);
            health = 100;
            mana = 100;
            base.imageManaBar.fillAmount = mana / manaMax;
            base.imageHealthBar.fillAmount = health / healthMax;
        }
        else base.Update();
        
        
    }
    // Fade Out
    // Considerations for other player and what happens to this player for later
    protected override IEnumerator Die()
    {

        anim.Play(death.name);
        other.kills += 1;
        lives -= 1;
        Debug.Log(lives);
        gold.text = "Lives: " + lives;
        if(lives == 0)
        {
            PVPController.instance.Show();
        }
       
        yield return new WaitForSeconds(death.length - .2f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        // Damage
        if (isSwingingSword && collision is CapsuleCollider2D && collision.gameObject.layer == LayerMask.NameToLayer("PVP") && collision.gameObject.GetComponent<PVP>().health > 0)
        {
            // Lower enemy hp, consider crit damage
            collision.gameObject.GetComponent<PVP>().TakeDamage(damage * damageMultiplier, transform.eulerAngles.y != 180);
            isSwingingSword = false;
        }
    }
    void OnCollisionEnter2D(Collision2D other)
    {

        // Double Jump
        if (other.gameObject.layer == LayerMask.NameToLayer("Ground") || other.gameObject.layer == LayerMask.NameToLayer("PVP"))
        {
            if (IsGrounded())
            {
                jumpsLeft = 2;
                anim.SetBool("isWalking", false);
                //teleport = true ;
            }

        }
    }
    bool IsGrounded()
    {
            RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position + new Vector3(0, .1f, 0), -transform.up, jump);
            Debug.DrawRay(transform.position, -transform.up * jump);
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                {
                    return true;
                }
            }
        return false;
    }
}
