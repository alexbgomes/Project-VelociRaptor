using UnityEngine;

public class GameManager : MonoBehaviour {
    static GameManager instance;

    static int currentLevel = 1;
    public static int CurrentLevel {
        get {
            return sessionCurrentLevel;
        } set {
            sessionCurrentLevel = value;
            SaveSystem.Save();
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

        if (!saveData) {
            SaveSystem.Save();
        } else {
            // load data into RAM; basically set GameManager variables from save data
            GameManager.CurrentLevel = saveData.CurrentLevel;
        }

        DontDestroyOnLoad(GameObject.Find("GameManager"));
    }


}