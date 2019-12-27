using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigController : MonoBehaviour
{
    public Animator animator;
    public GameObject pointsEffect;
    public Vector3 pointsEffectOffset;

    public float health;
    public int points;

    [Header("Animation settings")]
    public float smileDuration = 3f;
    public float idleDurationMax = 5f;
    public float idleDurationMin = 2f;

    float idle;
    float smile;
    bool smiling;

    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;
        gameManager.AddPig();

        smile = smileDuration;
        idle = Random.Range(idleDurationMin, idleDurationMax);
    }

    void Update()
    {
        if (gameManager.EOG || gameManager.pause)
        {
            SoundManager.instance.Stop("pig oink");
            return;
        }

        if (transform.position.x < gameManager.safeZone.x || transform.position.x > gameManager.safeZone.y)
        {
            Destroy(gameObject);
            return;
        }

        if (!smiling)
        {
            idle -= Time.deltaTime;

            if (idle <= 0)
            {
                SoundManager.instance.Play("pig oink");

                smiling = true;
                animator.SetBool("smile", true);
                idle = Random.Range(idleDurationMin, idleDurationMax);
            }

            return;
        }

        smile -= Time.deltaTime;

        if (smile <= 0)
        {
            SoundManager.instance.Stop("pig oink");

            smile = smileDuration;
            animator.SetBool("smile", false);
            smiling = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (gameManager.EOG || gameManager.pause)
            return;

        if (collision.relativeVelocity.magnitude > health)
        {
            Destroy(gameObject);
            return;
        }

        if (collision.relativeVelocity.magnitude > health / 2)
        {
            animator.SetInteger("eyeDmg", animator.GetInteger("eyeDmg") + 1);
            SoundManager.instance.Play("pig collision");
        }
    }

    void Die()
    {
        Instantiate(pointsEffect, transform.position + pointsEffectOffset, Quaternion.identity);

        gameManager.RemovePig();
        gameManager.AddScore(points);
    }

    void OnDestroy()
    {
        if (gameManager.forceQuit || gameManager.EOG)
            return;

        SoundManager.instance.Stop("pig oink");
        SoundManager.instance.Play("pig destroyed");
        Die();
    }
}
