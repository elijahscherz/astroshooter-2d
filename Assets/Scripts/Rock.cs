using UnityEngine;
using System.Collections;

public class Rock : MonoBehaviour
{

    public GameObject childRockPrefab;

    public AudioClip hitSfx;

    public int numChildRocks = 1;
    public int score;
    public float speed = 1f;

    private GameObject gameManager;
    private Vector3 screenSW;
    private Vector3 screenNE;
    private AudioSource audioSource;

    private float wrapPadding = 1f;

    // Use this for initialization
    void Start()
    {

        audioSource = this.GetComponent<AudioSource>();

        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));

        transform.Rotate(Vector3.forward * Random.Range(0, 360));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);

        // Wrapping on sides.
        // When rock goes off left..
        if (transform.localPosition.x < screenSW.x - wrapPadding)
        {
            transform.localPosition = new Vector3(screenNE.x, transform.localPosition.y, transform.localPosition.z);
        }
        // When rock goes off right..
        else if (transform.localPosition.x > screenNE.x + wrapPadding)
        {
            transform.localPosition = new Vector3(screenSW.x, transform.localPosition.y, transform.localPosition.z);
        }

        // Wrapping on top and bottom.
        // When rock goes out the bottom..
        if (transform.localPosition.y < screenSW.y - wrapPadding)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, screenNE.y, transform.localPosition.z);
        }
        // When rock goes out the top..
        else if (transform.localPosition.y > screenNE.y + wrapPadding)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, screenSW.y, transform.localPosition.z);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            StartCoroutine(DestroyRock());
        }

        if (other.gameObject.tag == "Rock")
        {
            gameObject.GetComponent<Rigidbody2D>().velocity *= 0.1f;
            speed *= -0.98f;
            float rotateValue = Random.Range(-15, 15);
            transform.Rotate(Vector3.forward * rotateValue);
        }
    }

    public void SetGameManager(GameObject gameManagerObject)
    {
        gameManager = gameManagerObject;
    }

    public void RockHit()
    {
        StartCoroutine(DestroyRock());
    }

    IEnumerator DestroyRock()
    {
        audioSource.PlayOneShot(hitSfx);

        if (childRockPrefab != null)
        {
            for (int i = 0; i < numChildRocks; i++)
            {
                GameObject rockClone = Instantiate(childRockPrefab, transform.localPosition, Quaternion.identity) as GameObject;

                rockClone.GetComponent<Rock>().SetGameManager(gameManager);
            }
        }

        gameManager.GetComponent<GameManager>().UpdateScore(score);

        yield return new WaitForSeconds(0.2f);

        Destroy(gameObject);
    }
}
