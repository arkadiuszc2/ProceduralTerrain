using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    // Start is called before the first frame update
    void Start()
    {
        Application.targetFrameRate = 120;
        player.transform.position = player.transform.position + new Vector3(0.5f, 0.5f, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.W)) {
            player.transform.position = player.transform.position + new Vector3(0f, 0.03f, 0f);
        }
        if (Input.GetKey(KeyCode.A)) {
            player.transform.position = player.transform.position + new Vector3(-0.03f, 0f, 0f);
        }
        if (Input.GetKey(KeyCode.S)) {
            player.transform.position = player.transform.position + new Vector3(0f, -0.03f, 0f);
        }
        if (Input.GetKey(KeyCode.D)) {
            player.transform.position = player.transform.position + new Vector3(0.03f, 0f, 0f);
        }
    }
}
