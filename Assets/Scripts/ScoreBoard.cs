using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using TMPro;

public class ScoreBoard : MonoBehaviour {

    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] TextMeshProUGUI pacScoreText;

    [SerializeField] TextMeshProUGUI ghostScoreText;

    [SerializeField] ScoreBoardCharacter pacCharacter;

    [SerializeField] ScoreBoardCharacter ghostCharacter;

    int ghostScore;

    int pacScore;

    float gameStartTime;

    bool inGame;

    void Update() {
        if (this.inGame) {
            int timeRemaining = (int)Mathf.Clamp(this.gameStartTime + Game.Instance.GameLength - Time.time, 0, Game.Instance.GameLength);
            this.timeText.text = string.Format("{0:D1}:{1:D2}", timeRemaining / 60, timeRemaining % 60);
        }
    }

    public void Reset() {
        this.SetPacScore(0);
        this.SetGhostScore(0);
        this.pacCharacter.Reset();
        this.ghostCharacter.Reset();
        this.inGame = false;
    }

    public void SetGameStart(float gameStartTime) {
        this.pacCharacter.StartGame();
        this.ghostCharacter.StartGame();
        this.inGame = true;
    }

    public void SetGameEnd() {
        this.Reset();
    }

    public void SetChasing(Player.Type type) {
        this.pacCharacter.Switch(type);
        this.ghostCharacter.Switch(type);
    }

    public void SetPowerPelletCharged(bool charged) {

    }

    public void SetKill(Player.Type type) {
        if (type == Player.Type.Ghost) {
            this.ghostCharacter.Hit();
            this.SetPacScore(this.pacScore + 1);
        } else {
            this.pacCharacter.Hit();
            this.SetGhostScore(this.ghostScore + 1);
        }
    }

    void SetGhostScore(int score) {
        this.ghostScore = score;
        this.ghostScoreText.text = this.ghostScore.ToString();
    }

    void SetPacScore(int score) {
        this.pacScore = score;
        this.pacScoreText.text = this.pacScore.ToString();
    }

}