using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject spaceshipPrefab;
    public GameObject startingRockPrefab;
    public GameObject saucerPrefab;
    public GameObject shieldPowerupPrefab;
    public GameObject bulletPowerupPrefab;
    public GameObject shipControlPowerupPrefab;
    public GameObject doubleShotPowerupPrefab;
<<<<<<< HEAD
    public GameObject addLifePowerupPrefab;
=======
	public GameObject addLifePowerupPrefab;
>>>>>>> origin/master
    public GameObject gameUI;
    public GameObject mainUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject finalScoreText;
    public GameObject scoreText;
    public GameObject livesText;

    public enum gameState { main, gamePaused, game, gameOver };
    public gameState state;

    public int playerLives = 3;
    public int score = 0;
    public int numStartingRocks = 2;

    public float saucerSpawnRate = 5f;
    public float powerupSpawnRate = 18f;

    private GameObject player;
    private GameObject powerupPrefab;

    private Vector3 screenSW;
    private Vector3 screenNE;
    private Vector3 screenSE;
    private Vector3 screenNW;

    private Spaceship spaceship;

    private int rockSpawnRadius = 4;
    private int startingScore;
    private int startingLives;

    private bool isPaused;

	// Use this for initialization
	void Start () {

        mainUI.SetActive(false);
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);

        switch(state)
        {
            case gameState.main:
                mainUI.SetActive(true);
                break;

            case gameState.game:
                gameUI.SetActive(true);
                break;

            case gameState.gameOver:
                gameOverUI.SetActive(true);
                break;

            case gameState.gamePaused:
                pauseMenuUI.SetActive(true);
                break;
        }

        startingScore = score;
        startingLives = playerLives;
        UpdateScore(0);
        UpdateLives(0);
	}
	
	// Update is called once per frame
	void Update () {

        switch(state)
        {
            case gameState.main:

                if(Input.GetKeyDown(KeyCode.Return))
                {
                    StartCoroutine(GameStart());
                }

                break;

            case gameState.gamePaused:

                GamePaused();

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    state = gameState.game;
                }
                else if(Input.GetKeyDown(KeyCode.Q))
                {
                    Application.Quit();
                }

                break;

            case gameState.gameOver:

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    GameObject[] rocksToDestroy = GameObject.FindGameObjectsWithTag("Rock");
                    for(int i = 0; i < rocksToDestroy.Length; i++)
                    {
                        Destroy(rocksToDestroy[i]);
                    }

                    StartCoroutine(GameStart());
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                }

                break;

            case gameState.game:
                {
                    GameUnpaused();

                    // Checking for player input on both axes. Value of 1 or -1 for each.
                    float translation = Input.GetAxis("Vertical");
                    float rotation = Input.GetAxis("Horizontal");

                    // Switch to 0.1?
                    if (rotation > 0.1) // When pushing the right arrow.
                    {
                        spaceship.TurnRight(rotation);
                    }

                    if (rotation < 0.1) // When pushing the left arrow.
                    {
                        spaceship.TurnLeft(rotation);
                    }

                    if (translation >= 0.5f) // When up arrow is pushed.
                    {
                        // This will actually just set the acceleration rate based on the input (1 or 0). Also sets the State in Mechanim.
                        spaceship.Move(translation);
                    }
                    else if ((translation <= -0.5f) && spaceship.isShipControlActive)
                    {
                        spaceship.Move(translation);
                    }
                    else
                    {
                        // No movement keys being pressed calls the Idle() function.
                        spaceship.Idle();
                    }

                    // If the player hits the spacebar shoot some lasers.
                    if (Input.GetButton("Jump"))
                        spaceship.ShootBullet();

                    if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                    {
                        spaceship.Warp();
                    }

                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                            state = gameState.gamePaused;
                    }

                    GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");
                    if (rocks.Length <= 0)
                    {
                        for (int i = 0; i < numStartingRocks; i++)
                        {
                            float rockPosX = rockSpawnRadius * Mathf.Cos(Random.Range(0, 360));
                            float rockPosy = rockSpawnRadius * Mathf.Sin(Random.Range(0, 360));

                            GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(rockPosX, rockPosy, 0), Quaternion.identity) as GameObject;

                            rockClone.GetComponent<Rock>().SetGameManager(this.gameObject);
                        }
                    }

                    break;
                }
        }
	}

    public void ResetShip()
    {
        player.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void UpdateScore(int scoreToAdd)
    {
        score += scoreToAdd;
        scoreText.GetComponent<GUIText>().text = "Score: " + score;
    }

    public void UpdateLives(int livesLost)
    {
        playerLives -= livesLost;
        livesText.GetComponent<GUIText>().text = "Lives: " + playerLives;

        if(playerLives < 1)
        {
            StartCoroutine(GameEnd());

        }
    }

    IEnumerator SaucerSpawn()
    {
        for(float timer = saucerSpawnRate; timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        int cornerSelection = Random.Range(0, 4);

        Vector3 saucerSpawnPos = new Vector3(0,0,0);

        if(cornerSelection == 0)
        {
            saucerSpawnPos = new Vector3(screenSW.x, screenSW.y, 0);
        }
        else if(cornerSelection == 1)
        {
            saucerSpawnPos = new Vector3(screenSE.x, screenSE.y, 0);
        }
        else if(cornerSelection == 2)
        {
            saucerSpawnPos = new Vector3(screenNE.x, screenNE.y, 0);
        }
        else if(cornerSelection == 3)
        {
            saucerSpawnPos = new Vector3(screenNW.x, screenNW.y, 0);
        }

        GameObject saucerClone = Instantiate(saucerPrefab, saucerSpawnPos, Quaternion.identity) as GameObject;
        saucerClone.GetComponent<Saucer>().SetGameManager(this.gameObject);

        if (cornerSelection == 0)
        {
            saucerClone.transform.Rotate(Vector3.back * Random.Range(0, 90));
        }
        else if (cornerSelection == 1)
        {
            saucerClone.transform.Rotate(Vector3.back * Random.Range(90, 180));
        }
        else if (cornerSelection == 2)
        {
            saucerClone.transform.Rotate(Vector3.back * Random.Range(180, 270));
        }
        else if (cornerSelection == 3)
        {
            saucerClone.transform.Rotate(Vector3.back * Random.Range(270, 360));
        }

        StartCoroutine(SaucerSpawn());
    }
    
    IEnumerator GameStart()
    {
        mainUI.SetActive(false);
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(false);
        state = gameState.game;
<<<<<<< HEAD
        UpdateScore(0);
        UpdateLives(0);
=======
        UpdateLives (0);
>>>>>>> origin/master
        player = Instantiate(spaceshipPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        spaceship = player.GetComponent<Spaceship>();
        spaceship.SetGameManager(this.gameObject);
        for (int i = 0; i < numStartingRocks; i++)
        {
            float rockPosX = rockSpawnRadius * Mathf.Cos(Random.Range(0, 360));
            float rockPosy = rockSpawnRadius * Mathf.Sin(Random.Range(0, 360));

            GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(rockPosX, rockPosy, 0), Quaternion.identity) as GameObject;

            rockClone.GetComponent<Rock>().SetGameManager(this.gameObject);
        }

        // ScreenSE and ScreenNW are incorrectly named. Change in the future.
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
        screenSE = new Vector3(screenSW.x, screenNE.y, 0);
        screenNW = new Vector3(screenNE.x, screenSW.y, 0);

        StartCoroutine(SaucerSpawn());
        StartCoroutine(PowerupSpawn());

        yield return null;
    }

    IEnumerator GameEnd()
    {
        mainUI.SetActive(false);
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
        pauseMenuUI.SetActive(false);
        state = gameState.gameOver;

        finalScoreText.GetComponent<GUIText>().text = "Final Score: " + score;

        Destroy(player);
        StopAllCoroutines();

        score = startingScore;
        playerLives = startingLives;

        yield return null;
    }

    IEnumerator PowerupSpawn()
    {
        for (float timer = powerupSpawnRate; timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        float powerupPosX = Random.Range((screenSW.x + 2), (screenNE.x - 2));
        float powerupPosY = Random.Range((screenSW.y + 2), (screenNE.y - 2));

        int randomPowerUp = Random.Range(0, 5);

        if(randomPowerUp == 0)
        {
            GameObject shieldPowerupClone = Instantiate(shieldPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            shieldPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        if (randomPowerUp == 1)
        {
            GameObject bulletPowerupClone = Instantiate(bulletPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            bulletPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        if (randomPowerUp == 2)
        {
            GameObject shipControlPowerupClone = Instantiate(shipControlPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            shipControlPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        if (randomPowerUp == 3)
        {
            GameObject doubleShotPowerupClone = Instantiate(doubleShotPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            doubleShotPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }
		if (randomPowerUp == 4)
		{
			GameObject addLifePowerupClone = Instantiate(addLifePowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
			addLifePowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
		}

        if (randomPowerUp == 4)
        {
            GameObject addLifePowerupClone = Instantiate(addLifePowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            addLifePowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        StartCoroutine(PowerupSpawn());
    }

    void GamePaused()
    {
        mainUI.SetActive(false);
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        Time.timeScale = 0;
        Cursor.visible = true;
    }

    void GameUnpaused()
    {
        mainUI.SetActive(false);
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        Time.timeScale = 1;
        Cursor.visible = false;
    }
}
