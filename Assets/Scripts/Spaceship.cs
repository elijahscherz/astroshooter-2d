using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour {

    public GameObject spaceshipBulletPrefab;

    public AudioClip bulletSfx;
    public AudioClip hitSfx;
    public AudioClip shieldUpSfx;
    public AudioClip shieldDownSfx;

    public float speed = 1f;
    public float turnSpeed = 1f;
    public float fireRate = 0.5f;
    public float respawnRate = 1f;
    public float warpCoolDown = 0.5f;
    public float shieldTime = 3f;

    private GameObject gameManager;

    private Animator animator;

    private Vector3 screenSW;
    private Vector3 screenNE;

    private AudioSource audioSource;

    private float wrapPadding = 1f;
    private float accelRate = 0f;
    private float nextFire;
    private float nextWarp;

    private bool hit = false;
    private bool isAnimating = false;
    private bool shielded = true;

	// Use this for initialization
	void Start () {

        // Switch "this" to "gameObject"?
        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();

        // Get the screen bounds coordinates. Bottom left corner = screen SW, and top right = screen NE.
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

        StartCoroutine(ShieldActive());
	}
	
	// Update is called once per frame
    // Switch to FixedUpdate?
	void FixedUpdate () {

        // Move the player!
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * (speed * accelRate));

        // Wrapping on sides.
        // When player goes off left..
        if(transform.localPosition.x < screenSW.x - wrapPadding)
        {
            transform.localPosition = new Vector3(screenNE.x, transform.localPosition.y, transform.localPosition.z);
        }
        // When player goes off right..
        else if (transform.localPosition.x > screenNE.x + wrapPadding)
        {
            transform.localPosition = new Vector3(screenSW.x, transform.localPosition.y, transform.localPosition.z);
        }

        // Wrapping on top and bottom.
        // When player goes out the bottom..
        if (transform.localPosition.y < screenSW.y - wrapPadding)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, screenNE.y, transform.localPosition.z);
        }
        // When player goes out the top..
        else if (transform.localPosition.y > screenNE.y + wrapPadding)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, screenSW.y, transform.localPosition.z);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hit || shielded)
            return;

        // SaucerBullet?
        if(other.gameObject.tag == "Rock" || other.gameObject.tag == "Saucer")
        {
            StartCoroutine(Hit());
        }
    }

    public void SpaceshipHit()
    {
        StartCoroutine(Hit());
    }

    public void SetGameManager(GameObject gameManagerObject)
    {
        gameManager = gameManagerObject;
    }

    // Rotates the player locally around the z axis.
    public void TurnRight(float rotation = 1)
    {
        if(hit)
            return;

        transform.Rotate(Vector3.back * (rotation * turnSpeed));
    }

    // Also rotates around z axis locally. Forward is counterclockwise.
    public void TurnLeft(float rotation = 1)
    {
        if (hit)
            return;

        transform.Rotate(Vector3.forward * (rotation * -turnSpeed));
    }

    // Sets acceleration rate to 0 or 1 based on up-arrow input. Then sets the animation state accordingly.
    public void Move(float accel)
    {
        if (hit)
            return;

        accelRate = accel;

        if(shielded)
        {
            animator.SetInteger("State", 3);
        }
        else
        {
            animator.SetInteger("State", 1);
        }
    }

    // Smoothly decelerates the player when theres no input.
    public void Idle()
    {
        if (hit)
            return;

        accelRate *= 0.5f;

        // More accurate representation of slowing down.
        //gameObject.GetComponent<Rigidbody2D>().velocity *= .98f;

        if (shielded)
        {
            animator.SetInteger("State", 2);
        }
        else
        {
            animator.SetInteger("State", 0);
        }
    }

    // Creates bullet object at position and rotation of the player. "nextFire" is used as a bullet rate of fire moderator.
    public void ShootBullet()
    {
        if (hit)
            return;

        // If more time has passed than the required wait time to fire, a bullet is fired.
        if(Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Instantiate(spaceshipBulletPrefab, transform.localPosition, transform.localRotation);
            audioSource.PlayOneShot(bulletSfx);
        }
    }

    public void Warp()
    {
        if (hit)
            return;

        if(Time.time > nextWarp)
        {
            nextWarp = Time.time + warpCoolDown;

            float newXPos = Random.Range(screenSW.x, screenNE.x);
            float newYPos = Random.Range(screenSW.y, screenNE.y);

            transform.localPosition = new Vector3(newXPos, newYPos, 0);
            audioSource.PlayOneShot(shieldUpSfx);
        }
    }

    IEnumerator Hit()
    {
        hit = true;
        isAnimating = true;

        accelRate = 0f;
        animator.SetInteger("State", 4);
        audioSource.PlayOneShot(hitSfx);
        gameManager.GetComponent<GameManager>().UpdateLives(1);

        while (isAnimating)
        {
            yield return null;
        }

        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(respawnRate);

        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;

        hit = false;
        gameManager.GetComponent<GameManager>().ResetShip();
        StartCoroutine(ShieldActive());
    }

    IEnumerator ShieldActive()
    {
        shielded = true;
        animator.SetInteger("State", 2);
        audioSource.PlayOneShot(shieldUpSfx);

        yield return new WaitForSeconds(shieldTime);

        shielded = false;
        animator.SetInteger("State", 0);
        audioSource.PlayOneShot(shieldDownSfx);
    }

    void AnimationComplete()
    {
        isAnimating = false;
    }
}
