using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;
using DP;

public class LightFixture : MonoBehaviour {
    private const int R_CHANNEL = 0;
    private const int G_CHANNEL = 1;
    private const int B_CHANNEL = 2;
    private const int W_CHANNEL = 3;
    private const int INTENSITY_CHANNEL = 4;
    public const int FIXTURE_CHANNELS_TOTAL = 5; 

    public Image image;
    public float r;
    public float g;
    public float b;
    public float w;
    public float intensity;
    public int startChannel;
    public DMX dmx;

    void Update() {
        if (this.dmx) {
            this.dmx[startChannel + R_CHANNEL] = Mathf.RoundToInt(this.r * 255);
            this.dmx[startChannel + G_CHANNEL] = Mathf.RoundToInt(this.g * 255);
            this.dmx[startChannel + B_CHANNEL] = Mathf.RoundToInt(this.b * 255);
            this.dmx[startChannel + W_CHANNEL] = Mathf.RoundToInt(this.w * 255);
            this.dmx[startChannel + INTENSITY_CHANNEL] = Mathf.RoundToInt(this.intensity * 255);
        }

        if (w != 0) {
            this.image.color = new Color(w * intensity, w * intensity, w * intensity);
        } else {
            this.image.color = new Color(r * intensity, g * intensity, b * intensity);
        }
    }

    public void SetColor(Color color, float intensity = 1) {
        this.r = color.r;
        this.g = color.g;
        this.b = color.b;
        this.w = 0;
        this.intensity = intensity;
    }

    public void SetWhite(float w, float intensity = 1) {
        this.r = 0;
        this.g = 0;
        this.b = 0;
        this.w = w;
        this.intensity = intensity;
    }
}