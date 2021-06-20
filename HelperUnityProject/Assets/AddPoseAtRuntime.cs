using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class AddPoseAtRuntime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();
		pose.inputSource = SteamVR_Input_Sources.LeftHand;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
