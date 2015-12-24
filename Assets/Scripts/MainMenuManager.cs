using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    public AudioClip shipSelectedSfx;

    public GameObject mainMenuUI;
    public GameObject hangarUI;

    public GameObject galacticCreditsTextUI;

    private AudioSource audioSource;

    private int currentGalacticCredits = 0;

    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    public void MainMenuLoad()
    {
        if (File.Exists(Application.persistentDataPath + "/gameInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/gameInfo.dat", FileMode.Open);

            GameData data = (GameData)bf.Deserialize(file);

            currentGalacticCredits = data.galacticCredits;
        }
        else
        {
            Debug.Log("Credits load failed.");
        }
    }

    public void PlayButtonClick()
    {
        audioSource.PlayOneShot(shipSelectedSfx);
        Application.LoadLevel("Game");
    }

    public void QuitButtonClick()
    {
        audioSource.PlayOneShot(shipSelectedSfx);
        Application.Quit();
    }

    public void HangarButtonClick()
    {
        audioSource.PlayOneShot(shipSelectedSfx);
        mainMenuUI.SetActive(false);
        hangarUI.SetActive(true);

        MainMenuLoad();

        galacticCreditsTextUI.GetComponent<Text>().text = "Current Galactic Credits: " + currentGalacticCredits;
    }

    public void HangarBackToMainButtonClick()
    {
        audioSource.PlayOneShot(shipSelectedSfx);
        mainMenuUI.SetActive(true);
        hangarUI.SetActive(false);
    }

    public void DefaultShipChoice()
    {
        ShipTypeSave(1);
        audioSource.PlayOneShot(shipSelectedSfx);
    }

    public void SpeedyShipChoice()
    {
        ShipTypeSave(2);
        audioSource.PlayOneShot(shipSelectedSfx);
    }

    public void BlazeShipChoice()
    {
        ShipTypeSave(3);
        audioSource.PlayOneShot(shipSelectedSfx);
    }

    public void ShipTypeSave(int shipTypeIndex)
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/shipInfo.dat");
        GameData data = new GameData();

        switch (shipTypeIndex)
        {
            case 1:
                data.shipTypeIndex = 1;
                break;

            case 2:
                data.shipTypeIndex = 2;
                break;

            case 3:
                data.shipTypeIndex = 3;
                break;
        }

        bf.Serialize(file, data);

        file.Close();
    }
}
