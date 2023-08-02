using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class Player
{
    //GroundState groundStateScript

    void FixedUpdate()
    {
        groundStateScript.isGround();
    }
}
