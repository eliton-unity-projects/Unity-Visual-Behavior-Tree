﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using UniRx;
using UnityEngine;

namespace Assets.Scripts.AI.Components
{
    /// <summary>
    /// Ticks all sub behaviors until a behavior returns success.
    /// Returns and is in success state if a child was successful, otherwise returns in fail state
    /// </summary>
    [Serializable]
    [Description("Runs children in order")]
    public class Selector : BehaviorComponent
    {
        public Selector(string name, int depth, int id) 
            : base(name, depth, id)
        { }

        public override IEnumerator Tick(UnityEngine.WaitForSeconds delayStart = null)
        {
            base.Tick().ToObservable().Subscribe(xb => Debug.Log("Subscribed to Selector at start (base.tick()"));
            
            CurrentState = (BehaviorState.Running);
            foreach (BehaviorTreeElement behavior in Children)
            {
                yield return BehaviorTreeManager.StartCoroutine(behavior.Tick());

                if (behavior.CurrentState != BehaviorState.Fail)
                {
                    this.CurrentState = behavior.CurrentState;

                    if (this.CurrentState == BehaviorState.Success)
                    {
                        UnityEngine.Debug.LogError("Selector is success");
                        //This selector has completed, break out of the operation
                        yield break;
                    }
                }
                Debug.LogError("Selector is fail");
            }
            //if it gets here, it went through all subbehaviors and had no successes
            CurrentState = BehaviorState.Fail;
            yield break;
        }
    } 
}