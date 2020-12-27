﻿using UnityEngine;
using Obi;


[RequireComponent(typeof(ObiActor))]
public class ResetObi : MonoBehaviour
{
	ObiActor actor;
	GameObject pick;
	GameObject end;
	Vector3 pick_orig_pos;
	Vector3 end_orig_pos;
    void Awake()
    {
        actor = GetComponent<ObiActor>();
        pick = GameObject.Find("Pick");
        end = GameObject.Find("End");
        pick_orig_pos = pick.transform.position;
        end_orig_pos = end.transform.position;
    }

    public void reset()
	{

		if (actor == null || !actor.isLoaded)
			return;

		actor.ResetParticles();
		pick.transform.position = pick_orig_pos;
		end.transform.position = end_orig_pos;
		print("Reset!");		
	}

	public void move()
    {
		actor.GetComponent<ObiParticlePicker>().MoveInit();
		print("Moving");
    }

	private void Update()
	{
		if (Input.GetKey(KeyCode.RightShift))
		{
			reset();
		}
	}
}
