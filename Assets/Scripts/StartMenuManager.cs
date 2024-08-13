using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using TMPro;
using System.IO;

[System.Serializable]
public class SaveData
{
    public string namePlayer;
    public int highScore;
}

[DefaultExecutionOrder(1000)]
public class StartMenuManager : MonoBehaviour
{
    public GameObject StartButton;
    public GameObject EnterButton;
    public TMP_InputField nameInput;
    public TMP_Text nameText;
    public string namePlayer;
    public int highScore;

    public void Start()
    {
        EnterButton.SetActive(true);
    }

    public void EnterName()
    {
        string inputValue = nameInput.text;
        if (string.IsNullOrEmpty(inputValue))
        {
            Debug.Log("Enter your name to continute.");
        }
        else
        {
            EnterButton.SetActive(false);
            StartButton.SetActive(true);

            LoadData();

            // Debug.Log(namePlayer);
        }

    }

    public void StartNew()
    {
        SceneManager.LoadScene(1);
    }

    public void Exit()
    {
#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit(); // original code to quit Unity player

#endif
    }

    public void LoadData()
    {
        namePlayer = nameInput.text;
        PlayerPrefs.SetString("name", namePlayer);

        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);

            foreach (var data in playerDataList.dataList)
            {
                // Debug.Log(data.namePlayer);
                // Debug.Log(data.highScore);
                if (data.namePlayer == namePlayer)
                {
                    highScore = data.highScore;
                    nameText.text = $"{data.namePlayer} - High Score: {data.highScore}";
                    break;
                }
                else
                {
                    nameText.text = namePlayer;
                }
            }
        }
        else
        {
            // Debug.Log("No save data found.");
            nameText.text = namePlayer;
        }
    }
}
