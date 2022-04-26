using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
    static GameManager instance;
    static List<GameObject> enemyGameObjects;

    static GameObject spaceship;
    public static GameObject pickupShieldPrefab;
    public static GameObject pickupInvulPrefab;
    public static GameObject pickupMultiplierPrefab;
    public static Material skyboxMaterial;
    public static float PlayerMovespeed;
    public static bool PlayerMoving;
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
            return 150.0f;
        }
    }

    public static float MaxYBoundary {
        get {
            return 35.0f;
        }
    }

    public static float MaxZAwayFromPlayer {
        get {
            return -25.0f;
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

        GameManager.LoadResources();

        GameManager.PlayerMovespeed = GameManager.Spaceship.GetComponent<SpaceshipController>().moveSpeed;
        GameManager.PlayerMoving = GameManager.Spaceship.GetComponent<SpaceshipController>().moving;

        DontDestroyOnLoad(GameObject.Find("GameManager"));
        Debug.Log("GameManager initialized.");
    }

    public static void LoadResources() {
        pickupShieldPrefab = Resources.Load<GameObject>("Prefabs/Shield Pickup");
        pickupInvulPrefab = Resources.Load<GameObject>("Prefabs/Invul Pickup");
        pickupMultiplierPrefab = Resources.Load<GameObject>("Prefabs/Multiplier Pickup");

        skyboxMaterial = Resources.Load<Material>("Skybox Materials/PlanetaryEarth4k");
    }

    void Update() {
        foreach (GameObject gameObject in GameManager.EnemyGameObjects.ToList()) { //ToList ensures mutation does not throw error
            MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
            if (monoBehaviour is Enemy) {
                if (monoBehaviour is PracticeTarget) {
                    PracticeTarget practiceTarget = (PracticeTarget)monoBehaviour;
                    if (practiceTarget.CanReset && CheckOutOfBounds(practiceTarget)) {
                        Debug.Log(practiceTarget.transform.position.z - GameManager.Spaceship.transform.position.z);
                        Debug.Log($"{practiceTarget.transform.name} is too far from player, can reset...");
                        practiceTarget.Reset();
                        practiceTarget.gameObject.SetActive(true);
                    }
                } else if (monoBehaviour is PickupsController) {
                    PickupsController pickupsController = (PickupsController)monoBehaviour;
                    if (CheckOutOfBounds(pickupsController)) {
                        Destroy(pickupsController.gameObject);
                    }
                } else if (monoBehaviour is Enemy) {
                    Enemy enemy = (Enemy)monoBehaviour;
                    if (CheckOutOfBounds(enemy)) {
                        Debug.Log(enemy.transform.position.z - GameManager.Spaceship.transform.position.z);
                        Debug.Log($"{enemy.transform.name} is too far from player, killing without increasing score...");
                        enemy.TakeDamage(9999, GameManager.Instance.gameObject);

                    }
                }
            }
        }
        if (!GameManager.LevelQueued && GameManager.CurrentLevelScore.Sum() >= LevelData.getScore(CurrentLevel)) {
            GameManager.LevelQueued = true;
            Debug.Log("Level Passed!");
            SpaceshipController spaceshipController = Spaceship.GetComponent<SpaceshipController>();
            spaceshipController.StartWarpDrive();
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

    bool CheckOutOfBounds(Transform transform) {
        return transform.position.z - GameManager.Spaceship.transform.position.z < MaxZAwayFromPlayer;
    }

    bool CheckOutOfBounds(GameObject gameObject) {
        return CheckOutOfBounds(gameObject.transform);
    }

    bool CheckOutOfBounds(MonoBehaviour script) {
        return CheckOutOfBounds(script.transform);
    }


}