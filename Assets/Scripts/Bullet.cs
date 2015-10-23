using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

    public float speed = 1f;

    public enum bulletType { spaceship, saucer, spaceshipPowerup };

    public bulletType type;

    private Vector3 screenSW;
    private Vector3 screenNE;

    private float destroyPadding = 1f;

	// Use this for initialization
	void Start () {
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        gameObject.GetComponent<Rigidbody2D>().AddForce(transform.up * speed);

        if(transform.localPosition.x < screenSW.x - destroyPadding ||
            transform.localPosition.x > screenNE.x + destroyPadding ||
            transform.localPosition.y < screenSW.y - destroyPadding ||
            transform.localPosition.y > screenNE.y + destroyPadding)
        {
            Destroy(gameObject);
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(type)
        {
            case bulletType.spaceship: // When it is a spaceship bullet type..

                // If it hits either a rock or saucer, kill the gameObject.
                if (other.gameObject.tag == "Rock")
                {
                    other.gameObject.GetComponent<Rock>().RockHit();
                    Destroy(gameObject);
                }
                else if (other.gameObject.tag == "Saucer")
                {
                    other.gameObject.GetComponent<Saucer>().SaucerHit();
                    Destroy(gameObject);
                }

                break;
            
            case bulletType.saucer: // When it is a saucer bullet type..

                // If it hits a player, kill the object.
                if (other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().SpaceshipHit();
                    Destroy(gameObject);
                }

                break;

            case bulletType.spaceshipPowerup:

                // If it hits either a rock or saucer, kill the gameObject.
                if (other.gameObject.tag == "Rock")
                {
                    other.gameObject.GetComponent<Rock>().RockHit();
                }
                else if (other.gameObject.tag == "Saucer")
                {
                    other.gameObject.GetComponent<Saucer>().SaucerHit();
                }

                break;
        }
    }
}
