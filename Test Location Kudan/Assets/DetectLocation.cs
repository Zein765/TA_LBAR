using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Kudan.AR.Samples;

public class DetectLocation : MonoBehaviour {

	//location marker coordinate
	private Vector2 targetCoordinates;
	//device location coordinates
	private Vector2 deviceCoordinates;
	//Distance that allowed from device to target
	private float distanceFromTarget = 0.0001f;
	//Distance between device to target coordinate
	private float proximity = 0.01f;
	//Values that passed from Device's GPS
	private float sLatitude, sLongitude;
	//Set value for target location
	public float dLatitude = -7.279837f, dLongitude = 112.797381f;
	//Var for target loc
	private bool enableByRequest = true;
	public int maxWait = 10;
	public bool ready = false;
	public Text text;
	//SampleAPP script
	public SampleApp sa;

	//call getlocation from start
	void Start(){
		// create vector2 coor from given lat and lon
		targetCoordinates = new Vector2 (dLatitude, dLongitude);
		StartCoroutine (getLocation ());
	}

	//get last update location, we need latitude and longtitude
	IEnumerator getLocation(){
		LocationService service = Input.location;
		if (!enableByRequest && !service.isEnabledByUser) {
			Debug.Log("Location Services not enabled by user");
			yield break;
		}
		service.Start();
		while (service.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds(1);
			maxWait--;
		}
		if (maxWait < 1){
			Debug.Log("Timed out");
			yield break;
		}
		if (service.status == LocationServiceStatus.Failed) {
			Debug.Log("Unable to determine device location");
			yield break;
		} else {
			text.text = "Target Location : "+dLatitude + ", "+dLongitude+"\nMy Location: " + service.lastData.latitude + ", " + service.lastData.longitude;
			//here we pass lat and lon values from device
			sLatitude = service.lastData.latitude;
			sLongitude = service.lastData.longitude;
		}
		//Stop service if you want
		//service.Stop();
		ready = true;
		startCalculate ();
	}
	
	
	void Update(){
		
	}
	
	//method to calculate distances between device location and target location
	public void startCalculate(){
		//create vector 2 from device lat and lon
		deviceCoordinates = new Vector2 (sLatitude, sLongitude);
		//proximity calculate distance
		proximity = Vector2.Distance (targetCoordinates, deviceCoordinates);
		//if proximity < or = target ...
		if (proximity <= distanceFromTarget) {
			text.text = text.text + "\nDistance : " + proximity.ToString ();
			text.text += "\nTarget Detected";
			//show 3d object! call sampleApp script
			sa.StartClicked ();
		} else {
			//else, show warning or whatever...
			text.text = text.text + "\nDistance : " + proximity.ToString ();
			text.text += "\nTarget not detected, too far!";
		}
	}
}