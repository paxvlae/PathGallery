using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.UtilityScripts
{
    class InertialMove<T>
    {
        private readonly int inertiaMaxFrames;
        private int inertialFrames = 0;
        private T inertialDelta;
        private readonly Func<T, T> suppressMoveFunc;
        private readonly Func<T, T, T> sumFunc;
        private readonly Predicate<T> isGreaterThenEpsilon;
        private readonly T zeroValue;

        public InertialMove(Func<T, T> suppressMoveFunc, Func<T, T, T> sumFunc, Predicate<T> isGreaterThenEpsilon, T zeroValue, int maxInertialFrames = 20)
        {
            inertiaMaxFrames = maxInertialFrames;
            this.suppressMoveFunc = suppressMoveFunc;
            this.sumFunc = sumFunc;
            this.isGreaterThenEpsilon = isGreaterThenEpsilon;
            this.zeroValue = zeroValue;
        }
        public T GetMove()
        {
            if (inertialFrames > 0)
            {
                inertialDelta = suppressMoveFunc(inertialDelta);
                if (isGreaterThenEpsilon(inertialDelta))
                {
                    inertialFrames--;
                }
                else
                {
                    inertialDelta = zeroValue;
                    inertialFrames = 0;
                }
            }
            return inertialDelta;
        }

        public void AddMove(T startDelta)
        {
            inertialDelta = sumFunc(inertialDelta, startDelta);
            inertialFrames = inertiaMaxFrames;
        }

    }
}
