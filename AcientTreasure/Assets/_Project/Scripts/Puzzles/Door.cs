using UnityEngine;
using System.Collections;
using UnityEngine.Serialization;


namespace DoorPuzzle
{
    public class Door : MonoBehaviour
    {
        public bool IsPressurePad01Pressed, IsPressurePad02Pressed;
        public bool IsDoorMoved;
        [FormerlySerializedAs("_topPos")] [SerializeField] private Vector3 topPos;
        [FormerlySerializedAs("_moveSpeed")] [SerializeField] private float moveSpeed;
        [FormerlySerializedAs("_timeToWait")] [SerializeField] private float timeToWait;
        
        public IEnumerator MoveDoor()
        {
            while (transform.position != topPos)
            {
                transform.position = Vector3.MoveTowards(transform.position, topPos, moveSpeed * Time.deltaTime);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }
}