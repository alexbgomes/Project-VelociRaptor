[System.Serializable]
public class SaveData {

    private int currentLevel = 1;
    public int CurrentLevel {
        get {
            return currentLevel;
        }
    }

    private int totalKills = 0;
    public int TotalKills {
        get {
            return totalKills;
        }
    }
    public SaveData() {

    }
}
