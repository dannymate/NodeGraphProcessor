using System.Collections.ObjectModel;
using System;
using System.Collections.Generic;
using GraphProcessor;
using UnityEngine;

[Serializable, NodeMenuItem("TEST NODE")]
public class TestNode : BaseNode
{
    [ShowAsDrawer, MultiPortInput(showParentInInspector = true), ShowInInspector(true), SerializeField]
    public string[] myList;

    public override bool needsInspector => true;

    [CustomPortBehavior(nameof(myList))]
    public void test()
    {
        // this.GetPort(nameof(myList), null).Add();
    }
}