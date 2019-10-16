using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DP;

public class Lights : MonoBehaviour {

    private const int FIXTURE_COUNT = 16;
    private static List<int> POWER_PELLET_FIXTURES = new List<int>(){0, 4, 11, 12};


    private enum State {Idle, Intro, GhostsChasing, PacsChasing, GhostKilled, PacKilled, Outro};

    private List<LightFixture> mainFixtures = new List<LightFixture>();
    private List<LightFixture> powerPelletFixtures = new List<LightFixture>();
    private State state;
    private float stateStartTime;
    private bool powerPelletState;
    private float powerPelletStateStartTime;
    [SerializeField] DMX dmx;
    [SerializeField] LightFixture fixturePrefab;
    [SerializeField] Transform fixtureDebugContainer;

    void Start() {
        for (int i=0; i<=FIXTURE_COUNT; i++) {
            LightFixture fixture = GameObject.Instantiate<LightFixture>(this.fixturePrefab);
            fixture.transform.SetParent(this.fixtureDebugContainer, false);
            fixture.startChannel = 1 + i * LightFixture.FIXTURE_CHANNELS_TOTAL;
            fixture.dmx = this.dmx;
            if (POWER_PELLET_FIXTURES.Contains(i)) {
                this.powerPelletFixtures.Add(fixture);
            } else {
                this.mainFixtures.Add(fixture);
            }
        }
    }

    void Update() {
        for (int i=0; i<this.mainFixtures.Count; i++) {
            LightFixture fixture = this.mainFixtures[i];
            float wave = Mathf.Sin(i + Time.time)/2 + 0.5f;
            if (this.state == State.Idle) {
                fixture.SetWhite(1);
            } else if (this.state == State.GhostsChasing) {
                fixture.SetColor(new Color(0.8f + 0.2f*wave, 1, 0));
            } else if (this.state == State.PacsChasing) {
                fixture.SetColor(new Color(1, 0, 0.3f*wave));     
            }
        }

        foreach (LightFixture fixture in this.powerPelletFixtures) {
            if (this.powerPelletState) {
                fixture.SetWhite(1);
            } else {
                fixture.SetWhite(0);
            }
        }
    }

    public void ToggleDebug() {
        this.fixtureDebugContainer.gameObject.SetActive(!this.fixtureDebugContainer.gameObject.activeSelf);
    }

    public void Reset() {
    }

    public void SetGameStart() {

    }

    public void SetGameEnd() {
        this.Reset();
    }

    public void SetChasing(Player.Type type) {
        if (type == Player.Type.Ghost) {
            this.state = State.GhostsChasing;
        } else if (type == Player.Type.Pac) {
            this.state = State.PacsChasing;
        }
    }

    public void SetPowerPelletCharged(bool charged) {
        this.powerPelletState = charged;
        this.powerPelletStateStartTime = Time.time;
    }

    public void SetKill(Player.Type type) {

    }




}