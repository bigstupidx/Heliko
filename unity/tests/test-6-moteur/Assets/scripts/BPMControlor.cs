﻿using UnityEngine;
using System.Collections;

public class BPMControlor : MonoBehaviour
{

	public AudioSource music;
	public float tempo = 130.0f;
	public float d = 0;
	private bool notifiedEnter = false;
	private bool notifiedExit = false;
	public float errorMargin = 100; // en MS

	float dHalf = 0;

	public float getDeltaTempo ()
	{
		return (60.0f / tempo);
	}

	public float getScore ()
	{
		float s = this.timeSinceLastTick () / this.getDeltaTempo ();
		float s2 = this.timeBeforeNextTick () / this.getDeltaTempo ();
		return 1 - (Mathf.Min (s, s2) * 2);
	}

	public bool isOnStep ()
	{
		float t = timeSinceLastTick (); // Plus en plus négatif
		float t2 = timeBeforeNextTick (); // Plus en plus positif
		float diff = Mathf.Min (t, t2) * 1000; // en MS
		//Debug.Log ("Diff = " + diff);
		return (diff < errorMargin);
	}

	public bool isOnHalfStep ()
	{
		float f1 = (getDeltaTempo () * 0.5f) * (1 + errorMargin);
		float f2 = (getDeltaTempo () * 0.5f) * (1 - errorMargin);
		return d < f1 && d > f2;
	}

	void Start ()
	{
		music.Play ();
		this.dHalf = d + this.getDeltaTempo () / 2;
	}
	
	public float timeBeforeNextTick ()
	{
		return this.getDeltaTempo () - d;
	}
	
	public float timeSinceLastTick ()
	{
		return d;
	}
	
	void notifyChildren ()
	{
		foreach (Transform s1 in transform) {
			if (s1.GetComponent<TempoReceiver> () != null) {
				s1.GetComponent<TempoReceiver> ().onStep ();
			} else {
				Debug.LogError (s1.name + " : GetComponent TempoReceiver null");
			}
		}
	}

	void notifyExitSuccessWindow ()
	{
		foreach (Transform s1 in transform) {
			if (s1.GetComponent<TempoReceiver> () != null) {
				s1.GetComponent<TempoReceiver> ().onSuccessWindowExit ();
			} else {
				Debug.LogError (s1.name + " : GetComponent TempoReceiver null");
			}
		}
	}

	void notifyEnterSuccessWindow ()
	{
		foreach (Transform s1 in transform) {
			if (s1.GetComponent<TempoReceiver> () != null) {
				s1.GetComponent<TempoReceiver> ().onSuccessWindowEnter ();
			} else {
				Debug.LogError (s1.name + " : GetComponent TempoReceiver null");
			}
		}
	}
	
	float lastTime = 0;
	
	private float getTime ()
	{
		return (float)music.timeSamples / (float)music.clip.frequency;
	}

	private bool exitedSuccessWindow ()
	{
		if (!notifiedExit && !isOnStep ()) {
			notifiedExit = true;
			notifiedEnter = false;
			return true;
		}
		return false;
	}
	
	private bool enteredSuccessWindow ()
	{
		if (!notifiedEnter && isOnStep ()) {
			notifiedEnter = true;
			notifiedExit = false;
			return true;
		}
		return false;
	}

	
	void Update ()
	{
		
		float diff = this.getTime () - lastTime;
		lastTime = this.getTime ();
		d += diff;
		dHalf += diff;
		
		if (d > this.getDeltaTempo ()) {
			d = (d - this.getDeltaTempo ());
			this.notifyChildren ();
		}

		if (enteredSuccessWindow ()) {
			this.notifyEnterSuccessWindow ();
		}

		if (exitedSuccessWindow ()) {
			this.notifyExitSuccessWindow ();
		}
		
		if (dHalf > this.getDeltaTempo ()) {
			dHalf = (dHalf - this.getDeltaTempo ());
			this.notifyChildren ();
		}
	}
}
