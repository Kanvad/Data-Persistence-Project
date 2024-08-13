using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;


public class MainManager : MonoBehaviour
{
    public Brick BrickPrefab;
    public int LineCount = 6;
    public Rigidbody Ball;

    public Text ScoreText;
    public Text bestScore;
    public GameObject GameOverText;

    private bool m_Started = false;
    private int m_Points;

    private bool m_GameOver = false;
    public string namePlayer;
    public int highScore;
    public List<SaveData> playerDataList = new List<SaveData>();
    private string highestScoringPlayer;
    private int highestScore;


    // Start is called before the first frame update
    void Start()
    {
        const float step = 0.6f;
        int perLine = Mathf.FloorToInt(4.0f / step);

        int[] pointCountArray = new[] { 1, 1, 2, 2, 5, 5 };
        for (int i = 0; i < LineCount; ++i)
        {
            for (int x = 0; x < perLine; ++x)
            {
                Vector3 position = new Vector3(-1.5f + step * x, 2.5f + i * 0.3f, 0);
                var brick = Instantiate(BrickPrefab, position, Quaternion.identity);
                brick.PointValue = pointCountArray[i];
                brick.onDestroyed.AddListener(AddPoint);
            }
        }

        namePlayer = PlayerPrefs.GetString("name");
        // Debug.Log(namePlayer);

        FindHighestScore();
        bestScore.text = $"Best Score: {highestScoringPlayer} - {highestScore}";
        // Debug.Log(highestScoringPlayer);
        // Debug.Log(highestScore);
    }

    private void Update()
    {
        if (!m_Started)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                m_Started = true;
                float randomDirection = Random.Range(-1.0f, 1.0f);
                Vector3 forceDir = new Vector3(randomDirection, 1, 0);
                forceDir.Normalize();

                Ball.transform.SetParent(null);
                Ball.AddForce(forceDir * 2.0f, ForceMode.VelocityChange);
            }
        }
        else if (m_GameOver)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                // SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                SceneManager.LoadScene(0);
                highScore = m_Points;
                SaveData();
            }
        }


    }

    void AddPoint(int point)
    {
        m_Points += point;
        ScoreText.text = $"Score : {m_Points}";
    }

    public void GameOver()
    {
        m_GameOver = true;
        GameOverText.SetActive(true);
    }

    public void FindHighestScore()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            PlayerDataList playerDataList = JsonUtility.FromJson<PlayerDataList>(json);

            highestScore = 0;

            foreach (var data in playerDataList.dataList)
            {
                if (data.highScore > highestScore)
                {
                    highestScore = data.highScore;
                    highestScoringPlayer = data.namePlayer;
                }
            }
        }
        else
        {
            Debug.Log("No save data found.");
        }
    }

    public void SaveData()
    {
        string path = Application.persistentDataPath + "/savefile.json";
        PlayerDataList playerDataList = new PlayerDataList();

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            playerDataList = JsonUtility.FromJson<PlayerDataList>(json);
        }

        bool playerExists = false;
        foreach (var data in playerDataList.dataList)
        {
            if (data.namePlayer == namePlayer)
            {
                playerExists = true;

                if (highScore > data.highScore)
                {
                    data.highScore = highScore;
                }
                break;
            }
        }

        if (!playerExists)
        {
            SaveData newData = new SaveData();
            newData.namePlayer = namePlayer;
            newData.highScore = highScore;
            playerDataList.dataList.Add(newData);
        }

        string newJson = JsonUtility.ToJson(playerDataList);
        File.WriteAllText(path, newJson);
    }

}

[System.Serializable]
public class PlayerDataList
{
    public List<SaveData> dataList = new List<SaveData>();
}
