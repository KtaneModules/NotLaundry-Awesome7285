using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enigma : MonoBehaviour {

    public KMSelectable[] Keypad;
    public KMSelectable SubmitButton;
    public KMSelectable ResetButton;
    public KMBombModule MBombModule;
    public KMGameInfo MInfo;
    public Transform[] RotorsTrans;
    public TextMesh Positions;
    public TextMesh WordMesh;
    


    #region consts
    private const string Rotor1 = "EKMFLGDQVZNTOWYHXUSPAIBRCJ";
    private const string Rotor2 = "AJDKSIRUXBLHWTMCQGZNPYFVOE";
    private const string Rotor3 = "BDFHJLCPRTXVZNYEIWGAKMUSQO";
    private const string Rotor4 = "ESOVPZJAYQUIRHXLNFTGKDCMWB";
    private const string Rotor5 = "VZBRGITYUPSDNHLXAWMJQOFECK";
    private const string Rotor6 = "JPGVOUMFYQBENHZRDKASXLICTW";
    private const string Rotor7 = "NZJHGRCXMYSWBOUFAIVLPEKQDT";
    private const string Rotor8 = "FKQHTLXOCBJSPDZRAMEWNIUYGV";

    private static string[] Rotors =
    {
        Rotor1,
        Rotor2,
        Rotor3,
        Rotor4,
        Rotor5,
        Rotor6,
        Rotor7,
        Rotor8
    };

    private static string[] RotorTexts =
    {
        "1",
        "2",
        "3",
        "4",
        "5",
        "6",
        "7",
        "8"
    };

    private const string ReflectB = "ENKQAUYWJICOPBLMDXZVFTHRGS";
    private const string ReflectC = "RDOBJNTKVEHMLFCWZAXGYIPSUQ";
    private const string ReflectD = "ZXWVUTSRQYPONMLKIHGFEDCBJA";

    private static string[] Reflectors =
    {
        ReflectB,
        ReflectC,
        ReflectD
    };

    private static string[] ReflectTexts = {
        "B",
        "C",
        "D"
    };

    private static char[] PosTexts = {
        'A',
        'B',
        'C',
        'D',
        'E',
        'F',
        'G',
        'H',
        'I',
        'J',
        'K',
        'L',
        'M',
        'N',
        'O',
        'P',
        'Q',
        'R',
        'S',
        'T',
        'U',
        'V',
        'W',
        'X',
        'Y',
        'Z',
    };

    private static int Reverse(string input, int pos, int rot)
    {
        char let = (char)('A' + ((pos + rot) % 26));
        for (int i = 0; i < input.Length; i++)
        {
            if (input[i] == let)
                return (i - rot + 26) % 26;
        }
        return -1;
    }

    private static int Forward(string input, int pos, int rot)
    {
        char c = input[(pos + rot) % 26];
        return (c - 'A' - rot + 26) % 26;
    }

    #endregion

    private static int moduleId = 0;
    private int myModuleId;
    private bool isActive = false;


    private int chosen1;
    private int chosen2;
    private int chosen3;
    private int chosenR;
    private int chosenP1;
    private int chosenP2;
    private int chosenP3;

    private string rot3;
    private string rot2;
    private string rot1;
    private string reflector;

    private int pos3 = 0;
    private int pos2 = 0;
    private int pos1 = 0;

    private string chosenWord = "";
    private string inputtedWord = "";
    private string enigmaWord = "";

    

    private char InputtedLetter(char input)
    {
        input = char.ToUpper(input);
        int firstLetter = Forward(rot3, input - 'A', pos3);
        int secondLetter = Forward(rot2, firstLetter, pos2);
        int thirdLetter = Forward(rot1, secondLetter, pos1);
        char reflected = reflector[thirdLetter];
        int rThird = Reverse(rot1, reflected-'A', pos1);
        int rSecond = Reverse(rot2, rThird, pos2 );
        int final = Reverse(rot3, rSecond, pos3);
        return (char)(final+'A');
    }

    private string EnigmaWord(string word)
    {
        pos1 = chosenP1;
        pos2 = chosenP2;
        pos3 = chosenP3;
        string outWord = "";
        foreach (char c in word)
        {
            outWord += InputtedLetter(c);
            pos3++;
            if (pos3 > 25)
            {
                pos3 = 0;
                pos2++;
                if(pos2 > 25)
                {
                    pos2 = 0;
                    pos1++;
                    if(pos1 > 25)
                        pos1 = 0;
                }
            }
        }
        return outWord;
    }

    string GetDisplay()
    {
        return inputtedWord + enigmaWord.Substring(inputtedWord.Length);
    }

    string GetPositions()
    {
        return pos1 + ", " + pos2 + ", " + pos3;
    }

    const float angle = 360f / 26f;
    void RotateRotors(int rot1, int rot2, int rot3)
    {
        RotorsTrans[0].Rotate(0, 0, -angle * rot1);
        RotorsTrans[1].Rotate(0, 0, -angle * rot2);
        RotorsTrans[2].Rotate(0, 0, -angle * rot3);
    }

    void OnLightsChange(bool state)
    {
        if (!isActive) return;
        for(int i =0; i < Keypad.Length; i++)
        {
            var mesh = Keypad[i].GetComponentInChildren<TextMesh>();
            if (state)
                mesh.text = "" + (char)('A' + i);
            else
                mesh.text = "";
        }
        if (state)
        {
            SubmitButton.GetComponentInChildren<TextMesh>().text = "✔";
            ResetButton.GetComponentInChildren<TextMesh>().text = "×";
            WordMesh.text = GetDisplay();
            Positions.text = GetPositions();
        }
        else
        {
            SubmitButton.GetComponentInChildren<TextMesh>().text = "";
            ResetButton.GetComponentInChildren<TextMesh>().text = "";
            WordMesh.text = "";
            Positions.text = "";
        }
        
    }

    void OnActivate()
    {
        isActive = true;
        OnLightsChange(true);
    }

    void OnKeypadInteract(char key)
    {

    }

    public Transform dial;
    public GameObject text;
    private float ang = 360f / 26f;
    private float rad = Mathf.PI/180;
	// Use this for initialization
	void Start () {
        myModuleId = moduleId++;
        /*float radius = text.transform.localPosition.x;
        for(int i = 0; i < 26; i++)
        {
            var c = Instantiate(text, dial);
            c.transform.localPosition = new Vector3(Mathf.Sin(rad *(( ang * i) + 90)) * radius, Mathf.Cos(rad * ((ang * i) + 90)) *radius,0);
            c.transform.localRotation = Quaternion.Euler(ang*-i, -90, 0);
            c.name = (char)((i) + 'A') + "Text";
            c.GetComponent<TextMesh>().text = (char)((i) + 'A') + "";
        }*/
        do
        {
            chosen1 = UnityEngine.Random.Range(0, Rotors.Length);
            chosen2 = UnityEngine.Random.Range(0, Rotors.Length);
            chosen3 = UnityEngine.Random.Range(0, Rotors.Length);
        } while (chosen1 == chosen2 || chosen1 == chosen3 || chosen2 == chosen3);
        chosenR = UnityEngine.Random.Range(0, Reflectors.Length);

        rot1 = Rotors[chosen1];
        rot2 = Rotors[chosen2];
        rot3 = Rotors[chosen3];
        reflector = Reflectors[chosenR];

        chosenP1 = UnityEngine.Random.Range(0, 26);
        chosenP2 = UnityEngine.Random.Range(0, 26);
        chosenP3 = UnityEngine.Random.Range(0, 26);
        chosenWord = "Halo";
        enigmaWord = EnigmaWord(chosenWord);
        MBombModule.OnActivate += OnActivate;
        MInfo.OnLightsChange += OnLightsChange;
        //RotateRotors(chosenP1, chosenP2, chosenP3);
        for(int i = 0; i < Keypad.Length; i++)
        {
            char c = (char)(i + 'A');
            Keypad[i].OnInteract = () => { OnKeypadInteract(c); return false; };
        }
    }
    


}
