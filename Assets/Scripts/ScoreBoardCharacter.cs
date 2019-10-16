using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreBoardCharacter : MonoBehaviour
{
    Animator animator;

    private float xScaleToSetInSpin = 1;

    public void OnMidSpin() {
        this.transform.localScale = new Vector3(this.xScaleToSetInSpin, 1, 1);
    }

    void Awake() {
        this.animator = GetComponent<Animator>();
    }

    public void Reset() {
        this.animator.SetBool("InGame", false);
        this.transform.localScale = Vector3.one;
    }

    public void StartGame() {
        this.animator.SetBool("InGame", true);
    }

    public void Hit() {
        this.animator.SetTrigger("Hit");
    }

    public void Switch(Player.Type chaser) {
        this.xScaleToSetInSpin = chaser == Player.Type.Ghost ? 1 : -1;
        this.animator.SetTrigger("Switch");
    }
}
