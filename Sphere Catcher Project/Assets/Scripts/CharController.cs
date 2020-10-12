using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharController : MonoBehaviour
{
    public int items = 0;

    
    [SerializeField]
    float chrSpeed = 5f;

    Vector3 forward, right;


    void Start()
    {
        forward = Camera.main.transform.forward;
        forward.y = 0;
        forward = Vector3.Normalize(forward);
        right = Quaternion.Euler(new Vector3(0, 90, 0)) * forward;
    }

    void Update()
    {
        if (Input.anyKey)
            Move();
        if(Input.GetKeyDown(KeyCode.R)){
            SceneManager.LoadScene("Main");
        }
        
    }

    void Move()
    {
        Vector3 direction = new Vector3(Input.GetAxis("HorizontalKey"), 0, Input.GetAxis("VerticalKey"));
        Vector3 rightMovement = right * chrSpeed * Time.deltaTime * Input.GetAxis("HorizontalKey");
        Vector3 upMovement = forward *chrSpeed * Time.deltaTime * Input.GetAxis("VerticalKey");

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }
    

    
    private void OnGUI(){
        GUI.Label(new Rect(10, 10, 100, 20), "Zebrano " + items + "/3 ");
    }

    
    public void Respawn(){
        if (AIControl.death == true)
            SceneManager.LoadScene("Main");
    }

}
