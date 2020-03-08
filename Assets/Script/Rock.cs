using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rock : MonoBehaviour {

    public bool fromPlayer;
    AudioSource audio;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (fromPlayer && collision.gameObject.CompareTag("Enemy"))
        {
            audio.Play();
            EnemyAI enemy = collision.gameObject.transform.parent.gameObject.GetComponent<EnemyAI>();
            enemy.Die();
            Destroy(gameObject);
        }

        if (!fromPlayer && collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().Die();
            audio.Play();
            Destroy(gameObject);
        }
    }
}
