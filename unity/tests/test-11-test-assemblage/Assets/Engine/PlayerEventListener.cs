using UnityEngine;
using System.Collections;

public class PlayerEventListener : MonoBehaviour
{
	public bool onKeyDown = true;
	public BeatCounter bc;
	public float timeBeforeLongTouch;

	private ArrayList observers;

	#if UNITY_ANDROID || UNITY_IOS
	private bool touchScreen;
	private Vector2 mouvement;
	private float timeTouchTotal;
	#endif

	void Start()
	{
		#if UNITY_ANDROID || UNITY_IOS
		touchScreen = false;
		timeTouchTotal = 0F;
		mouvement = Vector2.zero;
		#endif
	}

	public void Awake () {
		this.observers = new ArrayList ();
	}

	public void connect (PlayerEventReceiver r) {
		this.observers.Add(r);
	}

	private void sendEvent(int type) {
		foreach (PlayerEventReceiver e in this.observers) {
			e.onFinger (type);
		}
	}

	void Update () {
		if (bc.isInPause()) {
			return;
		}
		#if UNITY_ANDROID || UNITY_IOS

		foreach (Touch touch in Input.touches) {
			if (touch.phase == TouchPhase.Began) {
				foreach (PlayerEventReceiver e in this.observers) {
					e.onFinger (1);
				}
			}
		}

		/*
		if (Input.touchCount > 0) 
		{
			switch (Input.GetTouch(0).phase) 
			{
				case TouchPhase.Began:
					touchScreen=true;
				break;  

				case TouchPhase.Ended:
					if (mouvement.magnitude>=12 && touchScreen) {
						sendEvent(4);
					}
					else if(timeTouchTotal>=timeBeforeLongTouch) {
						sendEvent(3);
					}
					else {
						sendEvent(1);
					}
					timeTouchTotal = 0F;
					touchScreen=false; 
					mouvement=Vector2.zero;
				break;  

				case TouchPhase.Stationary: 
					timeTouchTotal+=Time.deltaTime;
					if (timeTouchTotal>=timeBeforeLongTouch && touchScreen) {
						sendEvent(2);
						touchScreen=false;
					}
				break;

				case TouchPhase.Moved: 
					mouvement+=Input.GetTouch(0).deltaPosition;	
					if (mouvement.magnitude <= 4) {
						timeTouchTotal += Time.deltaTime;
						if (timeTouchTotal >= timeBeforeLongTouch && touchScreen) {
							sendEvent(2);
							touchScreen=false;
						}
					}
				break;                
			}  		
		}
		*/
		#endif 
		
		if (Input.GetKeyDown (KeyCode.P) &&  Input.GetKeyDown (KeyCode.O))
			sendEvent(3);

		if ((onKeyDown && Input.GetKeyDown (KeyCode.O)) || (!onKeyDown && Input.GetKey (KeyCode.O)))
			sendEvent(1);

		if ((onKeyDown && Input.GetKeyDown (KeyCode.P)) || (!onKeyDown && Input.GetKey (KeyCode.P)))
			sendEvent(2);
		
	}


}