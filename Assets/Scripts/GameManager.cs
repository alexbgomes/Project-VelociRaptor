using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    static GameManager instance;
    static List<GameObject> enemyGameObjects;
    public static List<GameObject> EnemyGameObjects {
        get {
            return enemyGameObjects;
        } set {
            enemyGameObjects = value;
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

        DontDestroyOnLoad(GameObject.Find("GameManager"));
    }

    void Update() {
        foreach (GameObject gameObject in GameManager.EnemyGameObjects) {
            MonoBehaviour monoBehaviour = gameObject.GetComponent<MonoBehaviour>();
            if (monoBehaviour is PracticeTarget) {
                PracticeTarget practiceTarget = (PracticeTarget)monoBehaviour;
                if (practiceTarget.CanReset && practiceTarget.transform.position.z - GameObject.Find("Spaceship").transform.position.z < -25.0f) {
                    Debug.Log(practiceTarget.transform.position.z - GameObject.Find("Spaceship").transform.position.z);
                    Debug.Log($"{practiceTarget.transform.name} is too far from player, can reset...");
                    practiceTarget.Reset();
                    practiceTarget.gameObject.SetActive(true);
                }
            }
        }
    }


}