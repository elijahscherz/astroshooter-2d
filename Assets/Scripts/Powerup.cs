using UnityEngine;
using System.Collections;

public class Powerup : MonoBehaviour {

    public AudioClip spawnedSfx;

    public int score;

    public enum powerupType { shieldPowerup, laserPowerup, shipControlPowerup, doubleShotPowerup, addLifePowerup };

    public powerupType type;

    private GameObject gameManager;

    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();

        audioSource.PlayOneShot(spawnedSfx);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        switch(type)
        {
            case powerupType.shieldPowerup:

                if(other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().ActivateShield(10f);
                    gameManager.GetComponent<GameManager>().UpdateScore(score);
                    Destroy(gameObject);
                }   

                break;
            
            case powerupType.laserPowerup:

                if (other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().ActivatePowerupBullet();
                    gameManager.GetComponent<GameManager>().UpdateScore(score);
                    Destroy(gameObject);
                }

                break;

            case powerupType.shipControlPowerup:

                if (other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().ActivateShipControl();
                    gameManager.GetComponent<GameManager>().UpdateScore(score);
                    Destroy(gameObject);
                }

                break;

            case powerupType.doubleShotPowerup:

                if(other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().ActivateDoubleShot();
                    gameManager.GetComponent<GameManager>().UpdateScore(score);
                    Destroy(gameObject);
                }

                break;

            case powerupType.addLifePowerup:

                if (other.gameObject.tag == "Player")
                {
                    other.gameObject.GetComponent<Spaceship>().ActivateLife();
                    Destroy(gameObject);
                }

                break;
        }
    }

    public void SetGameManager(GameObject gameManagerObject)
    {
        gameManager = gameManagerObject;
    }
}

