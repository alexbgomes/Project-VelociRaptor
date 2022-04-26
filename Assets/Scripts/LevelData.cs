using System.Collections.Generic;

public static class LevelData {

    public static int Count {
        get {
            return scores.Count;
        } 
    }

    private static List<int> scores = new List<int>(){
        500,
        9999
    };

    public static int getScore(int level) {
        return scores[level];
    }
}