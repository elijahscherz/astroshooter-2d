using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour {

    public GameObject spaceshipBulletPrefab;
    public GameObject spaceshipPowerupBulletPrefab;

    public AudioClip bulletSfx;
    public AudioClip hitSfx;
    public AudioClip shieldUpSfx;
    public AudioClip shieldDownSfx;
    public AudioClip powerupGrabbed;
    public AudioClip powerupWoreOff;

    // Ship properties.
    public float Speed { get; set; }
    public float TurnSpeed { get; set; }
    public float FireRate { get; set; }
    public float RespawnRate { get; set; }
    public float WarpCoolDown { get; set; }

    // Physics properties.
    //public float LinearDrag { get; set; }
    //public float Mass { get; set; }

    public bool CanReverse { get; set; }

    // Powerup properties.
    public float BulletPowerupTime { get; set; }
    public float ShipControlPowerupTime { get; set; }
    public float DoubleShotPowerupTime { get; set; }

    public int ShipTypeIndex { get; set; }

    public bool isShipControlActive = false;
    public bool isDoubleShotActive = false;

    private GameObject gameManager;
    private GameObject bulletPrefab;

    private Animator animator;

    private Vector3 screenSW;
    private Vector3 screenNE;

    private AudioSource audioSource;

    private float wrapPadding = 1f;
    private float accelRate = 0f;
    private float rotationRate = 0f;
    private float nextFire;
    private float nextWarp;

    private bool hit = false;
    private bool isAnimating = false;
    private bool shielded = true;

	// Use this for initialization
	void Start () {

        // Switch "this" to "gameObject"?
        animator = gameObject.GetComponent<Animator>();
        audioSource = gameObject.GetComponent<AudioSource>();

        bulletPrefab = spaceshipBulletPrefab;

        // Get the screen bounds coordinates. Bottom left corner = screen SW, and top right = screen NE.
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

        StartCoroutine(ShieldActive());
	}
	
	void FixedUpdate () {

        // Move the player!
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * (Speed * accelRate));

        if (rotationRate > 0)
        {
            transform.Rotate(Vector3.back * (rotationRate * TurnSpeed));
            rotationRate *= 0.0f;
        }
        else if (rotationRate < 0)
        {
            transform.Rotate(Vector3.forward * (rotationRate * -TurnSpeed));
            rotationRate *= 0.0f;
        }

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

        if(other.gameObject.tag == "Rock" || other.gameObject.tag == "Saucer")
        {
            StartCoroutine(Hit());
        }
    }

    public void ActivateShield(float timeToShield)
    {
        if (shielded)
            return;

        StartCoroutine(ShieldActive(timeToShield));
    }

    public void ActivatePowerupBullet()
    {
        StartCoroutine(PowerupBullet());
    }

    public void ActivateShipControl()
    {
        StartCoroutine(ShipControl());
    }

    public void ActivateDoubleShot()
    {
        StartCoroutine(DoubleShot());
    }

    public void ActivateLife()
    {
        audioSource.PlayOneShot(powerupGrabbed);
        gameManager.GetComponent<GameManager>().UpdateLives(-1);
    }

    public void SpaceshipHit()
    {
        if (shielded)
            return;

        StartCoroutine(Hit());
    }

    public void SetGameManager(GameObject gameManagerObject)
    {
        gameManager = gameManagerObject;
    }

    // Also rotates around z axis locally. Forward is counterclockwise.
    public void Turn(float rotation)
    {
        if (hit)
            return;

        rotationRate = rotation;
    }

    // Sets acceleration rate to 0 or 1 based on up-arrow input. Then sets the animation state accordingly.
    public void Move(float accel)
    {
        if (hit)
            return;

        if(isShipControlActive)
        {
            accelRate = (accel * 1.8f);
        }
        else
        {
            accelRate = accel;
        }

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
        if (hit || gameManager.GetComponent<GameManager>().isPaused)
            return;

        accelRate *= 0.0f;

        if(isShipControlActive)
        {
            gameObject.GetComponent<Rigidbody2D>().velocity *= 0.8f;
        }
        else
        {
            gameObject.GetComponent<Rigidbody2D>().velocity *= 0.99f;
        }

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
            nextFire = Time.time + FireRate;

            if(isDoubleShotActive)
            {
                GameObject bulletCloneLeft = Instantiate(bulletPrefab, transform.localPosition, transform.localRotation) as GameObject;
                bulletCloneLeft.transform.Rotate(Vector3.back * 15);

                GameObject bulletCloneRight = Instantiate(bulletPrefab, transform.localPosition, transform.localRotation) as GameObject;
                bulletCloneRight.transform.Rotate(Vector3.back * -15);
            }
            else
            {
                Instantiate(bulletPrefab, transform.localPosition, transform.localRotation);
            }
            audioSource.PlayOneShot(bulletSfx);
        }
    }

    public void Warp()
    {
        if (hit)
            return;

        if(Time.time > nextWarp)
        {
            nextWarp = Time.time + WarpCoolDown;

            float newXPos = Random.Range(screenSW.x, screenNE.x);
            float newYPos = Random.Range(screenSW.y, screenNE.y);

            transform.localPosition = new Vector3(newXPos, newYPos, 0);
            audioSource.PlayOneShot(shieldUpSfx);
        }
    }

    IEnumerator PowerupBullet()
    {
        audioSource.PlayOneShot(powerupGrabbed);
        bulletPrefab = spaceshipPowerupBulletPrefab;

        yield return new WaitForSeconds(BulletPowerupTime);

        audioSource.PlayOneShot(powerupWoreOff);
        bulletPrefab = spaceshipBulletPrefab;
    }

    IEnumerator ShipControl()
    {
        audioSource.PlayOneShot(powerupGrabbed);

        if (ShipTypeIndex == 2)
            yield break;

        isShipControlActive = true;
        CanReverse = true;

        yield return new WaitForSeconds(ShipControlPowerupTime);

        audioSource.PlayOneShot(powerupWoreOff);
        isShipControlActive = false;
        CanReverse = false;
    }

    IEnumerator DoubleShot()
    {
        audioSource.PlayOneShot(powerupGrabbed);
        isDoubleShotActive = true;

        yield return new WaitForSeconds(DoubleShotPowerupTime);

        audioSource.PlayOneShot(powerupWoreOff);
        isDoubleShotActive = false;
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

        yield return new WaitForSeconds(RespawnRate);

        gameObject.GetComponent<Renderer>().enabled = true;
        gameObject.GetComponent<Collider2D>().enabled = true;

        hit = false;
        gameManager.GetComponent<GameManager>().ResetShip();
        StartCoroutine(ShieldActive());
    }

    IEnumerator ShieldActive(float shieldingTime = 3f)
    {
        shielded = true;
        animator.SetInteger("State", 2);
        audioSource.PlayOneShot(shieldUpSfx);

        yield return new WaitForSeconds(shieldingTime);

        shielded = false;
        animator.SetInteger("State", 0);
        audioSource.PlayOneShot(shieldDownSfx);
    }

    void AnimationComplete()
    {
        isAnimating = false;
    }
}
