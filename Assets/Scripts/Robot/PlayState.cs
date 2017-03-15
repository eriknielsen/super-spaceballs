using UnityEngine;
using System.Collections;
using System;

public class PlayState : IRobotState {
	
    private GameObject robot;
    private RobotBehaviour robotScript;

    Rigidbody2D robotrb;
   
    

    Vector2 zeroVector;
    Animator anim;
    public PlayState(GameObject r){
        zeroVector = new Vector2(0, 0);
        robot = r;
        robotScript = robot.GetComponent<RobotBehaviour>();
        anim = robot.GetComponent<Animator>();
        robotrb = r.GetComponent<Rigidbody2D>();
    }

    public void EnterPauseState(){
        robotScript.CurrentState = robotScript.pauseState;
        
        //empty the command list
        robotScript.Commands.Clear();
        robotScript.prevVelocity = robotScript.rb.velocity;
        robot.GetComponent<Rigidbody2D>().velocity = zeroVector;

        
        robotScript.GetComponent<AudioSource>().Stop();
        //set the speed of the current anim clip to 0
        //and then set it to 1 in pauseState's enterplaystate()

        robotScript.UpdateAnimationAngle(robotScript.prevVelocity.y,robotScript.prevVelocity.x);
        
        robotScript.angleAtPause = anim.GetFloat("DirectionAngle");
        
        //anim.Play("IdleAnims",0);
        anim.enabled = false;
    }
    
    public void EnterPlayState(){
        
    }

    public void UpdateState(){
        
        robotScript.ExecuteRobotCommand();
        robotScript.UpdateAnimationAngle(robotrb.velocity.y, robotrb.velocity.x);
       
        
    }
    public void OnAccelerate()
    {
        if(robotScript.accelerated == false)
        {
            
            //differentiate between the preview robot prefab and the real one
            if (robotScript.isPreview == false)
            {
                //anim.Play("MoveEntry");
                robotScript.accelerated = true;
                anim.SetBool("Accelerating", true);
                AudioManager.instance.PlayAudioWithRandomPitch(robotScript.igniteThrustersSound, true, robot);
                //loop thruster sound after ignite is finished
                robotScript.thrusterComponent.Play();
            }
        }
       
    }
    public void OnDeaccelerate()
    {
        if(robotScript.accelerated == true)
        {
            robotScript.accelerated = false;
            anim.SetBool("Accelerating", false);
            anim.SetTrigger("Deaccelerate");
            //differentiate between the preview robot prefab and the real one
            if (robotScript.isPreview == false)
            {
                AudioManager.instance.PlayAudioWithRandomPitch(robotScript.endThrustersSound, false, robot);
                robotScript.thrusterComponent.Stop();
            }
        }
       
           
    }
}
