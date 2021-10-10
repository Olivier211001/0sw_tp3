using Godot;
using System;

public class player : KinematicBody2D
{
	Vector2 UP = new Vector2(0, -1);
	const int GRAVITY = 20;
	const int MAXFALLSPEED = 200;
	const int MAXSPEED = 100;
	const int JUMPFORCE = 300;

	const int ACCEL = 10;

	bool facing_right = true;


	Vector2 motion = new Vector2();

	Sprite currentSprite;
	AnimationPlayer animPlayer;
	AnimationTree aT;
		// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentSprite = GetNode<Sprite>("Sprite");
		animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		aT = GetNode<AnimationTree>("AnimationTree");
	}
	public Vector2 GetInput()
	{
	    var input_vector = Vector2.Zero;
	    input_vector.x = Input.GetActionStrength("ui_right") 
	                        - Input.GetActionStrength("ui_left");
	    input_vector = input_vector.Normalized();
	   return input_vector;
	}
	
	public void direction(bool d)
	{
		if(d == true)
		{
			currentSprite.FlipH = false;
		 }
		 else
		 {
			currentSprite.FlipH = true;
		 }
	}
	
	public override void _PhysicsProcess(float delta)
	{
		var aS = (AnimationNodeStateMachinePlayback)aT.Get("parameters/playback");
		var input_vector = GetInput();
		motion.y += GRAVITY;

		if(motion.y > MAXFALLSPEED) {
			motion.y = MAXFALLSPEED;
		}
	
		 motion.x = motion.Clamped(MAXSPEED).x;

		if (input_vector != Vector2.Zero) 
		{
			aT.Set("parameters/Idle/blend_position", input_vector);
			aT.Set("parameters/Run/blend_position", input_vector);
			aS.Travel("Run");
			if(input_vector.x > 0)
			{
				motion.x += ACCEL;
				facing_right = true;
				direction(facing_right);
			}
			else
			{
				motion.x -= ACCEL;
				facing_right = false;
				direction(facing_right);
			}
	    } 
		else 
		{	
			aT.Set("parameters/Attack/blend_position", motion);
			if (Input.IsActionJustReleased("attack")) {
				aS.Travel("Attack");
			}
			else
			{
				aS.Travel("Idle");   	
			}			
	        motion = motion.LinearInterpolate(Vector2.Zero, 0.2f);
	    }
	   		 
		if (IsOnFloor())
		{
			// On ne regarde qu'un seul fois et non le maintient de la touche	
			
			if (Input.IsActionJustPressed("ui_jump")) {
				motion.y = -JUMPFORCE;
				GD.Print($"motion.y = {motion.y}");
				Console.WriteLine($"motion.y = {motion.y}");
			}	
		}
		
		if (!IsOnFloor()) {
			aT.Set("parameters/Jump/blend_position", motion);
			aT.Set("parameters/Fall/blend_position", motion);
			if (motion.y < 0) {
				aS.Travel("Jump");
			}
			else if (motion.y > 0) {
			   aS.Travel("Fall");
			}
			
		}		
		motion = MoveAndSlide(motion, UP);
	}


	
}
