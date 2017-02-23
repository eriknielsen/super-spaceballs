using UnityEngine;
using System.Collections;
using System;

public class PlayState : IRobotState {
	
    private GameObject robot;
    private RobotBehaviour robotScript;


    Vector2 zeroVector;
    Animator anim;
    public PlayState(GameObject r){
        zeroVector = new Vector2(0, 0);
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
        anim = robot.GetComponent<Animator>();
    }

    public void EnterPauseState(){
        robotScript.CurrentState = robotScript.pauseState;
        //empty the command list
        robotScript.Commands.Clear();
        robotScript.prevVelocity = robot.GetComponent<Rigidbody2D>().velocity;
        robot.GetComponent<Rigidbody2D>().velocity = zeroVector;

        anim.enabled = false;
        
    }

    public void EnterPlayState(){
        Debug.Log("already in playstate");
    }

    public void UpdateState(){
        
        robotScript.ExecuteRobotCommand();
        robotScript.UpdateAnimationAndCollider();
    }
    public void OnAccelerate()
    {
        anim.SetBool("Accelerating", true);
        //anim.Play("MoveEntry");
        //differentiate between the preview robot prefab and the real one
        if(robotScript.igniteThrustersSound != null)
        {
            AudioManager.instance.PlayAudioWithRandomPitch(robotScript.igniteThrustersSound, false, robot);
            //loop thruster sound after ignite is finished
            robotScript.thrusterComponent.PlayDelayed(robotScript.igniteThrustersSound.GetComponent<AudioSource>().clip.length);
        }
    }
    public void OnDeaccelerate()
    {
        anim.SetBool("Accelerating", false);
        //differentiate between the preview robot prefab and the real one
        if (robotScript.endThrustersSound != null)
        {
            AudioManager.instance.PlayAudioWithRandomPitch(robotScript.endThrustersSound, false, robot);
            robotScript.thrusterComponent.Stop();
        }
           
    }
}
