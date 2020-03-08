using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Home : MonoBehaviour {

	private SpriteRenderer spriteRenderer;
    [SerializeField] GameObject winText;

    public Sprite[] homeSprites;
    int houseLevel;

	// Use this for initialization
	void Start () {
		spriteRenderer = GetComponent<SpriteRenderer>();

        houseLevel = 0;

        spriteRenderer.sprite = homeSprites[0];
	}
	
    public void LevelUp()
    {
        houseLevel++;
        spriteRenderer.sprite = homeSprites[houseLevel];

        if (houseLevel >= homeSprites.Length - 1)
        {
            Win();
        }
    }

    void Win()
    {
        winText.SetActive(true);

        FindObjectOfType<Player>().Victory();

        ObjectSpawner[] spawners = FindObjectsOfType<ObjectSpawner>();
        foreach (ObjectSpawner spawner in spawners)
        {
            Destroy(spawner);
        }

        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            Destroy(enemy.gameObject);
        }

        Food[] foods = FindObjectsOfType<Food>();
        foreach (Food food in foods)
        {
            Destroy(food);
        }
    }

    IEnumerator LoadCredits()
    {
        yield return new WaitForSeconds(5f);

        SceneManager.LoadScene(2);
    }

}
