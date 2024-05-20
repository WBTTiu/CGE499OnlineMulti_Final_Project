using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DoorPuzzle
{
    public class Bridges : MonoBehaviour
    {
        public bool IsMovingUp, IsMovingDown;
        [Tooltip("List of game objects (bridges). Each elements in the list must be at the same index as its down position in DownPosList")]
        [SerializeField] private GameObject[] _bridgesList;
        [SerializeField] private List<Vector3> _topPos;
        [Tooltip("List of bridges' down position. Each elements in the list must be at the same index as its game object in BridgesList")]
        [SerializeField] private List<Vector3> _downPosList;
        [SerializeField] private float _moveSpeed;
        [SerializeField] private float _yAxisTopPos;

        private void Start()
        {
            for (int i = 0; i < _bridgesList.Length; i++)
            {
                _downPosList.Add(_bridgesList[i].transform.position);
                _topPos.Add(_bridgesList[i].transform.position);
                _topPos[i] = new Vector3(_topPos[i].x, _yAxisTopPos, _topPos[i].z);
            }

            
        }

        public IEnumerator MoveBridgesUp()
        {
            print("MoveBridgesUp Coroutine");
            while (_bridgesList.Any(x => x.transform.position.y != _yAxisTopPos) && IsMovingUp)
            {
                print("Bridges move up");
                _bridgesList[0].transform.position = Vector3.MoveTowards(_bridgesList[0].transform.position, _topPos[0], 
                    _bridgesList[0].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                _bridgesList[1].transform.position = Vector3.MoveTowards(_bridgesList[1].transform.position, _topPos[1], 
                    _bridgesList[1].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                _bridgesList[2].transform.position = Vector3.MoveTowards(_bridgesList[2].transform.position, _topPos[2], 
                    _bridgesList[2].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
        
        public IEnumerator MoveBridgesDown()
        {
            print("MoveBridgesDown Coroutine");
            while ((_bridgesList[0].transform.position != _downPosList[0] || _bridgesList[1].transform.position != _downPosList[1] 
                    || _bridgesList[2].transform.position != _downPosList[2]) && IsMovingDown)
            {
                print("Bridges move down");
                _bridgesList[0].transform.position = Vector3.MoveTowards(_bridgesList[0].transform.position, _downPosList[0], 
                    _bridgesList[0].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                _bridgesList[1].transform.position = Vector3.MoveTowards(_bridgesList[1].transform.position, _downPosList[1], 
                    _bridgesList[1].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                _bridgesList[2].transform.position = Vector3.MoveTowards(_bridgesList[2].transform.position, _downPosList[2], 
                    _bridgesList[2].GetComponent<BridgeData>().MoveSpeed * Time.deltaTime);
                yield return new WaitForSecondsRealtime(0.01f);
            }
        }
    }
}