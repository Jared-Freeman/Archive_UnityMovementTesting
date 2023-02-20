using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script uses a different movement handling solution
//For all movement calculations other than jumping, we ignore the y component (pretending instead that we're just in 2D)
public class Player_Movement_V2 : MonoBehaviour
{
    #region members
    private Rigidbody rb;
    private float in_x;
    private float in_y;

    private float force_move = 5000f; //some magical number that makes no sense to me currently
    public float walk_speed; //magnitude in units/s. Is necessarily approximated
    public float move_accel; //??

    private bool flag_movement_input_allowed;   //some inputs need to be dropped briefly to apply forces from prev inputs correctly
    private bool flag_full_player_control;      //need to detect when physics are moving the char outside of movement system constraints so we don't mess with those emergent reactions
    private bool flag_sliding;

    #endregion
    #region
    #endregion

    #region
    #endregion

    #region
    #endregion

    #region init
    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        if (rb == null)
            Debug.LogError(this.ToString() + ": no attached rigidbody!");

        flag_full_player_control = true;
        flag_movement_input_allowed = true;
        flag_sliding = false;
    }
    #endregion

    private void Update()
    {
        UpdateInput();
    }

    private void FixedUpdate()
    {
        if (flag_movement_input_allowed) HandleMovement();


    }

    private void DisallowMovementInputs()
    {
        flag_movement_input_allowed = false;
    }
    private void AllowMovementInputs()
    {
        flag_movement_input_allowed = true;
    }

    //This will eventually need to handle a bunch of cases:
    //sprinting, jumping, "movement pausing"...
    private void HandleMovement()
    {
        Vector3 movement_desired_direction = new Vector3(in_x, 0, in_y);
        if (movement_desired_direction.sqrMagnitude > 1) movement_desired_direction.Normalize();
        Debug.DrawRay(transform.position + new Vector3(0, .1f, 0), movement_desired_direction * 2, Color.yellow);

        //Vector3 movement_vector = movement_desired_direction * force_move * Time.fixedDeltaTime * rb.mass;

        rb.velocity = movement_desired_direction * walk_speed; //yes I know this will terrify anyone who has ever used a rigidbody before...

        /*
        //apply forces if we have not reached max speed
        if (in_y > 0 && rb.velocity.z < walk_speed)
            rb.AddForce(new Vector3(0, 0, in_y * force_move * Time.fixedDeltaTime * rb.mass));
        if (in_y < 0 && rb.velocity.z > -walk_speed)
            rb.AddForce(new Vector3(0, 0, in_y * force_move * Time.fixedDeltaTime * rb.mass));

        //apply forces if we have not reached max speed
        if (in_x > 0 && rb.velocity.x < walk_speed)
            rb.AddForce(new Vector3(in_x * force_move * Time.fixedDeltaTime * rb.mass, 0, 0));
        if (in_x < 0 && rb.velocity.x > -walk_speed)
            rb.AddForce(new Vector3(in_x * force_move * Time.fixedDeltaTime * rb.mass, 0, 0));

        //extra decay of move velocity if input is 0
        if (in_x == 0 && in_y == 0)
        {
            StartCoroutine(ContinueMovementStall());
        }
        */
    }

    private void UpdateInput()
    {
        in_x = Input.GetAxis("Horizontal");
        in_y = Input.GetAxis("Vertical");
    }

    private void OnCollisionEnter(Collision collision)
    {

        foreach (ContactPoint cp in collision.contacts)
        {
        }
    }

    public void DoImpulse()
    {

    }
    private IEnumerator ContinueImpulse()
    {
        float cooldown = 1.5f;

        yield return new WaitForSeconds(cooldown);
    }

    //idea: if player is not exceeding their max speed, they can stop at will very quickly.
    private IEnumerator ContinueMovementStall()
    {
        if (rb.velocity.sqrMagnitude > .15f)
        {

            float stall_approx_duration = .95f;

            float start_time = Time.time;
            float current_time = Time.time;

            //yield return new WaitForFixedUpdate();

            DisallowMovementInputs();
            while (!flag_sliding)
            {
                //should check if player is actually in control or at the mercy of some physics nightmare...
                current_time = Time.time;

                float t = Freeman_Utilities.MapValueFromRangeToRange((current_time - start_time), 0, stall_approx_duration, 0, 1);

                rb.velocity = Vector3.Lerp(rb.velocity, new Vector3(0, 0, 0), t);

                if (rb.velocity.sqrMagnitude < .15f) break;

                yield return new WaitForFixedUpdate();
            }
            AllowMovementInputs();
        }
    }
}
