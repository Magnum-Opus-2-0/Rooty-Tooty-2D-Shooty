using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CheatManager : MonoBehaviour
{

    /// <summary>
    /// Size of the buffer (so it cannot grow infinitely big).
    /// This effectively caps the length of a cheat code
    /// at a maximum of LIMIT characters long.
    /// </summary>
    private const int LIMIT = 10;

    /// <summary>
    /// Reference to the BoardController.
    /// Needed for the explode cheat.
    /// </summary>
    public BoardController boardController;


    public GameObject player1Tank;
    public GameObject player2Tank;

    public List<AudioClip> pew;
    public List<AudioClip> no;
    private int no_i;
    private int pew_i;

    /// <summary>
    /// When true, this class will print the buffer to Debug.Log
    /// when a button (keyboard or mouse) is pressed.
    /// When false, there will be no Debug.Log prints.
    /// </summary>
    public bool printBuffer;

    /// <summary>
    /// Buffer which keeps track of this CheatManager's string.
    /// All characters in this buffer are lowercase.
    /// Can be modified using this class's public member functions.
    /// Can be printed to Debug.Log if printBuffer is enabled.
    /// </summary>
    private string buffer;

    private void Start() {

        buffer = "";
        no_i = -1;
        pew_i = -1;
    }

    private void Update() {

        //debugRotateUpdate();

        //if (Input.GetKeyDown(KeyCode.Mouse0)) {

        //    Debug.Log("no_i: " + no_i);
        //    player1Tank.GetComponent<AudioSource>().Play();
        //}

        if (Input.anyKeyDown) {

            grabInput();

            activateCheats();

            if (printBuffer) Debug.Log(buffer);
        }


    }

    /// <summary>
    /// Tests end of buffer,
    /// activating a cheat if one is detected.
    /// To add cheat code functionality, add it here.
    /// Please add new cheats in order of how long the cheat code is,
    /// i.e. put short cheats at the top of this function
    /// and longer ones at the bottom.
    /// </summary>
    private void activateCheats() {

        bool successfulCheat = false;

        /*
        if (containsAtEnd("no") || no_i != -1) {

            // this is kinda needlessly expensive,
            // but don't mark to reset
            // if you're only in here because i is cycling
            successfulCheat = containsAtEnd("no");

            Debug.Log("no_i before modification: " + no_i);

            no_i++;
            no_i %= no.Count;

            Debug.Log("no_i after modification: " + no_i);



            // TODO
            // Move this functionality to ShootController,
            // or whoever can detect if a shot has been shot

            AudioSource p1_as = player1Tank.GetComponent<AudioSource>();
            AudioSource p2_as = player2Tank.GetComponent<AudioSource>();

            p1_as.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;
            p2_as.velocityUpdateMode = AudioVelocityUpdateMode.Dynamic;

            if (p1_as.isPlaying) p1_as.Stop();
            if (p2_as.isPlaying) p2_as.Stop();

            p1_as.clip = no[no_i];
            p2_as.clip = no[no_i];
        }

        else if (containsAtEnd("pew") || pew_i != -1) {

            successfulCheat = containsAtEnd("no");

            pew_i++;
            pew_i %= pew.Count;

            player1Tank.GetComponent<AudioSource>().clip = pew[pew_i];
            player2Tank.GetComponent<AudioSource>().clip = pew[pew_i];
        }
        

        else*/ if (containsAtEnd("boom")) {

            successfulCheat = true;
            boardController.absolutelyFuckingUncouple();
        }

        else if (containsAtEnd("smol")) {

            successfulCheat = true;
            player1Tank.transform.localScale *= 0.5f;
            player2Tank.transform.localScale *= 0.5f;
        }

        else if (containsAtEnd("jumbo")) {

            successfulCheat = true;
            player1Tank.transform.localScale *= 1.5f;
            player2Tank.transform.localScale *= 1.5f;
        }

        else if (containsAtEnd("win1") || containsAtEnd("lose2")) {

            successfulCheat = true;
            boardController.p2Base.GetComponent<HealthBehavior>().setHealth(0);
        }
        
        else if (containsAtEnd("win2") || containsAtEnd("lose1")) {

            successfulCheat = true;
            boardController.p1Base.GetComponent<HealthBehavior>().setHealth(0);
        }

        // Reset buffer if they cheated,
        // otherwise you'll apply the cheat every frame until buffer is changed
        if (successfulCheat) resetBuffer();
    }

    /// <summary>
    /// Updates buffer with keyboard input.
    /// If characters were entered this frame, they are appended.
    /// If the backspace key was pressed,
    /// a char is deleted from the right of buffer.
    /// Does nothing if the user presses enter/return.
    /// </summary>
    private void grabInput() {

        string input = Input.inputString;

        foreach (char c in input) {

            // Backspace
            if (c == '\b') {

                deleteChar();
            }

            // Enter/return
            else if (c == '\n') {


            }

            // any other ascii character
            else {

                insertChar(char.ToLower(c));
            }
        }
    }

    /// <summary>
    /// For debug purposes. Do not call this when you ship the build.
    /// If left mouse button is pressed during this frame,
    /// buffer is rotated left by 1.
    /// Else if right mouse button is pressed during this frame,
    /// buffer is rotated right by 1.
    /// </summary>
    private void debugRotateUpdate() {

        if (Input.GetKeyDown(KeyCode.Mouse0)) {

            buffer = rotateLeft(buffer);
        }

        else if (Input.GetKeyDown(KeyCode.Mouse1)) {

            buffer = rotateRight(buffer);
        }
    }

    /// <summary>
    /// Appends a char to the buffer.
    /// If the char would overflow the buffer's size,
    /// rotates the entire buffer left once
    /// before appending the character to maintain the constant size.
    /// </summary>
    /// <param name="c"></param>
    public void insertChar(char c) {

        if (buffer.Length < LIMIT) {

            buffer = buffer + c;
        }

        else {

            buffer = rotateLeft(buffer);
            buffer = replaceLastChar(c);
        }
    }

    /// <summary>
    /// Deletes the rightmost char from buffer.
    /// This method is "safe",
    /// so calling deleteChar on an empty buffer
    /// will simply do nothing.
    /// </summary>
    public void deleteChar() {

        if (buffer.Length <= 1) {

            buffer = "";
            return;
        }
        else {
            buffer = buffer.Substring(0, buffer.Length - 1);
        }
    }

    /// <summary>
    /// Checks if a string occurs at the end of buffer.
    /// For example, if the buffer holds "bulbasaur",
    /// then containsAtEnd("bulbasaur") and containsAtEnd("saur") return true.
    /// </summary>
    /// <param name="compareMe"></param>
    /// <returns></returns>
    public bool containsAtEnd(string compareMe) {

        if (compareMe.Length > buffer.Length)
            return false;

        int substringStart = buffer.Length - compareMe.Length;

        return buffer.Substring(substringStart).Equals(compareMe);
    }

    private string replaceLastChar(char c) {

        Assert.IsTrue(buffer.Length > 0, "Tried to replaceLastChar on an empty string");

        char[] tempArray = buffer.ToCharArray();
        tempArray[tempArray.Length - 1] = c;

        return new string(tempArray);
    }

    private string rotateLeft(string s, int n) {

        string temp = new string(s.ToCharArray());

        for (int thisRotate = 0; thisRotate < n; thisRotate++)
            temp = rotateLeft(temp);

        return temp;
    }

    private string rotateLeft(string s) {

        char[] temp = s.ToCharArray();

        for (int i = 0; i < temp.Length - 1; i++) {

            temp[i] = temp[i + 1];
        }

        return new string(temp);
    }

    private string rotateRight(string s) {

        char[] temp = s.ToCharArray();

        //for (int i = temp.Length - 1; i < 0; i--) {

        //    temp[i] = temp[i - 1];
        //}

        for (int i = temp.Length - 2; i >= 0; i--) {

            temp[i + 1] = temp[i];
        }

        return new string(temp);
    }

    private string rotateRight(string s, int n) {

        string temp = new string(s.ToCharArray());

        for (int thisRotate = 0; thisRotate < n; thisRotate++) {

            temp = rotateRight(s);
        }

        return temp;
    }

    private bool isFull() {

        return buffer.Length >= LIMIT;
    }

    private void resetBuffer() {

        buffer = "";
    }

}
