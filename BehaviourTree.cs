/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 11/25/2017 11:17:03 AM
** Version:  v1.0.0
*********************************************************************************/

using Object = System.Object;
using UnityEngine;

namespace AI
{
    public enum BTNodeState
    {
        Failure = 0,
        Success,
        Running,
        Ready,
        Error
    }

    public abstract class BehaviourTreeNode
    {
        protected BehaviourTreeNode mRunningNode;
        protected BTNodeState mNodeState;
        protected string mName = "";
        public BehaviourTreeNode()
        {
            mRunningNode = null;
            mNodeState = BTNodeState.Ready;
        }
        public virtual void Enter(Object obj)
        {

        }

        public string Name
        {
            get
            {
                return mName;
            }
            set
            {
                mName = value;
            }
        }

        public virtual BTNodeState Run(Object obj)
        {
            Enter(obj);
            mNodeState = Process(obj);
            Leave(obj);
            return mNodeState;
        }

        public virtual void Leave(Object obj)
        {
            // Debug.Log(mName + " " + mNodeState);
        }

        public abstract BTNodeState Process(Object obj);
    }

    public class BehaviorTreeRoot : BehaviourTreeNode
    {
        BehaviourTreeNode mRoot;
        
        public BehaviorTreeRoot()
        {

        }

        public BehaviorTreeRoot(BehaviourTreeNode root)
        {
            Root = root;
        }

        public override BTNodeState Process(Object obj)
        {
            mNodeState = Root.Run(obj);
            return mNodeState;
        }

        public BehaviourTreeNode Root
        {
            get
            {
                return mRoot;
            }
            set
            {
                mRoot = value;
            }
        }
    }
}
