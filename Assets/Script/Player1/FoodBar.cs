using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FoodBar : MonoBehaviour {

	public Slider foodBar;
	public float maxHealth;
	public float starve;

	// Use this for initialization
	void Start () {
		foodBar = GetComponent<Slider>();

		foodBar.maxValue = maxHealth;
		foodBar.value = starve;
	}
	
	// Update is called once per frame
	void Update () {
		starve+=0.1f;
 		foodBar.value = starve;
	}
}
