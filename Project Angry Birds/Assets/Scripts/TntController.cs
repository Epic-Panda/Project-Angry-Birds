using UnityEngine;

public class TntController : MonoBehaviour
{

    public GameObject explosionPrefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.relativeVelocity.magnitude < .5f)
            return;

        Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        SoundManager.instance.Play("tnt explosion");
        Destroy(gameObject);
    }
}
