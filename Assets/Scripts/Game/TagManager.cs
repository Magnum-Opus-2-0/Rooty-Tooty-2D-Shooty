using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Manager class for different tags.
/// You must manually edit the code to support new tags.
/// Go to init() and add your new tag into the corresponding section of code.
/// Note that if your new tag is shootable,
/// you will likely need to add your tag to more than 1 area of code.
/// </summary>
public class TagManager
{

    public TagManager() {
        if (p1Tags == null)
        { 
            init();
        }
    }

    public bool tagExists(string s) {

        return allTags.Contains(s);
    }

    public bool isP1Tag(string s) {

        return p1Tags.Contains(s);
    }

    public bool isP2Tag(string s) {

        return p2Tags.Contains(s);
    }

    public bool isShootable(string s) {

        return shootables.Contains(s);
    }

    public bool isMinion(string s)
    {
        return s == "P1_Teddy" || s == "P2_Teddy" || s == "P1_Soldier"
            || s == "P2_Soldier";
    }

    public bool isSpawner(string s)
    {
        return s == "P1_Spawner_Soldier" || s == "P1_Spawner_Teddy"
            || s == "P2_Spawner_Soldier" || s == "P2_Spawner_Teddy";
    }

    public bool isStructure(string s) {

        return structures.Contains(s);
    }



    private void init() {

        // Only populate the static maps once
        if (allTags != null && allTags.Count > 0) return;



        allTags = new HashSet<string>();
        p1Tags = new HashSet<string>();
        p2Tags = new HashSet<string>();
        shootables = new HashSet<string>();
        structures = new HashSet<string>();

        #region Player 1 tags
        // Manually do Player1 tags
        // If you need to insert a new p1Tag/p2Tag,
        // just add it here
        p1Tags.Add("Player_1");
        p1Tags.Add("Cam_1");
        p1Tags.Add("P1canvas");
        p1Tags.Add("Camera1");
        p1Tags.Add("P1_Base");
        p1Tags.Add("Player1_obj");
        p1Tags.Add("P1_Bullet");
        p1Tags.Add("P1_Soldier");
        p1Tags.Add("P1_Spawner");
        p1Tags.Add("P1_Turret");
        p1Tags.Add("P1_Healer");
        p1Tags.Add("P1_Teddy");
        p1Tags.Add("P1_Spawner_Soldier");
        p1Tags.Add("P1_Spawner_Teddy");
        #endregion

        #region Player 2 tags
        // Do Player2 tags based on Player 1
        foreach (string str in p1Tags) {

            string copy = new string(str.ToCharArray());
            copy = copy.Replace('1', '2');

            Assert.IsTrue(!copy.Contains("1"), "String.Replace() didn't work as intended");

            p2Tags.Add(copy);
        }
        Assert.IsTrue(p1Tags.Count == p2Tags.Count);
        #endregion

        #region Shootables
        // Do shootables manually
        shootables.Add("P1_Bullet");
        shootables.Add("P2_Bullet");
        shootables.Add("P1_Soldier");
        shootables.Add("P2_Soldier");
        shootables.Add("P1_Spawner");
        shootables.Add("P2_Spawner");
        shootables.Add("P1_Turret");
        shootables.Add("P2_Turret");
        shootables.Add("P1_Healer");
        shootables.Add("P2_Healer");
        shootables.Add("P1_Teddy");
        shootables.Add("P2_Teddy");
        shootables.Add("P1_Base");
        shootables.Add("P2_Base");
        shootables.Add("P1_Spawner_Soldier");
        shootables.Add("P1_Spawner_Teddy");
        shootables.Add("P2_Spawner_Soldier");
        shootables.Add("P2_Spawner_Teddy");


        #endregion

        #region Structures
        // Do structures manually
        structures.Add("P1_Spawner");
        structures.Add("P2_Spawner");
        structures.Add("P1_Turret");
        structures.Add("P2_Turret");
        structures.Add("P1_Healer");
        structures.Add("P2_Healer");
        structures.Add("P1_Base");
        structures.Add("P2_Base");
        shootables.Add("P1_Spawner_Soldier");
        shootables.Add("P1_Spawner_Teddy");
        shootables.Add("P2_Spawner_Soldier");
        shootables.Add("P2_Spawner_Teddy");

        #endregion

        #region All tags
        // Do allTags as a combo of the previous ones,
        // plus more
        allTags.UnionWith(p1Tags);
        allTags.UnionWith(p2Tags);
        allTags.UnionWith(shootables);
        allTags.UnionWith(structures);

        allTags.Add("Floor");
        allTags.Add("Wall");
        allTags.Add("Resource");
        allTags.Add("Spawnpoint");
        allTags.Add("Master Minion Fondler");
        allTags.Add("Board");
        #endregion
    }

    private static HashSet<string> allTags;
    private static HashSet<string> p1Tags;
    private static HashSet<string> p2Tags;
    private static HashSet<string> shootables;
    private static HashSet<string> structures;
}
