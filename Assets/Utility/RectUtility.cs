using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


    public static class RectUtility
    {
        public static Rect GetRealRectInGroup(this Rect inRect, Rect groupRect)
        {
            Rect tmp = new Rect(inRect.x + groupRect.x,
                            inRect.y + groupRect.y,
                            inRect.width,
                            inRect.height);
            float width = inRect.width;
            float height = inRect.height;
            if (tmp.xMax > groupRect.xMax)
            {
                width -= tmp.xMax - groupRect.xMax;
                width = width < 0 ? 0 : width;
            }
            if (tmp.yMax > groupRect.yMax)
            {
                height -= tmp.yMax - groupRect.yMax;
                height = height < 0 ? 0 : height;
            }

            return new Rect(tmp.x,
                            tmp.y,
                            width,
                            height);;
        }
    }

