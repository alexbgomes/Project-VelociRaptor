using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public static class SaveSystem {
    const string savePath = Application.persistent + "/sav.dat";
    public static void Save() {
        BinaryFormatter formatter = new BinaryFormatter();
        using (FileStream stream = new FileStream(savePath, FileMode.Create)) {
            data = new SaveData();
            BinaryFormatter.Serialize(stream, data);
        }
    }

    public static SaveData Load() {
        if (File.Exists(savePath)) {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(savePath, FileMode.Open)) {
                SaveData data = (SaveData)formatter.Deserialize(stream);
                BinaryFormatter.Serialize(stream, data);
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