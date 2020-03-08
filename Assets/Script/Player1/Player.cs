using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public float speed;
	public int paperboardCount = 0;
	private Rigidbody2D rigidBody;
	private Vector2 moveVelocity;
	public GameObject projectile;
    public Vector2 rockVelocity;
    [SerializeField, Range(0f, 100f)] float rockSpeed;
    //bool canShoot = true;
    private Vector2 offset  = new Vector2(0.4f, 0.1f);
    public float cooldown;
    AudioSource audio;
    [SerializeField] AudioClip throwRockClip;
    [SerializeField] AudioClip deathClip;
    [SerializeField] AudioClip foodClip;
    [SerializeField] AudioClip victoryClip;
    [SerializeField] AudioClip losingClip;
    [SerializeField] AudioClip paperboardPickupClip;

    Vector2 throwDirection;
    bool defeat = false;

    [SerializeField, Tooltip("How much hunger gained per second."), Range(0f, 100f)] float hungerRatePercentage;
    [SerializeField, Tooltip("How much hunger lost when eating."), Range(0f, 100f)] float hungerLossPerFood;

    [SerializeField] Slider foodSlider;
    [SerializeField] BoxCollider2D levelBox;

    bool canMove = true;
    bool isAlive = true;
    public bool isHungry = true;
    Animator anim;

    public int spriteDirection = 1;

    Text paperboardText;

	// Use this for initialization
	void Start () {

        paperboardText = GameObject.Find("Canvas/Text").GetComponent<Text>();

		rigidBody = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();

        // Gets reference to food slider
        Slider[] sliders = FindObjectsOfType<Slider>();
        foreach (Slider slider in sliders)
        {
            if (slider.gameObject.CompareTag("Food"))
            {
                foodSlider = slider;
                break;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire") && canMove)
        {
            ThrowRock();
        }

		Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
		moveVelocity = moveInput.normalized * speed;

        if (speed > 0f && canMove && Mathf.Sign(moveInput.x * spriteDirection) < 0)
        {
            FlipSprite();
        }

        Hunger();
	}

	void FixedUpdate(){
        if (!canMove) return;

        if (!isAlive) return;

		rigidBody.MovePosition(rigidBody.position + moveVelocity * Time.fixedDeltaTime);

	}

    public void ConsumeFood()
    {
        foodSlider.value += (hungerLossPerFood / 100);

        audio.clip = foodClip;
        audio.Play();
    }

    void Hunger()
    {
        if (!isHungry) return;
        if (!isAlive) return;

        foodSlider.value -= (hungerRatePercentage/100) * Time.deltaTime;
        if (foodSlider.value <= 0f)
        {
            Die();
        }
    }

	public void GetPaperboard()
    {
        audio.clip = paperboardPickupClip;
        audio.Play();

        paperboardCount++;
        FindObjectOfType<Home>().LevelUp();
        paperboardText.text = paperboardCount.ToString();
	}
		
    void FlipSprite()
    {
        if (!isAlive) return;

        spriteDirection *= -1;
        transform.GetChild(0).Rotate(new Vector3(0f, 180f, 0f));
    }

    void ThrowRock()
    {
        canMove = false;

        anim.SetBool("ThrowRock", true);
        StartCoroutine(ResumeMoveCoroutine(0.65f));

        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        throwDirection = (mousePosition - transform.position).normalized;

        if (Mathf.Sign(throwDirection.x * spriteDirection) < 0)
        {
            FlipSprite();
        }
    }

     IEnumerator ResumeMoveCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Actual throwing rock
        GameObject goRock = (GameObject)Instantiate(projectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
        goRock.GetComponent<Rigidbody2D>().velocity = new Vector2(throwDirection.x * rockSpeed, throwDirection.y * rockSpeed);
        goRock.GetComponent<Rock>().fromPlayer = true;

        audio.clip = throwRockClip;
        audio.Play();

        yield return new WaitForSeconds(0.15f);

        anim.SetBool("ThrowRock", false);
        canMove = true;
    }

    public void Die()
    {
        canMove = false;
        isAlive = false;

        anim.SetBool("Died", true);
        StartCoroutine(ReloadLevel());
    }

    IEnumerator ReloadLevel()
    {
        audio.clip = deathClip;
        audio.Play();

        yield return new WaitForSeconds(1f);

        if (!defeat)
        {
            audio.clip = losingClip;
            audio.Play();
            defeat = true;
        }

        yield return new WaitForSeconds(3f);

        SceneManager.LoadScene(1);
    }

    public void Victory()
    {
        isHungry = false;

        audio.clip = victoryClip;
        audio.Play();
    }

}
