using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextureRandomizer : RandomMaterialBehavior
{

    public List<Texture> textures;

    /// <summary>
    /// Sets the GameObject's Material to a texture randomly from the given list.
    /// </summary>
    public override void Randomize()
    {
        rend.material.mainTexture = textures[Random.Range(0, textures.Count)];
    }
}
