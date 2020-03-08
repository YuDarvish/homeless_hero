using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Paperboard : MonoBehaviour {

	Collider2D collider;
    CollectableEventFirer collectableEventFirer;

	private void Awake(){
		collider = GetComponent<Collider2D>();
        collectableEventFirer = GetComponent<CollectableEventFirer>();
	}

	private void OnTriggerEnter2D(Collider2D collision){
		
		Player player = collision.gameObject.GetComponent<Player>();
		if (!player) return;

		player.GetPaperboard();
        if (collectableEventFirer)
        {
            collectableEventFirer.Collected();
        }

		Destroy(gameObject);
	}
}
