using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchScript.Events;
using TouchScript.Gestures;
using UnityEngine;

namespace Assets.Gallery
{
    class SlideBehaviour : MonoBehaviour
    {
        void Start()
        {
            var tapGesture = GetComponent<TapGesture>();
            if(tapGesture != null)
                tapGesture.StateChanged += TapGestureOnStateChanged;
            else
            {
                Debug.Log("This slide don't have tap gesture");
            }
        }

        private void TapGestureOnStateChanged(object sender, GestureStateChangeEventArgs gestureStateChangeEventArgs)
        {
            if (gestureStateChangeEventArgs.State == Gesture.GestureState.Recognized)
            {
                Transform obj = GetGameObjectWithTagInPatent(transform, "PathElement");
                PathCameraController.Instance.SlideClicked(obj.name);
            }
        }

        private Transform GetGameObjectWithTagInPatent(Transform child, string tag)
        {
            for (Transform go = child; go != null; go = go.parent)
            {
                if (go.tag == tag)
                    return go;
            }
            return null;
        }
    }
}
