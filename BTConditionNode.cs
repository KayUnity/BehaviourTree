/********************************************************************************
** All rights reserved
** Auth： kay.yang
** E-mail: 1025115216@qq.com
** Date： 11/25/2017 11:38:21 AM
** Version:  v1.0.0
*********************************************************************************/

using System;
using Object = System.Object;
using UnityEngine;

namespace AI
{
    public class BTConditionNode : BehaviourTreeNode
    {
        public override BTNodeState Process(Object obj)
        {
            return BTNodeState.Ready;
        }
    }

}
