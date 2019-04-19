using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorRandomizer : RandomMaterialBehavior
{
    public List<Color> colors;

    public override void Randomize()
    {
        rend.material.color = colors[Random.Range(0, colors.Count)];
    }
}
