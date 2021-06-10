using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
public class CameraFollow360 : MonoBehaviour
{
	static public Transform player;


	void LateUpdate()
	{
		if (player)
		{
			this.transform.position = new Vector3(player.transform.position.x, player.transform.position.y + 0.5f, player.position.z - 5);
			this.transform.rotation = Quaternion.Euler(player.transform.rotation.x - 12, player.transform.rotation.y, player.transform.rotation.z);
		}
	}
}
