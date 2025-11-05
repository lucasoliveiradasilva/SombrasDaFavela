using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int health = 50;
    public float speed = 3f;
    public float attackRange = 1.5f;
    public int damage = 10;
    public Transform player;
    public Animator anim;

    private void Update()
    {
        float distance = Vector2.Distance(transform.position, player.position);

        if (distance < attackRange)
        {
            anim.SetTrigger("Attack");
        }
        else
        {
            transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
            anim.SetBool("IsRunning", true);
        }
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
            Destroy(gameObject);
    }
}
