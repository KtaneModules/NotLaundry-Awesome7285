using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using Rnd = UnityEngine.Random;

public class NotLaundry : MonoBehaviour
{
    #region BombStuff
    private class Battery
    {
        public int numbatteries;
    }
    private class Serial
    {
        public string serial;
    }
    private class Indicator
    {
        public string label;
        public string on;
    }
    private class Ports
    {
        public string[] PresentPorts;
    }

    #endregion

    #region Imports
    //Bomb info
    public KMBombInfo bombInfo;

    //Different selectables
    public KMSelectable coinSlot;
    public KMSelectable[] knobs;
    public KMSelectable[] slidersTop; //useless
    public KMSelectable[] slidersBottom; //useless

    //Displays
    public TextMesh ironingTextDisplay;
    public TextMesh specialTextDisplay;

    public Material[] washingDisplay; //Symbols for left dial MUST BE 12 LENGTH
    public Material[] dryingDisplay; //Symbols for right dial
    public Material[] coinSlotColours; // Red and Green

    public MeshRenderer leftKnobDisplay;
    public MeshRenderer rightKnobDisplay;
    public MeshRenderer coinSlotColour;

    //Audio
    public KMAudio sound;

    //Module itself   
    //public KMBombModule bombModule;
    #endregion

    #region Tables

    // Step 1 table
    // idk what an Enum[] is so we're doing it this way
    private static string[] lettersIron =       {"Y", "B", "B", "G", "N", "W", "I", "W", "R", "Q", "B", "H", "W"};
    private static string[] lettersDoNotIron =  {"N", "U", "L", "J", "F", "N", "U", "D", "P", "P", "V", "Z", "Q"};
    private static string[] letters110C =       {"L", "I", "V", "C", "R", "X", "K", "G", "O", "C", "R", "F", "D"};
    private static string[] letters300F =       {"Y", "S", "S", "M", "A", "D", "I", "J", "H", "A", "K", "T", "X"};
    private static string[] letters200C =       {"T", "C", "Z", "E", "T", "A", "M", "K", "O", "E", "M", "Q", "P"};
    private static string[] lettersNoSteam =    {"Z", "O", "S", "L", "J", "V", "Y", "F", "G", "X", "E", "H", "U"};
    private static string[][] letterTable = { lettersIron, lettersDoNotIron, letters110C, letters300F, letters200C, lettersNoSteam };

    // Different texts for showing on the module and logging
    private static string[] washingText = { "Machine Wash Permanent Press", "Machine Wash Gentle or Delicate", "Hand Wash", "Do Not Wash", "30°C", "40°C", "50°C", "60°C", "70°C", "95°C", "Do Not Wring", "Empty" };
    private static string[] dryingText = { "Tumble Dry", "1 Dot", "2 Dot", "3 Dot", "No Heat", "Hang to Dry", "Drip Dry", "Dry Flat", "Dry in the Shade", "Do Not Dry", "Do Not Tumble Dry", "Dry" };
    private static string[] ironingText = { "Iron", "Do Not Iron", "110°C", "300°F", "200°C", "No Steam" };
    private static string[] specialText = { "Bleach", "Don't Bleach", "No Chlorine", "Dryclean", "Any Solvent", "No Tetrachlore", "Petroleum Only", "Wet Cleaning", "Don't Dryclean", "Short Cycle", "Reduced Moist", "Low Heat", "No Steam Finish" };
    private static string[] coinSlotColourNames = { "Green", "Red" };
    private static string[] mathWords = { "Even", "Odd" };

    #endregion

    #region Internals

    //Index positions for changes
    private int ironingTextPos = 0;
    private int specialTextPos = 0;
    private int leftKnobPos = 0;
    private int rightKnobPos = 0;
    private int randomSlotColour = 0; // 0=Green 1=Red

    //Info from bomb
    private int totalIndicators = 0;
    private int batteryHolders = 0;
    private int totalPorts = 0;
    private int totalBatteries = 0;
    private int lastDigitSerial = 0;
    private int totalModules = 0;

    //Angle to rotate knobs
    private float dialRotate = 30.0f; // 360 / 12

    // Actual solution variables (Not Laundry)
    private string solutionLetter = "";
    private string correctPress = "None";
    private int presses = 0;

    //Solution and if solved
    private bool isSolved = false;

    #endregion


    private static int ModuleIdCounter = 1;

    private int ModuleId;

    void Awake () {
        ModuleId = ModuleIdCounter++;

        // Button Interacts
        coinSlot.OnInteract += () => { UseCoin(); return false; }; 
        knobs[0].OnInteract += () => { LeftKnob(); return false; };
        knobs[1].OnInteract += () => { RightKnob(); return false; };
    }

    void Start() {

        //Add handler for bomb activation
        GetComponent<KMBombModule>().OnActivate += GetBombValues;
        //bombModule.OnActivate += GetBombValues;

        // Randomize the dial positions
        // Always will have 12 rotation options
        // 0 = up
        leftKnobPos = Rnd.Range(0, 12);
        rightKnobPos = Rnd.Range(0, 12);

        knobs[0].transform.Rotate(new Vector3(0, dialRotate*leftKnobPos-90, 0));
        knobs[1].transform.Rotate(new Vector3(0, dialRotate*rightKnobPos-90, 0));
        leftKnobDisplay.material =  washingDisplay[leftKnobPos];
        rightKnobDisplay.material =  dryingDisplay[rightKnobPos];

        // Randomize the displays
        ironingTextPos = Rnd.Range(0, ironingText.Length); //6
        specialTextPos = Rnd.Range(0, specialText.Length); //13
        ironingTextDisplay.text = ironingText[ironingTextPos];
        specialTextDisplay.text = specialText[specialTextPos];

        // Find Letter in table 1
        solutionLetter = letterTable[ironingTextPos][specialTextPos];

        // Randomize the coin slot colour
        randomSlotColour = Rnd.Range(0, 2);
        coinSlotColour.material = coinSlotColours[randomSlotColour];

        Debug.LogFormat("[Not Laundry #{0}] The dial rotations are {1} ({3}) for left and {2} ({4}) for right.", ModuleId, leftKnobPos, rightKnobPos, washingText[leftKnobPos], dryingText[rightKnobPos]);
        Debug.LogFormat("[Not Laundry #{0}] The coin slot colour is {1}.", ModuleId, coinSlotColourNames[randomSlotColour]);
        Debug.LogFormat("[Not Laundry #{0}] The displays are {1} for top and {2} for bottom.", ModuleId, ironingText[ironingTextPos], specialText[specialTextPos]);
        Debug.LogFormat("[Not Laundry #{0}] The letter used for the ruleset is {1}.", ModuleId, solutionLetter);

        // Do the thing
        GetComponent<KMBombModule>().OnActivate += CalculateCorrectPress;
    }

    // Get all of the bomb info 
    void GetBombValues()
    {

        //Get ports stuff
        List<string> portList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string port in portList)
        {
            Ports i = JsonConvert.DeserializeObject<Ports>(port);

            totalPorts += i.PresentPorts.Length;

        }

        //Get Serial number information.
        Serial serialNumber = JsonConvert.DeserializeObject<Serial>(bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null)[0]);
        lastDigitSerial = serialNumber.serial[5] - '0';

        //Get batteries information
        List<string> batteryList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        batteryHolders = batteryList.Count;


        foreach (string batteries in batteryList)
        {
            Battery i = JsonConvert.DeserializeObject<Battery>(batteries);
            totalBatteries += i.numbatteries;
        }

        //Get indicators information and check for lit bob
        List<string> indicatorList = bombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        totalIndicators = indicatorList.Count;

        totalModules = bombInfo.GetSolvableModuleNames().Count;

        // totalBatteries = bombInfo.GetBatteryCount();
        // batteryHolders = bombInfo.GetBatteryHolderCount();
        // totalIndicators = bombInfo.GetIndicators();
        // totalPorts = bombInfo.GetPortCount();
        // lastDigitSerial = bombInfo.GetSerialNumberNumbers().Last();
        // totalModules = bombInfo.GetModuleNames().Count();
        // Debug.LogFormat("{0} The edgework is: bat: {1}, bathol: {2}, ind:{3}, port:{4}, SNlast:{5}, module:{6}.", ModuleId, totalBatteries, batteryHolders, totalIndicators, totalPorts, lastDigitSerial, totalModules);
        
    }

    void CalculateCorrectPress() {
        int theValue = -1;
        int evenCheck = 0;

        // Third Press
        if (presses == 2) {
            theValue = leftKnobPos + rightKnobPos;

            // Is the letter a check if even rule
            string evenLetters = "CEFGHJKLNOTVZ";
            if (evenLetters.Contains(solutionLetter)) {
                evenCheck = 0;
            } else {
                evenCheck = 1;
            }
        }
        // Second press
        if (presses == 1) {
            switch (solutionLetter) {
                case "A": theValue = leftKnobPos + totalIndicators; break;
                case "B": theValue = leftKnobPos + totalBatteries; break;
                case "C": theValue = leftKnobPos + totalIndicators; break;
                case "D": theValue = rightKnobPos + totalBatteries; break;
                case "E": theValue = leftKnobPos + totalModules; break;
                case "F": theValue = rightKnobPos + totalPorts; break;
                case "G": theValue = leftKnobPos + batteryHolders; break;
                case "H": theValue = rightKnobPos + batteryHolders; break;
                case "I": theValue = leftKnobPos + totalPorts; break;
                case "J": theValue = leftKnobPos + totalBatteries; break;
                case "K": theValue = rightKnobPos + totalBatteries; break;
                case "L": theValue = leftKnobPos + totalPorts; break;
                case "M": theValue = leftKnobPos + totalModules; break;
                case "N": theValue = rightKnobPos + totalIndicators; break;
                case "O": theValue = rightKnobPos + totalBatteries; break;
                case "P": theValue = leftKnobPos + batteryHolders; break;
                case "Q": theValue = rightKnobPos + totalIndicators; break;
                case "R": theValue = rightKnobPos + totalPorts; break;
                case "S": theValue = rightKnobPos + totalBatteries; break;
                case "T": theValue = leftKnobPos + totalPorts; break;
                case "U": theValue = rightKnobPos + totalPorts; break;
                case "V": theValue = rightKnobPos + totalPorts; break;
                case "W": theValue = leftKnobPos + totalPorts; break;
                case "X": theValue = rightKnobPos + totalBatteries; break;
                case "Y": theValue = rightKnobPos + batteryHolders; break;
                case "Z": theValue = rightKnobPos + totalBatteries; break;

            }
        }
        // First press
        if (presses == 0) {
            switch (solutionLetter) {
                case "A": theValue = rightKnobPos + totalModules; break;
                case "B": theValue = rightKnobPos + lastDigitSerial; break;
                case "C": theValue = rightKnobPos + totalModules; break;
                case "D": theValue = leftKnobPos + lastDigitSerial; break;
                case "E": theValue = rightKnobPos + totalIndicators; break;
                case "F": theValue = leftKnobPos + totalBatteries; break;
                case "G": theValue = rightKnobPos + totalBatteries; break;
                case "H": theValue = leftKnobPos + totalBatteries; break;
                case "I": theValue = rightKnobPos + totalIndicators; break;
                case "J": theValue = rightKnobPos + lastDigitSerial; break;
                case "K": theValue = leftKnobPos + lastDigitSerial; break;
                case "L": theValue = rightKnobPos + totalIndicators; break;
                case "M": theValue = rightKnobPos + totalIndicators; break;
                case "N": theValue = leftKnobPos + totalModules; break;
                case "O": theValue = leftKnobPos + totalModules; break;
                case "P": theValue = rightKnobPos + totalBatteries; break;
                case "Q": theValue = leftKnobPos + totalModules; break;
                case "R": theValue = leftKnobPos + lastDigitSerial; break;
                case "S": theValue = leftKnobPos + totalModules; break;
                case "T": theValue = rightKnobPos + totalBatteries; break;
                case "U": theValue = leftKnobPos + totalBatteries; break;
                case "V": theValue = leftKnobPos + lastDigitSerial; break;
                case "W": theValue = rightKnobPos + totalBatteries; break;
                case "X": theValue = leftKnobPos + totalIndicators; break;
                case "Y": theValue = leftKnobPos + totalBatteries; break;
                case "Z": theValue = leftKnobPos + totalIndicators; break;

            }
        }

        // this code could be used for press 2 aswell, not press 3 tho
        Debug.LogFormat("[Not Laundry #{0}] The value calculated for rule {2} is {1}.", ModuleId, theValue, presses+1);

        // If both value is even & colour is green or both not, return left otherwise right
        if ((theValue % 2 == evenCheck) ^ (randomSlotColour == 0)) {
            correctPress = "Right";
        } else {
            correctPress = "Left";
        }
        

        //correctPress = (theValue % 2 == 0) == (randomSlotColour == 0) ? 'left' : 'right';
        Debug.LogFormat("[Not Laundry #{0}] The value is {1} and the coin slot colour is {2}, which means the correct dial to press is {3}.", ModuleId, mathWords[theValue % 2], coinSlotColourNames[randomSlotColour], correctPress);
    }

    // Submit Function
    void SubmitDialPress (string thisDial) {
        // Check if correct press is the dial being pressed
        if (thisDial == correctPress) {
            Debug.LogFormat("[Not Laundry #{0}] You pressed {1}. That was correct.", ModuleId, thisDial);
            presses++;
        } else {
            Debug.LogFormat("[Not Laundry #{0}] You pressed {1}. That was incorrect.", ModuleId, thisDial);
            HandleStrike();
        }
        

        // Solve
        if (presses == 3) {
            HandlePass();
            isSolved = true;
        }
    }

    // Functions for the knob buttons.
    void LeftKnob()
    {
        if (!isSolved) {
            string thisDial = "Left";
            SubmitDialPress(thisDial);
            leftKnobPos++;
            leftKnobPos = (leftKnobPos + washingDisplay.Length) % washingDisplay.Length; //idk why it adds again
            knobs[0].transform.Rotate(new Vector3(0, dialRotate, 0)); // maybe this adds the rotation relative? so it's not added from 0 every time
            leftKnobDisplay.material =  washingDisplay[leftKnobPos]; // update material
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, knobs[0].transform); // play sound
            if (!isSolved) { // ok this looks stupid but its to prevent it from calculating a "stage 4"
                CalculateCorrectPress();
            }
        }
    }

    void RightKnob()
    {
        if (!isSolved) {
            string thisDial = "Right";
            SubmitDialPress(thisDial);
            rightKnobPos++;
            rightKnobPos = (rightKnobPos + dryingDisplay.Length) % dryingDisplay.Length;
            knobs[1].transform.Rotate(new Vector3(0, dialRotate, 0));
            rightKnobDisplay.material = dryingDisplay[rightKnobPos];
            sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, knobs[1].transform);
            if (!isSolved) {
                CalculateCorrectPress();
            }
        }
    }
    

     

    

    //Functions for handling pass and strike, really useless, but nice to have.
    void HandlePass()
    {
        GetComponent<KMBombModule>().HandlePass();
    }

    void HandleStrike()
    {
        GetComponent<KMBombModule>().HandleStrike();
    }


    //Called when the coin slot is activated
    void UseCoin()
    {
        //Play the sound, it's a laundry machine after all
        sound.PlaySoundAtTransform("Insert Coin", coinSlot.transform);

    }


    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"Use !{0} press left, or !{0} press right to press one of the dial buttons. Use !{0} press coinslot to play the funny coin sound.";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command) {
        command = command.Trim().ToLower();
        yield return null;
        string[] parameters = command.Split(' ');
        if (parameters[0] == "press") {
            if (parameters[1] == "left") {
                knobs[0].OnInteract();
            }
            else if (parameters[1] == "right") {
                knobs[1].OnInteract();
            }
            else if (parameters[1] == "coinslot") {
                coinSlot.OnInteract();
            } else {
                yield return "sendtochaterror Invalid Button.";
                yield break;
            }

        } else {
            yield return "sendtochaterror Invalid Command.";
            yield break;
        }

    }

    // Auto solve
    IEnumerator TwitchHandleForcedSolve () {
        while (isSolved == false) {
            if (correctPress == "Left") {
                knobs[0].OnInteract();
            }
            if (correctPress == "Right") {
                knobs[1].OnInteract();
            }
            yield return new WaitForSeconds(.1f);
        }
    }
}
