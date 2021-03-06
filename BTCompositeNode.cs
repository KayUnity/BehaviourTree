﻿/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 11/25/2017 11:40:23 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using System.Collections.Generic;

using KayUtils;
using System.Collections;
using Object = System.Object;
using UnityEngine;

namespace AI
{
    public class BTCompositeNode : BehaviourTreeNode
    {
        protected List<BehaviourTreeNode> mChildren;
        public BTCompositeNode() : base()
        {
            mChildren = new List<BehaviourTreeNode>();
        }

        public void AddChild(BehaviourTreeNode node)
        {
            mChildren.Add(node);
        }

        public override BTNodeState Process(Object obj)
        {
            return BTNodeState.Ready;
        }
    }

    public class BTSelectorNode : BTCompositeNode
    {
        public override BTNodeState Process(Object obj)
        {
            int index = 0;
            if (mNodeState == BTNodeState.Running && mRunningNode != null)
            {
                index = mChildren.IndexOf(mRunningNode);
                if (index == -1)
                {
                    mNodeState = BTNodeState.Ready;
                    index = 0;
                }
            }
            for (int i = index; i < mChildren.Count; ++i)
            {
                mNodeState = mChildren[i].Run(obj);
                if (mNodeState == BTNodeState.Running)
                {
                    mRunningNode = mChildren[i];
                    return mNodeState;
                }
                if (mNodeState == BTNodeState.Success)
                {
                    return mNodeState;
                }
            }
            mNodeState = BTNodeState.Failure;
            return mNodeState;
        }
    }

    public class BTSequenceNode : BTCompositeNode
    {
        public override BTNodeState Process(Object obj)
        {
            int index = 0;
            if (mNodeState == BTNodeState.Running && mRunningNode != null)
            {
                index = mChildren.IndexOf(mRunningNode);
                if (index == -1)
                {
                    mNodeState = BTNodeState.Ready;
                    index = 0;
                }
            }
            for (int i = index; i < mChildren.Count; ++i)
            {
                mNodeState = mChildren[i].Run(obj);
                if (mNodeState == BTNodeState.Running)
                {
                    mRunningNode = mChildren[i];
                    return mNodeState;
                }
                if (mNodeState == BTNodeState.Failure)
                {
                    return mNodeState;
                }
            }
            mNodeState = BTNodeState.Success;
            return mNodeState;
        }
    }

    public class BTRandomSelectorNode : BTCompositeNode
    {
        public override BTNodeState Process(Object obj)
        {
            int[] seq = RandomShuffle(mChildren.Count);
            foreach (int index in seq)
            {
                if (mChildren[index].Run(obj) == BTNodeState.Success)
                {
                    return BTNodeState.Success;
                }
            }
            return BTNodeState.Failure;
        }

        private int[] RandomShuffle(int indexCount)
        {
            int[] res = new int[indexCount];
            for (int i = 0; i < indexCount; ++i)
            {
                res[i] = i;
            }
            for (int i = 0; i < indexCount; ++i)
            {
                int j = UnityEngine.Random.Range(0, indexCount);
                while (j == i)
                {
                    j = UnityEngine.Random.Range(0, indexCount);
                }
                int temp = res[i];
                res[i] = res[j];
                res[j] = temp;
            }
            return res;
        }
    }

    // 并行，与 Sequence 不同，节点同时运行
    //  &&  || 操作
    // 只是优化 可使用 协程 StartCor
    public abstract class BTParallelNode : BTCompositeNode
    {
        // 并行节点除了继承 BehaviourTreeNode 外，还需要实现 BTParallelFunc
        public interface IBTParallelFunc
        {
            IEnumerator RunCode();
            BTNodeState NodeStatus();
        }

        List<Coroutine> mCoroutines = new List<Coroutine>();
        MonoBehaviour behaviour;
        public override BTNodeState Process(Object obj)
        {
            if (mNodeState == BTNodeState.Ready || mNodeState == BTNodeState.Success)
            {
                if (!(obj is MonoBehaviour))
                {
                    throw new Exception("exception: " + "obj should extends MonoBehaviour");
                }
                behaviour = (MonoBehaviour)obj;
                foreach (BehaviourTreeNode node in mChildren)
                {
                    if (typeof(IBTParallelFunc).IsAssignableFrom(node.GetType()))
                    {
                        mCoroutines.Add(behaviour.StartCoroutine(((IBTParallelFunc)node).RunCode()));
                    }
                    else
                    {
                        throw new Exception("exception: " + node.GetType().Name + " should implements IBTParallelFunc");
                    }
                }
                mNodeState = BTNodeState.Running;
            }
            else if (mNodeState == BTNodeState.Running)
            {
                mNodeState = Handle();
            }
            return mNodeState;
        }

        protected abstract BTNodeState Handle();

        protected void Cancel()
        {
            if (behaviour != null)
            {
                foreach (Coroutine cor in mCoroutines)
                {
                    behaviour.StopCoroutine(cor);
                }
            }
        }
    }

    public class BTAndParaNode : BTParallelNode
    {
        protected override BTNodeState Handle()
        {
            BTNodeState result = BTNodeState.Running;
            int count = 0;
            foreach (BehaviourTreeNode node in mChildren)
            {
                if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Failure)
                {
                    result = BTNodeState.Failure;
                    break;
                }
                else if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Running)
                {
                    continue;
                }
                else if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Success)
                {
                    count++;
                }
            }
            if (count == mChildren.Count)
            {
                result = BTNodeState.Success;
            }
            return result;
        }
    }

    public class BTOrParaNode : BTParallelNode
    {
        protected override BTNodeState Handle()
        {
            BTNodeState result = BTNodeState.Running;
            int count = 0;
            foreach (BehaviourTreeNode node in mChildren)
            {
                if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Success)
                {
                    result = BTNodeState.Success;
                    break;
                }
                else if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Running)
                {
                    continue;
                }
                else if (((IBTParallelFunc)node).NodeStatus() == BTNodeState.Failure)
                {
                    count++;
                }
            }
            if (count == mChildren.Count)
            {
                result = BTNodeState.Failure;
            }
            return result;
        }
    }

}
