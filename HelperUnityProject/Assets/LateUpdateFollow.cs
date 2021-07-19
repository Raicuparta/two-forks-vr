using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LateUpdateFollow : MonoBehaviour {
	public Transform Target;
	
	void LateUpdate () {
		transform.position = Target.position;
		transform.rotation = Target.rotation;
    }
}
