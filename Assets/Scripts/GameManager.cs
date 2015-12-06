using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameManager : MonoBehaviour {

    // Prefab variable GameObjects set in the Unity interface later.
    public GameObject spaceshipDefaultPrefab;
    public GameObject spaceshipSpeedyPrefab;
    public GameObject spaceshipBlazePrefab;
    public GameObject startingRockPrefab;
    public GameObject saucerPrefab;
    public GameObject shieldPowerupPrefab;
    public GameObject bulletPowerupPrefab;
    public GameObject shipControlPowerupPrefab;
    public GameObject doubleShotPowerupPrefab;
    public GameObject addLifePowerupPrefab;

    // UI element GameObjects also to be set in Unity interface, then used here of course.
    public GameObject gameUI;
    public GameObject pauseMenuUI;
    public GameObject gameOverUI;
    public GameObject finalScoreText;
    public GameObject scoreText;
    public GameObject livesText;
    public GameObject highscoreText;

    // This enum stores different possible states of the game, we can then set these and do different
    // things depending on what state we are currently in...
    public enum gameState { gamePaused, game, gameOver };
    // .. using this variable.
    public gameState state;

    public enum shipType { defaultShip, speedyShip, blazeShip}

    // Starting values for game elements.
    public int playerLives = 3;
    public int score = 0;
    public int numStartingRocks = 2;

    public float saucerSpawnRate = 5f;
    public float powerupSpawnRate = 18f;

    public bool isPaused;

    private GameObject player;
    private GameObject spaceshipPrefab;

    private Vector3 screenSW;
    private Vector3 screenNE;
    private Vector3 screenSE;
    private Vector3 screenNW;

    // Used for calling to the player's spaceship.
    private Spaceship spaceship;

    // The radius from the origin that the rocks can spawn.
    private int rockSpawnRadius = 4;
    private int startingScore;
    private int startingLives;
    private int highscore = 0;
    private int shipTypeIndex = 1;

	// Use this for initialization.
	void Start () {

        // These four lines set the UI elements to hidden when the game first starts.
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        // These cases allow for leniency when activating UI that applies specifically
        // to the state during the start of this script.
        switch(state)
        {
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

        StartCoroutine(GameStart());
	}
	
	// Update is called once per frame.
	void Update () {

        switch(state)
        {
            // While in the gameState "gamePaused" execute this code..
            case gameState.gamePaused:

                GamePaused();

                if(Input.GetKeyDown(KeyCode.Escape))
                {
                    // Resumes the game, by setting it back to the "game" gameState.
                    state = gameState.game;
                }

                break;

            // When the game ends (basically), run these lines of code..
            case gameState.gameOver:

                // When the player strikes Enter/Return to restart the game. (Previously at highscore screen.)
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Creates an array of the leftover Rocks in the scene, and destroys them. Finds using Unity Tags.
                    GameObject[] rocksToDestroy = GameObject.FindGameObjectsWithTag("Rock");

                    /* Starting int "i" equals zero (represents the current Rock in the array) and until
                     that int is at a value less than the length of the rocksToDestroy array, destroy the
                     current Rock in the for loop. */
                    
                    for(int i = 0; i < rocksToDestroy.Length; i++)
                    {
                        Destroy(rocksToDestroy[i]);
                    }

                    // Creates an array just like the rocksToDestroy array, but for powerups.
                    GameObject[] powerupsToDestroy = GameObject.FindGameObjectsWithTag("Powerup");

                    // The for loop also functions in the same fashion.
                    for (int i = 0; i < powerupsToDestroy.Length; i++)
                    {
                        Destroy(powerupsToDestroy[i]);
                    }
                        
                        // When all that is finally done getting cleaned up, restart the game.
                        StartCoroutine(GameStart());
                }
                else if (Input.GetKeyDown(KeyCode.Escape))
                {
                    Application.Quit();
                }

                break;

            case gameState.game:
                {
                    // Resumes the timeScale, and disables the pauseUI.
                    GameUnpaused();

                    // Checking for player input on both axes. Value of 1 or -1 for each.
                    float translation = Input.GetAxis("Vertical");
                    float rotation = Input.GetAxis("Horizontal");

                    if (rotation > 0) // When pushing the right arrow.
                    {
                        spaceship.Turn(rotation);
                    }

                    if (rotation < 0) // When pushing the left arrow.
                    {
                        spaceship.Turn(rotation);
                    }

                    if (translation >= 0.5f) // When up arrow is pushed.
                    {
                        // This will actually just set the acceleration rate based on the input (1 or 0).
                        // Also sets the State in Mechanim.
                        spaceship.Move(translation);
                    }
                    else if ((translation <= -0.5f) && spaceship.isShipControlActive) // When the down arrow is pushed and the
                    {                                                                 // ship control powerup is active.
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

                    // We support warping with the Ctrl key here at GlitchedPixel Inc..
                    if (Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl))
                    {
                        spaceship.Warp();
                    }

                    // We also support pausing of the game, give us an award. (Please.. heh.)
                    if(Input.GetKeyDown(KeyCode.Escape))
                    {
                            state = gameState.gamePaused;
                    }

                    // This array and following "if" statement and "for" loop keep track of the Rocks in the scene.
                    GameObject[] rocks = GameObject.FindGameObjectsWithTag("Rock");

                    // If there are no Rocks left...
                    if (rocks.Length <= 0)
                    {
                        // Spawn more! Same as the initial amount. Creates neverending gameplay.
                        for (int i = 0; i < numStartingRocks; i++)
                        {
                            float rockPosX = rockSpawnRadius * Mathf.Cos(UnityEngine.Random.Range(0, 360));
                            float rockPosy = rockSpawnRadius * Mathf.Sin(UnityEngine.Random.Range(0, 360));

                            GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(rockPosX, rockPosy, 0), Quaternion.identity) as GameObject;

                            rockClone.GetComponent<Rock>().SetGameManager(this.gameObject);
                        }
                    }

                    break;
                }
        }
	}

    void ShipPrefabSet(int shipTypeIndex)
    {
        switch (shipTypeIndex)
        {
            case 1:
                spaceshipPrefab = spaceshipDefaultPrefab;
                break;

            case 2:
                spaceshipPrefab = spaceshipSpeedyPrefab;
                break;

            case 3:
                spaceshipPrefab = spaceshipBlazePrefab;
                break;
        }
    }

    void ShipTypeSet(int shipTypeIndex)
    {
        switch (shipTypeIndex)
        {
            case 1:
                spaceship.Speed = ShipData.SHIP_DEFAULT_SPEED;
                spaceship.TurnSpeed = ShipData.SHIP_DEFAULT_TURNSPEED;
                spaceship.FireRate = ShipData.SHIP_DEFAULT_FIRERATE;
                spaceship.RespawnRate = ShipData.SHIP_DEFAULT_RESPAWNRATE;
                spaceship.WarpCoolDown = ShipData.SHIP_DEFAULT_WARPCOOLDOWN;
                spaceship.BulletPowerupTime = ShipData.SHIP_DEFAULT_BULLETPOWERUPTIME;
                spaceship.ShipControlPowerupTime = ShipData.SHIP_DEFAULT_SHIPCONTROLPOWERUPTIME;
                spaceship.DoubleShotPowerupTime = ShipData.SHIP_DEFAULT_DOUBLESHOTPOWERUPTIME;

                player.GetComponent<Rigidbody2D>().mass = ShipData.SHIP_DEFAULT_MASS;
                player.GetComponent<Rigidbody2D>().drag = ShipData.SHIP_DEFAULT_LINEARDRAG;
                break;

            case 2:
                spaceship.Speed = ShipData.SHIP_SPEEDY_SPEED;
                spaceship.TurnSpeed = ShipData.SHIP_SPEEDY_TURNSPEED;
                spaceship.FireRate = ShipData.SHIP_SPEEDY_FIRERATE;
                spaceship.RespawnRate = ShipData.SHIP_SPEEDY_RESPAWNRATE;
                spaceship.WarpCoolDown = ShipData.SHIP_SPEEDY_WARPCOOLDOWN;
                spaceship.BulletPowerupTime = ShipData.SHIP_SPEEDY_BULLETPOWERUPTIME;
                spaceship.ShipControlPowerupTime = ShipData.SHIP_SPEEDY_SHIPCONTROLPOWERUPTIME;
                spaceship.DoubleShotPowerupTime = ShipData.SHIP_SPEEDY_DOUBLESHOTPOWERUPTIME;

                player.GetComponent<Rigidbody2D>().mass = ShipData.SHIP_SPEEDY_MASS;
                player.GetComponent<Rigidbody2D>().drag = ShipData.SHIP_SPEEDY_LINEARDRAG;
                break;

            case 3:
                spaceship.Speed = ShipData.SHIP_BLAZE_SPEED;
                spaceship.TurnSpeed = ShipData.SHIP_BLAZE_TURNSPEED;
                spaceship.FireRate = ShipData.SHIP_BLAZE_FIRERATE;
                spaceship.RespawnRate = ShipData.SHIP_BLAZE_RESPAWNRATE;
                spaceship.WarpCoolDown = ShipData.SHIP_BLAZE_WARPCOOLDOWN;
                spaceship.BulletPowerupTime = ShipData.SHIP_BLAZE_BULLETPOWERUPTIME;
                spaceship.ShipControlPowerupTime = ShipData.SHIP_BLAZE_SHIPCONTROLPOWERUPTIME;
                spaceship.DoubleShotPowerupTime = ShipData.SHIP_BLAZE_DOUBLESHOTPOWERUPTIME;

                player.GetComponent<Rigidbody2D>().mass = ShipData.SHIP_BLAZE_MASS;
                player.GetComponent<Rigidbody2D>().drag = ShipData.SHIP_BLAZE_LINEARDRAG;
                break;
        }
    }

    // While the ship / player is hidden after dying, we move them back to center. Sneaky.
    public void ResetShip()
    {
        player.transform.localPosition = new Vector3(0, 0, 0);
    }

    // Available to call to update the current score of the game. The int passed is what is added.
    public void UpdateScore(int scoreToAdd)
    {
        // If the current score is greater than the highscore, set the score to the highscore.
        if(score >= highscore)
        {
            score += scoreToAdd;
            highscore = score;
            highscoreText.GetComponent<Text>().text = "Highscore: " + highscore;
        }
        else
        {
            score += scoreToAdd;
        }

        // Gets the Text component and updates it.
        scoreText.GetComponent<Text>().text = "Score: " + score;
    }

    // Available to call to update the current lives the player has.
    // Unlike UpdateScore() this is the number of lives lost.
    public void UpdateLives(int livesLost)
    {
        playerLives -= livesLost;

        // Gets the Text component and updates it.
        livesText.GetComponent<Text>().text = "Lives: " + playerLives;

        // This keeps track of player lives, if less than one.. Game over.
        if(playerLives < 1)
        {
            StartCoroutine(GameEnd());
        }
    }

    public void UpdateHighscore()
    {
        Load();

        highscoreText.GetComponent<Text>().text = "Highscore: " + highscore;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/gameInfo.dat");

        GameData data = new GameData();

        data.highscore = score;

        bf.Serialize(file, data);

        file.Close();
    }

    public void Load()
    {
        if(File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);

            GameData data = (GameData)bf.Deserialize(file);

            highscore = data.highscore;
        }
        else
        {
            Debug.Log("Highscore load failed.");
        }
    }

    public void LoadShipType()
    {
        if (File.Exists(Application.persistentDataPath + "/shipInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/shipInfo.dat", FileMode.Open);

            GameData data = (GameData)bf.Deserialize(file);

            shipTypeIndex = data.shipTypeIndex;
        }
        else
        {
            Debug.Log("Ship type load failed. Attempting to set to default.");
            shipTypeIndex = 1;
        }
    }

    // Function to constantly spawn Saucers into the game.
    IEnumerator SaucerSpawn()
    {
        // Wait before next spawn...
        for(float timer = saucerSpawnRate; timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        // We pick a random corner of the screen..
        int cornerSelection = UnityEngine.Random.Range(0, 4);

        Vector3 saucerSpawnPos = new Vector3(0,0,0);

        // Then depending on the random choice, we spawn the next Saucer there.
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

        // Instantiate a clone object at the previously selected corner.
        GameObject saucerClone = Instantiate(saucerPrefab, saucerSpawnPos, Quaternion.identity) as GameObject;
        saucerClone.GetComponent<Saucer>().SetGameManager(this.gameObject);

        // Then depending on the corner, we rotate the Saucer to prevent it from shooting off into space!
        if (cornerSelection == 0)
        {
            saucerClone.transform.Rotate(Vector3.back * UnityEngine.Random.Range(0, 90));
        }
        else if (cornerSelection == 1)
        {
            saucerClone.transform.Rotate(Vector3.back * UnityEngine.Random.Range(90, 180));
        }
        else if (cornerSelection == 2)
        {
            saucerClone.transform.Rotate(Vector3.back * UnityEngine.Random.Range(180, 270));
        }
        else if (cornerSelection == 3)
        {
            saucerClone.transform.Rotate(Vector3.back * UnityEngine.Random.Range(270, 360));
        }

        // Restart the Coroutine that is already running so Saucers are always respawning.
        StartCoroutine(SaucerSpawn());
    }
    
    IEnumerator GameStart()
    {
        // Set the proper UI's to display.
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        // Set our correct game state.
        state = gameState.game;

        // Update the score and lives in the beginning of the game, or on restart.
        UpdateScore(0);
        UpdateLives(0);
        UpdateHighscore();

        LoadShipType();

        ShipPrefabSet(shipTypeIndex);

        // Spawn the player!
        player = Instantiate(spaceshipPrefab, new Vector3(0, 0, 0), Quaternion.identity) as GameObject;

        // We use a Spaceship object for easier calling to the player's spaceship.
        spaceship = player.GetComponent<Spaceship>();

        ShipTypeSet(shipTypeIndex);

        // Set the GameManager to this object so we can keep a leash on it.
        spaceship.SetGameManager(this.gameObject);

        // "for" loop that spawns the number of Rocks you set into the scene.
        for (int i = 0; i < numStartingRocks; i++)
        {
            // Cosine and sine always output a value from [-1, 1] inclusive.
            // We use that value to generate a random position that is always around the rockSpawnRadius.
            float rockPosX = rockSpawnRadius * Mathf.Cos(UnityEngine.Random.Range(0, 360));
            float rockPosy = rockSpawnRadius * Mathf.Sin(UnityEngine.Random.Range(0, 360));

            // Spawn the Rock prefab!
            GameObject rockClone = Instantiate(startingRockPrefab, new Vector3(rockPosX, rockPosy, 0), Quaternion.identity) as GameObject;

            // Set the GameManager of this object to the GameManager in the game, so we can control this Rock.
            rockClone.GetComponent<Rock>().SetGameManager(this.gameObject);
        }

        // TODO: ScreenSE and ScreenNW are incorrectly named. Change in the future.
        screenSW = Camera.main.ScreenToWorldPoint(new Vector3(0, 0, Camera.main.transform.localPosition.z));
        screenNE = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.localPosition.z));
        screenSE = new Vector3(screenSW.x, screenNE.y, 0);
        screenNW = new Vector3(screenNE.x, screenSW.y, 0);

        // Begin the Coroutines that add a large part of the functionality to the game.
        StartCoroutine(SaucerSpawn());
        StartCoroutine(PowerupSpawn());

        // Since Coroutines always need a "yield return" to know when they are done..
        yield return null;
    }

    IEnumerator GameEnd()
    {
        // Enable / Disable needed UI elements.
        gameUI.SetActive(false);
        gameOverUI.SetActive(true);
        pauseMenuUI.SetActive(false);

        // Set the game state to the game over part of the game.
        state = gameState.gameOver;

        // Update the final score with the current score.
        //finalScoreText.GetComponent<GUIText>().text = "Final Score: " + score;
        finalScoreText.GetComponent<Text>().text = "Final Score: " + score;

        // Destroy this instance of "player" a new one will be spawned on restart.
        Destroy(player);

        // Stop Coroutines that are currently running, SaucerSpawn() and PowerupSpawn() included.
        StopAllCoroutines();

        // TODO: Maybe don't need this?
        if (score >= highscore)
        {
            highscore = score;

            Save();
        }

        // Reset the score values to the original values. They are updated later when GameStart() is called.
        score = startingScore;
        playerLives = startingLives;

        // Yield return to end the Coroutine.
        yield return null;
    }

    IEnumerator PowerupSpawn()
    {
        for (float timer = powerupSpawnRate; timer >= 0; timer -= Time.deltaTime)
        {
            yield return null;
        }

        float powerupPosX = UnityEngine.Random.Range((screenSW.x + 2), (screenNE.x - 2));
        float powerupPosY = UnityEngine.Random.Range((screenSW.y + 2), (screenNE.y - 2));

        int randomPowerUp = UnityEngine.Random.Range(0, 5);

        // These next lines instantiate a prefab based on the random choice made by randomPowerUp.
        // Shield Powerup
        if(randomPowerUp == 0)
        {
            GameObject shieldPowerupClone = Instantiate(shieldPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            shieldPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        // Enhanced Bullet Powerup
        if (randomPowerUp == 1)
        {
            GameObject bulletPowerupClone = Instantiate(bulletPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            bulletPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        // Ship Control Powerup
        if (randomPowerUp == 2)
        {
            GameObject shipControlPowerupClone = Instantiate(shipControlPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            shipControlPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        // Double-Shot Powerup
        if (randomPowerUp == 3)
        {
            GameObject doubleShotPowerupClone = Instantiate(doubleShotPowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            doubleShotPowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        // Life Powerup
        if (randomPowerUp == 4)
        {
            GameObject addLifePowerupClone = Instantiate(addLifePowerupPrefab, new Vector3(powerupPosX, powerupPosY, 0), Quaternion.identity) as GameObject;
            addLifePowerupClone.GetComponent<Powerup>().SetGameManager(this.gameObject);
        }

        // Like the Saucers spawning, we need powerups to continually spawn.
        StartCoroutine(PowerupSpawn());
    }

    void GamePaused()
    {
        // Set the correct UIs to enabled or disabled.
        gameUI.SetActive(false);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(true);

        // This stops the game running by setting the time passing to zero.
        Time.timeScale = 0;

        // Allows you to see the cursor in the game window.
        Cursor.visible = true;
    }

    public void GameUnpaused()
    {
        // Set the correct UIs to visible or hidden.
        gameUI.SetActive(true);
        gameOverUI.SetActive(false);
        pauseMenuUI.SetActive(false);

        // Set time scale to one, which is normal time.
        Time.timeScale = 1;

        // Hide the cursor while normal game is running.
        Cursor.visible = false;
    }

    public void ResumeButtonClick()
    {
        state = gameState.game;
    }

    public void MainMenuButtonClick()
    {
        Application.LoadLevel("MainMenu");
    }
}

[Serializable]
public class GameData
{
    public int highscore;

    public int shipTypeIndex;
}