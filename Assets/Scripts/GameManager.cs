using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    static GameManager instance;
    static List<GameObject> enemyGameObjects;

    static GameObject spaceship;
    public static GameObject Spaceship {
        get {
            return spaceship;
        } set {
            spaceship = value;
        }
    }

    public static List<GameObject> EnemyGameObjects {
        get {
            return enemyGameObjects;
        } set {
            enemyGameObjects = value;
        }
    }

    static List<int> currentLevelScore;
    public static List<int> CurrentLevelScore {
        get {
            return currentLevelScore;
        } set {
            currentLevelScore = value;
        }
    }

    static int currentLevel = 1;
    public static int CurrentLevel {
        get {
            return currentLevel;
        } set {
            currentLevel = value;
            SaveSystem.Save();
        }
    }

    static bool levelQueued = false;
    public static bool LevelQueued {
        get {
            return levelQueued;
        } set {
            levelQueued = value;
        }
    }

    static int totalKills = 0;
    public static int TotalKills {
        get {
            return totalKills;
        } set {
            totalKills = value;
            SaveSystem.Save();
        }
    }

    public static float MaxXBoundary {
        get {
            return 25.0f;
        }
    }

    public static GameManager Instance {
        get {
            if (!instance) {
                GameObject gameObject = new GameObject("GameManager");
                gameObject.AddComponent<GameManager>();
            }

            return instance;
        }
    }

    void Awake() {
        instance = this;
        SaveData saveData = SaveSystem.Load();

        if (saveData is null) {
            SaveSystem.Save();
        } else {
            // load data into RAM; basically set GameManager variables from save data
            GameManager.CurrentLevel = saveData.CurrentLevel;
        }

        GameManager.EnemyGameObjects = new List<GameObject>();
        GameManager.CurrentLevelScore = new List<int>();
        GameManager.Spaceship = GameObject.Find("Spaceship");

        DontDestroyOnLoad(GameObject.Find("GameManager"));
        Debug.Log("GameManager initialized.");
    }

    void Update() {
        foreach (GameObject gameObject in GameManager.EnemyGameObjects) {
            MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
            if (monoBehaviour is PracticeTarget) {
                PracticeTarget practiceTarget = (PracticeTarget)monoBehaviour;
                if (practiceTarget.CanReset && practiceTarget.transform.position.z - GameManager.Spaceship.transform.position.z < -25.0f) {
                    Debug.Log(practiceTarget.transform.position.z - GameManager.Spaceship.transform.position.z);
                    Debug.Log($"{practiceTarget.transform.name} is too far from player, can reset...");
                    practiceTarget.Reset();
                    practiceTarget.gameObject.SetActive(true);
                }
            }
        }
        if (!GameManager.LevelQueued && GameManager.CurrentLevelScore.Sum() > LevelData.getScore(CurrentLevel)) {
            GameManager.LevelQueued = true;
            Debug.Log("Level Passed!");
            Invoke("NextScene", 5.0f);
        }
    }

    void NextScene() {
        GameObject spaceship = GameManager.Spaceship;

        EnemyGameObjects = new List<GameObject>();
        GameManager.CurrentLevelScore = new List<int>();
        GameManager.levelQueued = false;
        Vector3 position = spaceship.transform.position;
        position = Vector3.up * 8;
        spaceship.transform.position = position;

        GameManager.CurrentLevel = GameManager.CurrentLevel + 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        
        // Post load
        BulletPool spaceshipBulletPool = spaceship.GetComponent<BulletPool>();
        spaceshipBulletPool.ReadyPool();

    }


}