using Assets;
using Assets.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Trader
{
    public class Trader : MonoBehaviour
    {
        private AsyncStateMachine<TraderState, Trader> stateMachine;
        public IDictionary<TraderState, dynamic> stateData;
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
