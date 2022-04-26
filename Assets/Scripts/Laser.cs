using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour {
    public int damage;
    public GameObject Source { get; set; }
    public LaserOrigin laserOrigin;

    void Start() {
        tag = laserOrigin.ToString();
    }

}

public enum LaserOrigin {
    PlayerLaser,
    EnemyLaser
}