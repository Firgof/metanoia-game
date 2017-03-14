using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class textRefreshCoords : MonoBehaviour {
	public string template = "";
	public Text targetComponent;
	public GameObject player;
	public Vector3 playerTransform;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag("Player");
		targetComponent = gameObject.GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
		playerTransform = new Vector3(player.transform.position.x,player.transform.position.y,player.transform.position.z);
		targetComponent.text = template+"[ "+SceneManager.GetActiveScene().buildIndex.ToString()+" ]"+" < "+playerTransform.x.ToString("F2")+","+playerTransform.y.ToString("F2")+","+playerTransform.z.ToString("F2")+" >";
	}
}
