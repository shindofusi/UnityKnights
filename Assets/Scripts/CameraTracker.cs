using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraTracker : MonoBehaviour
{
    public PlayerController p1, p2;
    private Transform player1, player2;
    public float minSizeY = 5f;
    private Camera camera;

    void SetCameraPos()
    {
        Vector3 middle = (player1.position + player2.position) * 0.5f;

        camera.transform.position = new Vector3(
            middle.x,
            middle.y,
            camera.transform.position.z
        );
    }

    void SetCameraSize()
    {
        //horizontal size is based on actual screen ratio
        float minSizeX = minSizeY * Screen.width / Screen.height;

        //multiplying by 0.5, because the ortographicSize is actually half the height
        float width = Mathf.Abs(player1.position.x - player2.position.x) * 0.5f;
        float height = Mathf.Abs(player1.position.y - player2.position.y) * 0.5f;

        //computing the size
        float camSizeX = Mathf.Max(width, minSizeX);
        camera.orthographicSize = Mathf.Max(height,
        camSizeX * Screen.height / Screen.width, minSizeY);
    }

    // Start is called before the first frame update
    void Start()
    {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        player1 = p1.transform;
        player2 = p2.transform;
    }
    // Update is called once per frame
    void Update()
    {

        if (p1 == null || !p1.isActiveAndEnabled) player1 = player2;
        else player1 = p1.transform;
        if (p2 == null || !p2.isActiveAndEnabled) player2 = player1;
        else player2 = p2.transform;
        Vector3 diff = player1.position - player2.position;

        //if ((diff.x > 10f || diff.y > 10f) || SceneManager.GetActiveScene().name == "PVP")
        //{
            SetCameraPos();
            SetCameraSize();
        //}
    }
}
