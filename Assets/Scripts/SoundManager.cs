using UnityEngine;

public class SoundManager : MonoBehaviour {

    private static SoundManager instance;

    AudioSource musicSource;

    public static SoundManager Instance {
        get {
            if (!instance) {
                GameObject gameObject = new GameObject("SoundManager");
                gameObject.AddComponent<GameManager>();
            }

            return instance;
        }
    }

    void Awake() {
        //set the instance variable to this instance for the sake of retrieval/checking
        instance = this;

        DontDestroyOnLoad(GameObject.Find("SoundManager"));
    }

    public enum Track {

    }

}