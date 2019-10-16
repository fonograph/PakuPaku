using UnityEngine;
using System;

public class EnhancedMoveController : UniMoveController 
{
    public Action OnMagnetHit;

    private float mx;
	private float my;
	private float mz;

	public float mxMin;
	public float mxMax;
	public float myMin;
	public float myMax;
	public float mzMin;
	public float mzMax;

	private bool canIBeCalibrating;
	private int mxMinDriftTicks;
	private int mxMaxDriftTicks;
	private int myMinDriftTicks;
	private int myMaxDriftTicks;
	private int mzMinDriftTicks;
	private int mzMaxDriftTicks;

    private float magnetThreshold;

    private bool lostConnection;

    public bool Init(int index, float magnetThreshold) {
        if (base.Init(index)) {
            this.magnetThreshold = magnetThreshold;
            canIBeCalibrating = true;
            mxMax = -2048;
            mxMin = 2048;
            myMax = -2048;
            myMin = 2048;
            mzMax = -2048;
            mzMax = 2048;
            mxMinDriftTicks = 0;
            mxMaxDriftTicks = 0;
            myMinDriftTicks = 0;
            myMaxDriftTicks = 0;
            mzMinDriftTicks = 0;
            mzMaxDriftTicks = 0;

            LoadCalibration();
            Invoke("CalibrateForever", 0);
        }
        return false;
	}

    public new void SetLED(Color color) {
        if (this.lostConnection) {
            return;
        }
        base.SetLED(color);
    }

    private void Update() {
        mx = Magnetometer.x;
		my = Magnetometer.y;
		mz = Magnetometer.z;		
        float d = this.magnetThreshold;
        if ( (mx!=0 || my!=0 || mz!=0 ) && ( mx < mxMin-d || mx > mxMax+d || my < myMin-d || my > myMax+d || mz < mzMin-d || mz > mzMax+d ) ) {
			if (this.OnMagnetHit != null) {
                this.OnMagnetHit();
            }
		}

        if ( mx==0 && my==0 && mz==0 ) {
            this.SetLED(Color.red);
            this.lostConnection = true;
        } else if (lostConnection) {
			lostConnection = false;
			this.SetLED(Color.white);
		}
    }

    private void CalibrateForever() {
		int thresholdTicks = 2;
		if ( canIBeCalibrating ) {
			if ( mx < mxMin ) {
				mxMinDriftTicks++;
				if ( mxMinDriftTicks >= thresholdTicks ) {
					mxMin = mx;
					mxMax = mxMin+360;
					mxMinDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "XMax", mxMax);
					PlayerPrefs.SetFloat(Serial + "XMin", mxMin);
				}
			}
			if ( mx > mxMax ) {
				mxMaxDriftTicks++;
				if ( mxMaxDriftTicks >= thresholdTicks ) {
					mxMax = mx;
					mxMin = mxMax-360;
					mxMaxDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "XMax", mxMax);
					PlayerPrefs.SetFloat(Serial + "XMin", mxMin);
				}
			}
			if ( my < myMin ) {
				myMinDriftTicks++;
				if ( myMinDriftTicks >= thresholdTicks ) {
					myMin = my;
					myMax = myMin+360;
					myMinDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "YMax", myMax);
					PlayerPrefs.SetFloat(Serial + "YMin", myMin);
				}
			}
			if ( my > myMax ) {
				myMaxDriftTicks++;
				if ( myMaxDriftTicks >= thresholdTicks ) {
					myMax = my;
					myMin = myMax-360;
					myMaxDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "YMax", myMax);
					PlayerPrefs.SetFloat(Serial + "YMin", myMin);
				}
			}
			if ( mz < mzMin ) {
				mzMinDriftTicks++;
				if ( mzMinDriftTicks >= thresholdTicks ) {
					mzMin = mz;
					mzMax = mzMin+360;
					mzMinDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "ZMax", mzMax);
					PlayerPrefs.SetFloat(Serial + "ZMin", mzMin);
				}
			}
			if ( mz > mzMax ) {
				mzMaxDriftTicks++;
				if ( mzMaxDriftTicks >= thresholdTicks ) {
					mzMax = mz;
					mzMin = mzMax-360;
					mzMaxDriftTicks = 0;
					PlayerPrefs.SetFloat(Serial + "ZMax", mzMax);
					PlayerPrefs.SetFloat(Serial + "ZMin", mzMin);
				}
			}
		}
		Invoke("CalibrateForever", 0.5f);
	}

    private void LoadCalibration() {
		mxMax = PlayerPrefs.GetFloat(Serial + "XMax");
		mxMin = PlayerPrefs.GetFloat(Serial + "XMin");
		myMax = PlayerPrefs.GetFloat(Serial + "YMax");
		myMin = PlayerPrefs.GetFloat(Serial + "YMin");
		mzMax = PlayerPrefs.GetFloat(Serial + "ZMax");
		mzMin = PlayerPrefs.GetFloat(Serial + "ZMin");
	}
	


}