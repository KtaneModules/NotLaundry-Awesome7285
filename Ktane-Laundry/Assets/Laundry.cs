﻿using UnityEngine;
using System.Collections.Generic;
using Newtonsoft.Json;

public class Laundry : MonoBehaviour
{

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
    




    public KMBombInfo BombInfo;
    public KMSelectable Coinslot;
    public KMSelectable[] Knobs;
    public KMSelectable[] SlidersTop;
    public KMSelectable[] SlidersBottom;
    public TextMesh IroningTextDisplay;
    public TextMesh SpecialTextDisplay;

    public Material[] WashingDisplay;
    public Material[] DryingDisplay;

    public KMAudio Sound;
    public MeshRenderer LeftKnobDisplay;
    public MeshRenderer RightKnobDisplay;

    public static int[,] ClothingType = { { 7, 7, 3, 0 }, { 5, 3, 5, 5 }, { 4, 5, 0, 10 }, { 1, 0, 4, 10 }, { 10, 8, 3, 1 }, { 9, 11, 2, 8 } };
    public static int[,] MaterialType = { { 6, 4, 3, 6 }, { 9, 2, 0, 8 }, { 2, 8, 4, 10 }, { 4, 6, 5, 11 }, { 5, 6, 2, 7 }, { 3, 8, 1, 5 } };
    public static int[,] ColorType = { { 7, 3, 1, 4 }, { 9, 7, 0, 11 }, { 4, 0, 4, 9 }, { 4, 10, 3, 12 }, { 0, 1, 5, 11 }, { 7, 2, 4, 2, } };
      
    public static string[] IroningText = { "Iron", "Don't Iron", "110°C", "300°F", "200°C", "No Steam" };
    public static string[] SpecialText = { "Bleach", "Don't Bleach", "No Chlorine", "Dryclean", "Any Solvent", "No Tetrachlore", "Petroleum Only", "Wet Cleaning", "Don't Dryclean", "Short Cycle", "Reduced Moist", "Low Heat", "No Steamfinish" };
    public static string[] MaterialNames = { "polyester", "cotton", "wool", "nylon", "corduroy", "leather" };

    private int IroningTextPos = 0;
    private int SpecialTextPos = 0;
    private int LeftKnobPos = 0;
    private int RightKnobPos = 0;
    private int TotalIndicators = 0;
    private int BatteryHolders = 0;
    private int TotalPorts = 0;
    private int TotalBatteries = 0;
    private int LastDigitSerial = 0;
    private bool HasBOB = false;
    private string SerialNum = ""; 
    

    void IroningSlide (int sign)
    {
        
        IroningTextPos += sign;
        IroningTextPos = (IroningTextPos + IroningText.Length) % IroningText.Length;
        IroningTextDisplay.text = IroningText[IroningTextPos]; 
    }

    void SpecialSlide (int sign)
    {
        SpecialTextPos += sign;
        SpecialTextPos = (SpecialTextPos + SpecialText.Length) % SpecialText.Length;
        SpecialTextDisplay.text = SpecialText[SpecialTextPos];
    }

    void LeftKnob()
    {
        LeftKnobPos ++ ;
        LeftKnobPos = (LeftKnobPos + WashingDisplay.Length) % WashingDisplay.Length;
        Knobs[0].transform.Rotate(new Vector3(0, 360.0f/ WashingDisplay.Length, 0));
        LeftKnobDisplay.material =  WashingDisplay[LeftKnobPos];
        Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Knobs[0].transform);
    }

    void RightKnob()
    {
        RightKnobPos ++ ;
        RightKnobPos = (RightKnobPos + DryingDisplay.Length) % DryingDisplay.Length;
        Knobs[1].transform.Rotate(new Vector3(0, 360.0f / DryingDisplay.Length, 0));
        RightKnobDisplay.material = DryingDisplay[RightKnobPos];
        Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, Knobs[1].transform);
    }

    void GetBombValues()
    {

        List<string> PortList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS, null);
        foreach (string Portals in PortList)
        {
            Ports i = JsonConvert.DeserializeObject<Ports>(Portals);

            TotalPorts += i.PresentPorts.Length;
        
        }
          Serial SerialNumber = JsonConvert.DeserializeObject<Serial>(BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER, null)[0]);
          LastDigitSerial = SerialNumber.serial[5] - '0';
          SerialNum = SerialNumber.serial.ToLower();

        List<string> BatteryList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        BatteryHolders = BatteryList.Count;


        foreach (string Batteries in BatteryList)
        {
            Battery i = JsonConvert.DeserializeObject<Battery>(Batteries);
            TotalBatteries += i.numbatteries;
        }


        List<string> IndicatorList = BombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        TotalIndicators = IndicatorList.Count;

        foreach (string Indicators in IndicatorList)
        {
            Indicator i = JsonConvert.DeserializeObject<Indicator>(Indicators);
            if (i.on == "True" && i.label == "BOB") 
            {
                HasBOB = true;
            }
        } 

    } 

    int[] GetSolutionValues()
    {

        if (HasBOB && TotalBatteries == 4 && BatteryHolders == 2)
        {
            return new int[1];
        }

        int[] SolutionStates = new int[4];
        int ClothingItem = (BombInfo.GetSolvableModuleNames().Count - BombInfo.GetSolvedModuleNames().Count + TotalIndicators + 6) % 6;
        int ItemMaterial = (TotalPorts + BombInfo.GetSolvedModuleNames().Count - BatteryHolders + 6) % 6;
        int ItemColor = (LastDigitSerial + TotalBatteries + BombInfo.GetStrikes() + 6) % 6 ;

        bool CloudedPearl = ItemColor == 4;
        bool LeatherJadeCluster = ItemMaterial == 5 || ItemColor == 3;
        bool CorsetCorduroy = ClothingItem == 0 || ItemMaterial == 4;
        bool WoolStarLemon = ItemMaterial == 2 || ItemColor == 1;
        bool MaterialSerial = false;

        foreach (char c in MaterialNames[ItemMaterial])
        {
            if (SerialNum.Contains(c+""))
            {
                MaterialSerial = true;
            }
        }
        if (CloudedPearl)
        {
            SolutionStates[3] = 2;
        }
        else if (CorsetCorduroy)
        {
            SolutionStates[3] = MaterialType[ItemMaterial, 3];
        }
        else if (MaterialSerial)
        {
            SolutionStates[3] = ColorType[ItemColor, 3];
        }
        else
        {
            SolutionStates[3] = ClothingType[ClothingItem, 3];
        }


        if (LeatherJadeCluster)
        {
            SolutionStates[0] = 4;
        }
        else
        {
            SolutionStates[0] = MaterialType[ItemMaterial, 0];
        }


        if (WoolStarLemon)
        {
            SolutionStates[1] = 3;
        }
        else
        {
            SolutionStates[1] = ColorType[ItemColor, 1];
        }
        SolutionStates[2] = ClothingType[ClothingItem, 2];

        return SolutionStates;
    }


    void HandlePass()
    {
        GetComponent<KMBombModule>().HandlePass();
    }

    void HandleStrike()
    {
        GetComponent<KMBombModule>().HandleStrike();
    }


    void UseCoin()
    {
        int[] Solution = GetSolutionValues();
        if (Solution.Length == 1)
        {
            HandlePass();
            return;
        }
        bool WashingCorrect = Solution[0] == LeftKnobPos;
        bool DryingCorrect = Solution[1] == RightKnobPos;
        bool IroningCorrect = Solution[2] == IroningTextPos;
        bool SpecialCorrect = Solution[3] == SpecialTextPos;

        if (WashingCorrect && DryingCorrect && IroningCorrect && SpecialCorrect)
        {
            HandlePass();
        }
        else
        {
            HandleStrike();
        }
    }








    void Start()
    {

     GetComponent<KMBombModule>().OnActivate += GetBombValues;
      
        for (int i = 0; i < SlidersTop.Length; i++)
        {
            var j = i;
            SlidersTop[i].OnInteract += delegate () { IroningSlide(j * 2 - 1); Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SlidersTop[j + 1 - 1 ].transform); return false; };

        }
        for (int i = 0; i < SlidersBottom.Length; i++)
        {
            var j = i;
            SlidersBottom[i].OnInteract += delegate () { SpecialSlide(j * 2 - 1); Sound.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, SlidersBottom[j + 1 - 1 ].transform); return false; };

        }

        Coinslot.OnInteract += delegate () { Sound.PlaySoundAtTransform("Insert Coin", Coinslot.transform);  UseCoin(); return false; }; 

        Knobs[0].OnInteract += delegate ()
        { LeftKnob(); return false; };

        Knobs[1].OnInteract += delegate ()
        { RightKnob(); return false; };

        IroningTextDisplay.text = IroningText[IroningTextPos];
        SpecialTextDisplay.text = SpecialText[SpecialTextPos];

        //LeftKnobDisplay.material.SetTexture("_MainTex", WashingDisplay[LeftKnobPos]);
        //RightKnobDisplay.material.SetTexture("_MainTex", DryingDisplay[RightKnobPos]);
        
    }

   
}
