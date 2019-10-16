using UnityEngine;
using System;
using System.Collections.Generic;
using SimpleTCP;
using System.Net;

public class Game : MonoBehaviour {

    private enum Phase { Connecting, Waiting, Intro, InGame, Outro }

    public static Game Instance;

    public float GameLength;

    public bool AutoRevive;

    public float AutoReviveTime;

    public float PowerPelletActiveTime;

    public float PowerPelletRechargeTime;

    public float MagnetThreshold;

    [SerializeField] ScoreBoard scoreBoard;

    [SerializeField] Lights lights;

    [SerializeField] SFX sfx;

    private Phase phase = Phase.Connecting;

    private List<Player> players = new List<Player>();

    private float gameStartTime;

    private Player.Type chaseType;

    private float chaseSwitchTime;

    private bool powerPelletCharged;

    private bool receivedSensorData;

    private SimpleTcpServer tcpServer = new SimpleTcpServer();

    private void Awake() {
        Game.Instance = this;
        
        this.tcpServer.Start(IPAddress.Any, 8080);
        this.tcpServer.DataReceived += this.HandleTCPData;
    }

    private void OnDestroy() {
        this.tcpServer.Stop();
    }

    private void Update() {
        if (this.phase == Phase.Connecting) {
            int controllerCount = UniMoveController.GetNumConnected();
            bool isTesting = false;
            if (controllerCount == 0) {
                controllerCount = 6;
                isTesting = true;
            }
            for (int i=players.Count; i<controllerCount; i++) {
                GameObject go = new GameObject("Player");
                go.transform.SetParent(this.transform);
                EnhancedMoveController controller = !isTesting ? go.AddComponent<EnhancedMoveController>() : null;
                if (controller == null || controller.Init(i, this.MagnetThreshold)) {
                    Player player = go.AddComponent<Player>();
                    player.OnKill += this.HandlePlayerKill;
                    player.Init(controller, i < 3 ? Player.Type.Pac : Player.Type.Ghost);
                    this.players.Add(player);
                } else {
                    Destroy(go);
                }
            }
            Debug.Log("players connected: " + players.Count + " of " + controllerCount);
            if (players.Count == controllerCount) {
                Debug.Log("connected!");
                this.phase = Phase.Waiting;
            }
        }

        if (this.phase == Phase.Waiting) {
            if (Input.GetKeyDown(KeyCode.Space)) {
                this.StartGame();
                return;
            }
        }

        if (this.phase == Phase.InGame) {
            if (Time.time >= this.gameStartTime + this.GameLength) {
                this.EndGame();
                return;
            }

            if (this.chaseType == Player.Type.Ghost) {
                // process sensor data received this frame
                if (this.receivedSensorData && this.powerPelletCharged) {
                    this.SetChasing(Player.Type.Pac);
                    this.SetPowerPelletCharged(false);
                }
                if (!this.powerPelletCharged && Time.time >= this.chaseSwitchTime + this.PowerPelletRechargeTime) {
                    this.SetPowerPelletCharged(true);
                }
            } else if (this.chaseType == Player.Type.Pac) {
                if (Time.time >= this.chaseSwitchTime + this.PowerPelletActiveTime) {
                    this.SetChasing(Player.Type.Ghost);
                }
            }

            // hotkeys
            if (Input.GetKeyDown(KeyCode.Alpha1)) {
                this.HandlePlayerKill(Player.Type.Ghost);
            } else if (Input.GetKeyDown(KeyCode.Alpha2)) {
                this.HandlePlayerKill(Player.Type.Pac);
            } else if (Input.GetKeyDown(KeyCode.Alpha3)) {
                if (this.powerPelletCharged) {
                    this.SetChasing(Player.Type.Pac);
                    this.SetPowerPelletCharged(false);
                }
            } 
        }

        // debug tools
        if (Input.GetKeyDown(KeyCode.L)) {
            this.lights.ToggleDebug();
        }

        // clear sensor data every frame
        this.receivedSensorData = false;
    }

    private void StartGame() {
        Debug.Log("starting game");
        this.phase = Phase.InGame;
        this.gameStartTime = Time.time;
        this.SetChasing(Player.Type.Ghost);
        this.SetPowerPelletCharged(true);
        this.scoreBoard.SetGameStart(this.gameStartTime);
        this.lights.SetGameStart();
        this.sfx.SetGameStart();
    }

    private void EndGame() {
        Debug.Log("ending game");
        this.phase = Phase.Waiting;
        foreach (Player player in this.players) {
            player.Reset();
        }
        this.scoreBoard.SetGameEnd();
        this.lights.SetGameEnd();
        this.sfx.SetGameEnd();
    }

    private void SetChasing(Player.Type type) {
        Debug.Log("set chasing: " + type);
        this.chaseType = type;
        this.chaseSwitchTime = Time.time;
        foreach (Player player in this.players) {
            if (player.type != type) {
                player.SetHittable(true);
            } else {
                player.SetHittable(false);
            }
        }
        this.scoreBoard.SetChasing(type);
        this.lights.SetChasing(type);
        this.sfx.SetChasing(type);
    }

    private void SetPowerPelletCharged(bool charged) {
        Debug.Log("power pellet charged: " + charged);
        this.powerPelletCharged = charged;
        this.lights.SetPowerPelletCharged(charged);
        this.sfx.SetPowerPelletCharged(charged);
    } 

    private void HandlePlayerKill(Player player) {
        Debug.Log("player killed: "  + player.type);
        this.HandlePlayerKill(player.type);
    }

    private void HandlePlayerKill(Player.Type type) {
        this.scoreBoard.SetKill(type);
        this.lights.SetKill(type);
        this.sfx.SetKill(type);
    }

    private void HandleTCPData(object sender, Message message) {
        Debug.Log("Sensor hit!");
        this.receivedSensorData = true;
    }

}