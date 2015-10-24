using UnityEngine;
using System.Collections;

public class Saucer : MonoBehaviour {

    public GameObject saucerBulletPrefab;
    public AudioClip bulletSfx;
    public AudioClip hitSfx;

    public float speed = 1f;
    public float maxFireWaitTime = 5f;
    public int score;

    private Animator animator;
    private GameObject gameManager;
    private Vector3 screenSW;
    private Vector3 screenNE;
    private AudioSource audioSource;

    private float destroyPadding = 2f;

	// Use this for initialization
	void Start () {

        animator = this.GetComponent<Animator>();
        audioSource = this.GetComponent<AudioSource>();

        // Get the screen bounds coordinates. Bottom left corner = screen SW, and top right = screen NE.
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

        StartCoroutine(Attack());

        animator.SetInteger("State", 0);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);

        if (transform.localPosition.x < screenSW.x - destroyPadding ||
            transform.localPosition.x > screenNE.x + destroyPadding ||
            transform.localPosition.y < screenSW.y - destroyPadding ||
            transform.localPosition.y > screenNE.y + destroyPadding)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Player")
        {
            StartCoroutine(Hit());
        }
    }

    public void SetGameManager(GameObject gameManagerObject)
    {
        gameManager = gameManagerObject;
    }

    public void SaucerHit()
    {
        StartCoroutine(Hit());
    }

    IEnumerator Attack()
    {
        for(float timer = Random.Range(0, maxFireWaitTime); timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        Instantiate(saucerBulletPrefab, transform.localPosition, transform.localRotation);
        audioSource.PlayOneShot(bulletSfx);

        StartCoroutine(Attack());
    }

    IEnumerator Hit()
    {
        audioSource.PlayOneShot(hitSfx);
        animator.SetInteger("State", 1);
        gameManager.GetComponent<GameManager>().UpdateScore(score);

        gameObject.GetComponent<Collider2D>().enabled = false;

        yield return new WaitForSeconds(0.4f);

        Destroy(gameObject);
    }
}
