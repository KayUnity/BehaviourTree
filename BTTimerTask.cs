/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 11/27/2017 11:39:50 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using KayUtils;
using Object = System.Object;
using UnityEngine;

namespace AI
{
    public class BTTimerTask
    {
        long mCurrentTicks;
        long mInterval;
        long mNextTicks;
        Callback<Object> mCallback = null;
        bool isStop;
        public BTTimerTask(float interval, Action<Object> action)
        {
            if (action != null)
            {
                mCallback = new Callback<object>();
                mCallback.Handler = action;
            }
            mCurrentTicks = System.DateTime.Now.Ticks;
            mInterval = (long)(interval * 10000000);
            mNextTicks = mCurrentTicks + mInterval;
            isStop = true;
        }

        public bool Process(Object obj)
        {
            Start();
            if (System.DateTime.Now.Ticks > mNextTicks)
            {
                if (mCallback != null)
                {
                    mCallback.Arg1 = obj;
                    mCallback.Run();
                }
                Stop();
                return true;
            }
            return false;
        }

        void Start()
        {
            if (isStop)
            {
                isStop = false;
                mCurrentTicks = System.DateTime.Now.Ticks;
                mNextTicks = mCurrentTicks + mInterval;
            }
        }

        public void Stop()
        {
            isStop = true;
            mCurrentTicks = 0;
        }
    }

}
