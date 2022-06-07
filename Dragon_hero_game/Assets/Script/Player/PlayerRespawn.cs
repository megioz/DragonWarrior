using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;// store last check point
    private Health playerHealth;

    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }
    public void Respawn()
    {
        transform.position = currentCheckpoint.position; //move player to check point position
        //restore player health and reset animation
        playerHealth.Respawn();

        //Move camera to checkpoint room (for this to work the checkpoint objects has to placed as a child of the room object)
        Camera.main.GetComponent<CameraController>().MoveToNewRoom(currentCheckpoint.parent);
    }
    //Activate checkpoint
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.transform.tag == "Checkpoint")
        {
            currentCheckpoint = collision.transform; //store the checkpoint that we activated as the current one
            SoundManager.instance.PlaySound(checkpointSound);
            collision.GetComponent<Collider2D>().enabled = false;//Deactivate checkpoint collider
            collision.GetComponent<Animator>().SetTrigger("appear");

        }
    }
}
