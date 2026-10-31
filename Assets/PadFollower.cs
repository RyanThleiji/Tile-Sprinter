using UnityEngine;

// Added by the spawner to the wall or sky obstacle in a jump pad pattern.
// Until the player hits the pad, it keeps the obstacle at the right distance from the pad
// for the current game speed, so a speed boost can't throw off the launch timing.
public class PadFollower : MonoBehaviour {
    public JumpPad pad;                // The pad this obstacle belongs to
    public float padToObstacleTime;    // Seconds from hitting the pad to reaching this obstacle
    public float catchUpSpeed = 15f;   // How quickly it slides into place when the speed changes

    // LateUpdate runs after everything has moved this frame
    void LateUpdate() {
        // Once the pad is hit (or gone), the speed is locked, so stop adjusting
        if ( pad == null || pad.Used ) return;

        float targetX = pad.transform.position.x + GameManager.S.ScrollSpeed * padToObstacleTime;
        Vector3 pos = transform.position;
        pos.x = Mathf.MoveTowards( pos.x, targetX, catchUpSpeed * Time.deltaTime );
        transform.position = pos;
    }
}