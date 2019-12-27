using System.Collections;
using UnityEngine;

public class BirdController : MonoBehaviour
{
    [Header("Bomb effect")]
    public GameObject bombExplosionEffect;
    public bool bomb = false;

    [Header("Red effect")]
    public GameObject windEffect;

    [Header("Points effect")]
    public int pointsWorth = 1000;
    public GameObject pointsEffect;
    public Vector3 pointsEffectOffset;

    [Header("Bird settings")]
    public string birdName;
    public Animator animator;
    public Rigidbody2D hookRb;
    public Rigidbody2D rb;

    public GameObject trailDot;
    GameObject trail;
    float trailDistance = .1f;
    float trailDistanceTime = .1f;

    public float maxDragDistance = 2f;

    [HideInInspector]
    public bool inAir = false;
    [HideInInspector]
    public bool clone = false; // for blue bird super power

    bool startCounting = false;
    float timeLimit = 10f;
    float timePassed = 0;

    bool pressed = false;
    bool released = false;

    bool waitBeforeDetection = true;
    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.instance;

        if (!trail)
            trail = GameObject.Find("Trail");
        if (clone)
            waitBeforeDetection = false;
    }

    void Update()
    {
        if (gameManager.EOG || gameManager.pause)
            return;

        if (startCounting)
        {
            timePassed += Time.deltaTime;
            if (timePassed >= timeLimit)
            {
                Destroy(gameObject);
                return;
            }
        }

        // drag bird
        if (!released && pressed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (Vector3.Distance(mousePos, hookRb.position) > maxDragDistance)
                rb.position = hookRb.position + (mousePos - hookRb.position).normalized * maxDragDistance;
            else
                rb.position = mousePos;

            // rotate towards hook
            Vector2 direction = hookRb.position - rb.position;
            rb.rotation = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        }

        // if bird is done moving destroy it unless it is Bomb
        if (released)
            if (rb.IsSleeping() && !bomb || !bomb && !inAir && rb.velocity.magnitude < 0.0005f && rb.angularVelocity < 0.0005f)
            {
                Destroy(gameObject);
                return;
            }

        if (transform.position.x < gameManager.safeZone.x || transform.position.x > gameManager.safeZone.y)
        {
            Destroy(gameObject);
            return;
        }

        // rotate toward direction till collision
        if (inAir)
        {
            rb.rotation = Mathf.Atan2(rb.velocity.y, rb.velocity.x) * Mathf.Rad2Deg;

            trailDistanceTime -= Time.deltaTime;

            if (trailDistanceTime <= 0)
            {
                Instantiate(trailDot, transform.position, Quaternion.identity, trail.transform);
                trailDistanceTime = trailDistance;
            }

            // on mouse click activate super power
            if (Input.GetMouseButtonDown(0) && !animator.GetBool("superPower"))
            {
                // create big dot
                GameObject bigDot = Instantiate(trailDot, transform.position, Quaternion.identity, trail.transform);
                trailDistanceTime = trailDistance;
                bigDot.transform.localScale = Vector3.one;

                animator.SetBool("superPower", true);
            }
        }
    }

    // special effect of birds called from animation events
    #region bird effect
    // Bomb super power
    void Explode()
    {
        Instantiate(bombExplosionEffect, transform.position, Quaternion.identity);
        SoundManager.instance.Play("tnt explosion");
        Destroy(gameObject);
    }

    // Red super power
    void Wind()
    {
        float radius = GetComponent<CircleCollider2D>().radius;
        Vector2 size = windEffect.GetComponent<BoxCollider2D>().size;
        Vector2 direction = rb.velocity.normalized * (radius + size.x / 2);

        Instantiate(windEffect, transform.position + new Vector3(direction.x, direction.y, 0), transform.rotation);
    }

    // Chuck super power
    void SuperSpeed()
    {
        rb.velocity = rb.velocity * 2;
    }

    // Blue super power
    void SplitToThree()
    {
        if (clone)
            return;

        BirdController blueClone;

        float radius = GetComponent<CircleCollider2D>().radius * 2 + .02f;
        Vector2 direction = RotateVector(rb.velocity.normalized * radius, 30);

        GameObject blue1 = Instantiate(gameObject, transform.position + new Vector3(direction.x, direction.y, 0), Quaternion.identity);

        blueClone = blue1.GetComponent<BirdController>();
        blueClone.clone = true;
        blueClone.released = true;
        blueClone.inAir = true;
        blueClone.animator.SetBool("inAir", true);
        blueClone.animator.SetBool("superPower", true);
        blueClone.rb.velocity = RotateVector(rb.velocity, 30);

        // clone 2
        direction = RotateVector(rb.velocity.normalized * radius, -30);
        blue1 = Instantiate(gameObject, transform.position + new Vector3(direction.x, direction.y, 0), Quaternion.identity);

        blueClone = blue1.GetComponent<BirdController>();
        blueClone.clone = true;
        blueClone.released = true;
        blueClone.inAir = true;
        blueClone.animator.SetBool("inAir", true);
        blueClone.animator.SetBool("superPower", true);
        blueClone.rb.velocity = RotateVector(rb.velocity, -30);
    }

    Vector2 RotateVector(Vector2 v, float degree)
    {
        float cos = Mathf.Cos(degree * Mathf.Deg2Rad);
        float sin = Mathf.Sin(degree * Mathf.Deg2Rad);

        return new Vector2(cos * v.x - sin * v.y, sin * v.x + cos * v.y);
    }
    #endregion

    public void AddPoints()
    {
        Instantiate(pointsEffect, transform.position + pointsEffectOffset, Quaternion.identity);
        gameManager.AddScore(pointsWorth);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.tag == "ground" && waitBeforeDetection)
            return;

        if (collision.relativeVelocity.magnitude >= 2)
            SoundManager.instance.Play(birdName + " collision");

        gameManager.cameraDragEnabled = true;

        startCounting = true;
        inAir = false;
        animator.SetBool("hit", true);
        animator.SetBool("inAir", false);
    }

    void OnMouseDown()
    {
        if (released || gameManager.EOG || gameManager.pause)
            return;

        gameManager.cameraDragEnabled = false;
        gameManager.cameraController.goToTarget = true;

        SoundManager.instance.Play("slingshot streched");

        pressed = true;
    }

    void OnMouseUp()
    {
        if (released || gameManager.EOG || gameManager.pause)
            return;

        if (!trail)
            trail = GameObject.Find("Trail");

        if (trail)
        {
            trail.name = "trial delete";
            Destroy(trail.gameObject);
            trail = null;
        }

        if (!trail)
        {
            trail = new GameObject("Trail");
            trail.transform.position = Vector3.zero;
            trail.transform.rotation = Quaternion.identity;
        }

        gameManager.cameraController.goToTarget = true;
        released = true;
        pressed = false;

        animator.SetBool("inAir", true);
        inAir = true;

        rb.isKinematic = false;
        StartCoroutine(Release());
    }

    IEnumerator Release()
    {
        yield return new WaitForSeconds(.1f);
        GetComponent<SpringJoint2D>().enabled = false;
        SoundManager.instance.Play(birdName + " flying");
        waitBeforeDetection = false;
    }

    public void PrepareForLaunch()
    {
        rb.position = hookRb.position;//new Vector2(-3.776f, -1.667f);
        gameObject.GetComponent<CircleCollider2D>().enabled = true;
    }

    void Die()
    {
        SoundManager.instance.Play("bird destroyed");
        gameManager.RemoveBird();
    }

    private void OnDestroy()
    {
        if (!gameManager.forceQuit && !gameManager.EOG && !clone)
            Die();
    }
}
