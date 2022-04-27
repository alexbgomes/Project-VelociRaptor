using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR;
public class GameManager : MonoBehaviour {
    static GameManager instance;
    static List<GameObject> enemyGameObjects;

    static GameObject destroyer;
    static GameObject spaceship;
    public static GameObject pickupShieldPrefab;
    public static GameObject pickupInvulPrefab;
    public static GameObject pickupMultiplierPrefab;
    public static Material skyboxMaterial;
    public static float PlayerMovespeed;
    public static bool PlayerMoving;
    public static bool playerNeedsReset;
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

    static int currentLevel = 0;
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

    public static float MaxZToPlayerForSpawn {
        get {
            return 150.0f;
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

        if (SceneManager.GetActiveScene().buildIndex != 0)
        {
            GameManager.Spaceship = GameObject.Find("Spaceship");
            destroyer = GameObject.Find("AlienDestroyer");
            GameManager.LoadResources();

            GameManager.PlayerMovespeed = GameManager.Spaceship.GetComponent<SpaceshipController>().moveSpeed;
            GameManager.PlayerMoving = GameManager.Spaceship.GetComponent<SpaceshipController>().moving;
            playerNeedsReset = false;
        }
        

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
        SpaceshipController spaceshipController;
        if (GameManager.Spaceship != null) {
            spaceshipController = GameManager.Spaceship.GetComponent<SpaceshipController>();
        } else {
            spaceshipController = null;
            return;
        }
        
        foreach (GameObject gameObject in GameManager.EnemyGameObjects.ToList()) { //ToList ensures mutation does not throw error
            if (!spaceshipController.IsAlive) {
                break;
            }
            if (gameObject == null) {
                continue;
            }
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

                    if (CheckSpawnable(enemy) && !enemy.gameObject.activeInHierarchy) {
                        enemy.gameObject.SetActive(true);
                        Debug.Log($"GameManager has spawned {enemy.gameObject.name}...");
                    }

                    if (CheckOutOfBounds(enemy)) {
                        Debug.Log($"{enemy.transform.name} is too far from player, killing without increasing score...");
                        enemy.TakeDamage(9999, GameManager.Instance.gameObject);

                    }
                }
            }
        }

        if (!GameManager.LevelQueued && GetCurrentScore() >= LevelData.getScore(CurrentLevel)) {
            GameManager.LevelQueued = true;
            Debug.Log("Level Passed!");
            spaceshipController.StartWarpDrive();
            Invoke("NextScene", 5.0f);
        }

        if (!spaceshipController.IsAlive && !playerNeedsReset) {
            Debug.Log("Player died... Warping!");
            playerNeedsReset = true;
            spaceshipController.moving = false;
            foreach (GameObject gameObject in GameManager.EnemyGameObjects.ToList()) {
                if (gameObject == null) {
                    continue;
                }
                MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
                if (monoBehaviour is Enemy) {
                    gameObject.SetActive(false);
                }
            }
            spaceshipController.StartWarpDrive();
            Invoke("ShowDeathScreen", 2.5f);
        }

        if(spaceshipController.IsAlive && destroyer == null)
        {
            spaceshipController.StartWarpDrive();
            Invoke("ShowWinScreen", 2.5f);
        }
    }

    void ShowDeathScreen() {
        Spaceship.GetComponent<SpaceshipController>().ShowDeathScreen();
    }

    void ShowWinScreen()
    {
        if (GameManager.Spaceship != null)
        {
            Spaceship.GetComponent<SpaceshipController>().ShowWinScreen();
        }
    }

    public int GetCurrentScore() {
        return GameManager.CurrentLevelScore.Sum();
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

    public void ReloadScene() {
        SceneManager.LoadScene(0);
        Destroy(GameManager.Spaceship);
        instance.Invoke("SceneFadeIn", 1.0f);
        Destroy(GameManager.instance);
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

    bool CheckSpawnable(Transform transform) {
        return transform.position.z - GameManager.Spaceship.transform.position.z <= MaxZToPlayerForSpawn;
    }

    bool CheckSpawnable(GameObject gameObject) {
        return CheckSpawnable(gameObject.transform);
    }

    bool CheckSpawnable(MonoBehaviour script) {
        return CheckSpawnable(script.transform);
    }

    public static void LoadMainGameClick() {
        //SteamVR_Fade.View(new Color(0, 0, 0), 1.0f);
        EnemyGameObjects = new List<GameObject>();
        GameManager.CurrentLevelScore = new List<int>();
        GameManager.levelQueued = false;
        GameManager.CurrentLevel = GameManager.CurrentLevel + 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        //Invoke("SceneFadeIn", 1.0f);
    }
    public void SceneFadeIn() {
        SteamVR_Fade.View(Color.clear, 1.0f);
    }
}