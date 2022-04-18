using System.Collections.Generic;

public static class LevelData {

    public static int Count {
        get {
            return scores.Count;
        } 
    }

    private static List<int> scores = new List<int>(){
        5,
        9999
    };

    /** This is expected to be 1-index */
    public static int getScore(int level) {
        return scores[level - 1];
    }
}