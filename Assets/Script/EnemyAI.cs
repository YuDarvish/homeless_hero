using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour {

	[SerializeField] float speed = 1.0f;

	private Vector2 playerPosition;
	private Vector2 position;

	GameObject player;

	RaycastHit2D hit;
	public LayerMask blockingLayer;
	public LayerMask playerLayer;

	bool isReachable = false;
	bool canMove = true;
    bool alive = true;

	Animator anim;

	private float nextFire;
	public float fireRate;

	public GameObject projectile;
	[SerializeField, Range(0f, 100f)] float rockSpeed;
	private Vector2 offset  = new Vector2(0.4f, 0.1f);

    AudioSource audio;
    [SerializeField] AudioClip throwRockClip;
    [SerializeField] AudioClip deathClip;

    Vector2 previousDirection;

	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		anim = GetComponentInChildren<Animator>();
        audio = GetComponent<AudioSource>();

        if (player.transform.position.x <= transform.position.x)
        {
            FlipSprite();
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (!alive) return;

		GetPlayerPosition ();

		if (CanAttackPlayer()){
			AttackPlayer ();
		}else if(CanMoveTowardPlayer()){
			MoveTowardPlay ();
		}else{
			//StandStill ();
		}
	}

	void OnTriggerEnter2D(Collider2D col){
		if (col.tag == "Player") {
			isReachable = true;
		} 
	}

	void OnTriggerExit2D(Collider2D col){
		if (col.tag == "Player") {
			isReachable = false;
		} 
	}

	#region Behaviours
	public void AttackPlayer(){
		if( Time.time > nextFire){
			nextFire = Time.time + fireRate;
			ThrowRock ();
		}

	}

	public bool CanAttackPlayer(){
		if (isReachable) {
			hit = Physics2D.Linecast (transform.position, playerPosition, playerLayer);

			if (hit.transform != null && hit.collider.tag == "Player") {
				Debug.DrawLine (transform.position, playerPosition, Color.red);
				return false;
			} else {
				return true;
			}
		}
		return false;
	}

	public bool CanMoveTowardPlayer(){
		hit = Physics2D.Linecast (transform.position, playerPosition, blockingLayer);

		if (hit.transform != null && hit.collider.tag == "Obstacle") {
			Debug.DrawLine (transform.position, playerPosition);
			return false;
		} else {
			return true;
		}
	}

	public bool CanMoveTo(Vector2 direction){
		return false;
	}
		
	public void GetPlayerPosition(){
		playerPosition = player.GetComponent<Transform> ().position;
	}

	public void MoveTowardPlay(){
		//float step = speed * Time.deltaTime;
		//transform.position = Vector2.MoveTowards(transform.position, playerPosition, step);
		Move (transform.position, playerPosition);
	}

	public void Move(Vector3 origin, Vector3 target){
		float step = speed * Time.deltaTime;
		transform.position = Vector2.MoveTowards(origin, target, step);

        Vector2 direction = (target - origin).normalized;
        if (Mathf.Sign(direction.x * previousDirection.x) < 0)
        {
            FlipSprite();
        }
        previousDirection = direction;
	}

	public void StandStill(){
		if (CanMoveTo (Vector2.up)) {
			Move (transform.position, (transform.position+(Vector3)Vector2.up));
		} else if(CanMoveTo (Vector2.down)) {
			Move(transform.position, (transform.position+(Vector3)Vector2.down));
		} else if(CanMoveTo (Vector2.right)){
			Move(transform.position, (transform.position+(Vector3)Vector2.right));
		} else{
			Move (transform.position, (transform.position+(Vector3)Vector2.left));
		}
	}

	void ThrowRock()
	{
		canMove = false;

		anim.SetBool("ThrowRock", true);
		StartCoroutine(ResumeMoveCoroutine(0.65f));
	}

	IEnumerator ResumeMoveCoroutine(float delay)
	{
		yield return new WaitForSeconds(delay);

		// Actual throwing rock
		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 direction = ((Vector3)playerPosition - transform.position).normalized;

		GameObject goRock = (GameObject)Instantiate(projectile, (Vector2)transform.position + offset * transform.localScale.x, Quaternion.identity);
		goRock.GetComponent<Rigidbody2D>().velocity = new Vector2(direction.x * rockSpeed, direction.y * rockSpeed);

        audio.clip = throwRockClip;
        audio.Play();

        yield return new WaitForSeconds(0.15f);

		anim.SetBool("ThrowRock", false);
		//canMove = true;
	}

    public void Die()
    {
        alive = false;
        anim.SetBool("Died", true);

        audio.clip = deathClip;
        audio.Play();

        Destroy(gameObject, 2f);
    }

    void FlipSprite()
    {
        transform.GetChild(0).gameObject.transform.Rotate(new Vector3(0f, 180f, 0f));
    }

	#endregion
}
