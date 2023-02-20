using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Controller : MonoBehaviour
{
    public GameObject follow_target;
    protected Vector3 follow_target_static_offset;
    public float follow_time_smooth;

    // Start is called before the first frame update
    void Start()
    {
        if(follow_target == null)
        {
            Debug.LogError(this.ToString() + ": Camera has no follow target assigned!");
        }

        follow_target_static_offset = new Vector3(0, transform.position.y - follow_target.transform.position.y, 0);

        StartCoroutine(ContinueFollowingTarget());
    }

    //TODO
    //Use mouse screen xy to add offset to let player see futher (dist btwn character/screen center and mouse)
    //"deadzone" in certain radius around player where this behavior stops
    public IEnumerator ContinueFollowingTarget()
    {
        float max_cam_dist = 50f;

        while(true)
        {
            Vector2 dist_2d = new Vector2(transform.position.x - follow_target.transform.position.x, transform.position.z - follow_target.transform.position.z);

            float mag = Mathf.Clamp(dist_2d.magnitude, 0, max_cam_dist);

            float t = Freeman_Utilities.MapValueFromRangeToRange(mag, 0, max_cam_dist, 0, 1);

            //Vector3 velocity = Vector3.zero;
            //transform.position = Vector3.SmoothDamp(transform.position, follow_target.transform.position + follow_target_static_offset, ref velocity, follow_time_smooth);
            transform.position = Vector3.Lerp(transform.position, follow_target.transform.position + follow_target_static_offset, t); //seems to behave nicer with rigidbody movement (aka across fixed time)

            yield return null;
        }
    }
}
