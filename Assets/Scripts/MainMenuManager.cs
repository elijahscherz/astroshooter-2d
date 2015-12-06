using UnityEngine;
using System.Collections;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class MainMenuManager : MonoBehaviour {

    public AudioClip shipSelectedSfx;

    public GameObject mainMenuUI;
    public GameObject hangarUI;

    private AudioSource audioSource;

	void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }
	public void PlayButtonClick()
    {
        Application.LoadLevel("Game");
    }

    public void QuitButtonClick()
    {
        Application.Quit();
    }

    public void HangarButtonClick()
    {
        mainMenuUI.SetActive(false);
        hangarUI.SetActive(true);
    }

    public void HangarBackToMainButtonClick()
    {
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

        switch(shipTypeIndex)
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
