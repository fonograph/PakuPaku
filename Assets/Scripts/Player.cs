using UnityEngine;
using System;

public class Player : MonoBehaviour {

    public enum Type {Pac, Ghost};

    public Action<Player> OnKill;
    public Action<Player> OnRevive;

    private EnhancedMoveController controller;
    public Type type {get; private set;}
    private bool hittable;
    private bool alive;
    private float reviveTime;

    public void Init(EnhancedMoveController controller, Type defaultType) {
        this.controller = controller;
        this.type = defaultType;
        if (this.controller != null) {
            this.GetSavedType(defaultType);
            this.SaveType();
            this.controller.OnMagnetHit += this.HandleMagnetHit;
        }

    }

    public void Reset() {
        this.CancelInvoke();
        this.alive = true;
        this.hittable = false;
    }

    public void SetHittable(bool hittable) {
        this.hittable = hittable;
    }

    private void Update() {
        if (this.controller != null) {
            // all buttons plus the trigger
            if (this.controller.GetButton(PSMoveButton.Circle) && this.controller.GetButton(PSMoveButton.Cross) && this.controller.GetButton(PSMoveButton.Square) && this.controller.GetButton(PSMoveButton.Triangle) && this.controller.GetButtonDown(PSMoveButton.Trigger)) {
                this.type = this.type == Type.Ghost ? Type.Pac : Type.Ghost;
                this.SaveType();
            } 
        }
    }

    private void Kill() {
        this.alive = false;
        if (Game.Instance.AutoRevive) {
            this.Invoke("Revive", Game.Instance.AutoReviveTime);
        }
        if (this.OnKill != null) {
            this.OnKill(this);
        }
    }

    private void Revive() {
        this.alive = true;
        if (this.OnRevive != null) {
            this.OnRevive(this);
        }
    }

    private void HandleMagnetHit() {
        if (this.hittable && this.alive) {
            this.Kill();
        }
    }

    private void GetSavedType(Type defaultType) {
        this.type = (Type)PlayerPrefs.GetInt(this.controller.Serial + "_type", (int)defaultType);
    }

    private void SaveType() {
        PlayerPrefs.SetInt(this.controller.Serial + "_type", (int)this.type);
    }
}