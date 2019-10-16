using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class SFX : MonoBehaviour {

    [SerializeField] AudioSource music;
    [SerializeField] AudioSource pacsChasing;
    [SerializeField] AudioSource killGhost;
    [SerializeField] AudioSource killPac;
    [SerializeField] AudioSource powerPelletUsed;
    [SerializeField] AudioSource powerPelletRecharged;

    private float defaultMusicVolume;

    void Start() {
        this.defaultMusicVolume = this.music.volume;
    }

    public void Reset() {
    }

    public void SetGameStart() {
        this.music.time = 0;
        this.music.Play();
    }

    public void SetGameEnd() {
        this.Reset();
    }

    public void SetChasing(Player.Type type) {
        if (type == Player.Type.Ghost) {
            this.pacsChasing.Stop();
            this.music.volume = this.defaultMusicVolume;
        } else if (type == Player.Type.Pac) {
            this.pacsChasing.time = 0;
            this.pacsChasing.Play();
            this.music.volume = this.defaultMusicVolume / 2;
        }
    }

    public void SetPowerPelletCharged(bool charged) {
        if (charged) {
            this.powerPelletRecharged.time = 0;
            this.powerPelletRecharged.Play();
        } else {
            this.powerPelletUsed.time = 0;
            this.powerPelletUsed.Play();
        }
    }

    public void SetKill(Player.Type type) {
        if (type == Player.Type.Pac) {
            this.killPac.time = 0;
            this.killPac.Play();
        } else {
            this.killGhost.time = 0;
            this.killGhost.Play();
        }
    }

}