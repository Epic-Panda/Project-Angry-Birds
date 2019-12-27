using System.Collections.Generic;
using UnityEngine;

public class ConstructionController : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public int health = 4;
    public string type = "wood";

    public int pointDmg = 50;
    public int pointsWorth = 500;
    public GameObject destroyPointsEffect;
    public GameObject dmgPointsEffect;

    int index = 0;

    int maxHealth;
    bool destroying = false;

    [Header("Dmg sprites")]
    public List<Sprite> damagedSprite;

    void Start()
    {
        maxHealth = health;
    }

    void Update()
    {
        if (transform.position.x < GameManager.instance.safeZone.x || transform.position.x > GameManager.instance.safeZone.y)
        {
            health = 1;
            ApplyDamage();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!destroying && collision.relativeVelocity.magnitude > 3f)
        {
            ApplyDamage();
            SoundManager.instance.Play(type + " collision");
        }
    }

    void ApplyDamage()
    {
        health--;

        if (health == 0)
        {
            Instantiate(destroyPointsEffect, transform.position, Quaternion.identity);
            GameManager.instance.AddScore(pointsWorth);

            destroying = true;
            Destroy(gameObject);
            return;
        }

        // if max health = 6 and num of effect is 3, moduo is 2
        if (health % (maxHealth / damagedSprite.Count) == 0)
        {
            index++;
            Instantiate(dmgPointsEffect, transform.position, Quaternion.identity);
            GameManager.instance.AddScore(pointDmg);
        }

        spriteRenderer.sprite = damagedSprite[index];
    }

    void OnDestroy()
    {
        if (GameManager.instance.EOG || GameManager.instance.pause || GameManager.instance.forceQuit)
            return;

        SoundManager.instance.Play(type + " destroyed");
    }
}
