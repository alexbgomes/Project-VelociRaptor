using System.Collections;
using UnityEngine;

public class Laser : MonoBehaviour {
    public int damage;
    public GameObject Source { get; set; }
    public LaserOrigin laserOrigin;
    private AudioSource laserSound;

    void Start() {
        tag = laserOrigin.ToString();
        laserSound = GetComponent<AudioSource>();
        laserSound.Play();
    }

}

public enum LaserOrigin {
    PlayerLaser,
    EnemyLaser
}