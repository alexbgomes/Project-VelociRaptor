using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class SaveSystem {
    static string savePath = Application.persistentDataPath + "/sav.dat";
    public static void Save() {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
            SaveData data = new SaveData();
            formatter.Serialize(stream, data);
        }
    }

    public static SaveData Load() {
        if (File.Exists(savePath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Open)) {
                SaveData data = (SaveData)formatter.Deserialize(stream);
                formatter.Serialize(stream, data);
                return data;
            }
        }
        return null;
    }

    public static void Delete() {
        if (File.Exists(savePath)) {
            File.Delete(savePath);
        }
    }
}