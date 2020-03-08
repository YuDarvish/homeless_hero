using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour {

    public delegate void CollectionEvent();
    public static event CollectionEvent OnCollected;

    Collider2D collider;

    [SerializeField] float timeToDestroyFood = 4f;

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    private void Start()
    {
        Destroy(gameObject, timeToDestroyFood);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.gameObject.GetComponent<Player>();
        if (!player) return;

        player.ConsumeFood();
        Destroy(gameObject);
    }
}
