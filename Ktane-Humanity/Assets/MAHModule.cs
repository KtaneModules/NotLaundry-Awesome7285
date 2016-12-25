using UnityEngine;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using System;

public class MAHModule : MonoBehaviour {

    public KMSelectable[] LeftCardButtons;
    public KMSelectable[] RightCardButtons;
    public KMSelectable ResetButton;
    public KMSelectable AcceptButton;
    public TextMesh LeftCard;
    public TextMesh RightCard;
    public KMBombInfo mBombInfo;
    public KMBombModule mBombModule;
    public static Material LeftCardMat;
    public static Material RightCardMat;
    public MeshRenderer LeftCardMesh;
    public MeshRenderer RightCardMesh;
    public KMGameInfo mGameInfo;
    public TextMesh DummyMesh;
    public KMAudio Audio;

    private const int AMOUNT_OF_CARDS = 10;

    private static string[] WhiteModuleIDs = { "wire sequence", "simon says", "maze", "memory", "needy capacitor", "who's on first", "needy vent gas", "modules against humanity", "needy knob", "morse code", "two bits", "anagrams", "word scramble", "semaphore", "colour flash", "logic", "listening", "shift puzzle", "crazy talk", "silly slots", "probing", "forget me not", "morsematics", "simon states", "perspective pegs", "caesar cipher", "tic tac toe", "astrology", "adventure game", "skewed slots", "blind alley", "english test", "mouse in themaze", "turn the keys", "turn the key", "tetris", "sea shells", "murder","adjacent letters","colored squares" ,"hexamaze" ,"souvenir" };
    private static string[] BlackModuleIDs = { "the button", "password", "wires", "keypad", "complicated wires", "chess module", "switches", "emoji math", "lettered keys", "orientation cube", "piano keys", "connection check", "cryptography", "number pad", "alphabet", "round keypad", "plumbing", "safety safe", "resistors", "microcontroller", "the gamepad", "laundry", "3d maze", "follow the leader", "friendship", "the bulb", "monsplode, fight!", "foreign exchange rates", "combination lock", "shape shift", "needy math", "lights out", "motion sense", "needy rotary phone", "needy advanced questions", "monsplode, who?", "filibuster","third base","bitmaps","rock-paper-scissors-l.-sp.","square button", "broken buttons", "word search" };

    private static Dictionary<string, string> ModuleTexts = new Dictionary<string, string>() {
        { "wire sequence", "Trickier than it sounds; all the wires are hidden behind panels." },
        { "the button", "Push the button. Galvanize." },
        { "simon says", "Too late. simon says you should try again." },
        { "password", "You need to know the secret password to defuse this bomb." },
        { "maze", "I know you feel lost. Sadly, this is not a maze so there's no way out." },
        { "wires", "Cut all the wires!" },
        { "memory", "Nope, that doesn't work. Try memorizing the manual next time." },
        { "keypad", "If you recognize any of these symbols, you're a smart fella." },
        { "needy capacitor", "You're on the dishonorable discharge, Take your capacitor and leave." },
        { "complicated wires", "Cut all the wires! It's not that complicated after all." },
        { "who's on first", "You're clearly the last, but who's first?" },
        { "chess module", "I would play even Billy the Puppet if that game was chess." },
        { "needy vent gas", "Stop venting your emotions. Vent gas instead." },
        { "switches", "It’s useless. Switch to another bomb." },
        { "modules against humanity", "Yes, this bomb is very inhumane. Deal with it." },
        { "emoji math" , "Did you know your texts can include words, and not just emojis?" },
        { "needy knob", "You can play with this knob even when you're done with the bomb." },
        { "lettered keys", "Wait, now keys have letters on them? What is this sorcery?" },
        { "morse code" , "If you're bad with words, try Morse code instead." },
        { "orientation cube", "Bad luck with your crush? Maybe it’s just their sexual orientation." },
        { "two bits", "Here's my two bits of information: you're gonna explode." },
        { "anagrams", "The instructions were clear and not in anagrams; you're an idiot." },
        { "piano keys", "Musical instruments? I recommend piano, cruel one or not." },
        { "connection check", "Bomb exploded due to bad wifi connection." },
        { "word scramble", "You got scrambled by your own sword." },
        { "cryptography", "Your mind is so cryptic, I can't read it." },
        { "semaphore", "Just as I expected, you're waving a white flag already." },
        { "number pad", "Keypad here, keypad there. I want a keypad with numbers!" },
        { "colour flash", "That almost got you dead. Your whole life flashed before your eyes." },
        { "alphabet", "Never try tongue twisters before you learn alphabet." },
        { "logic", "Do you have at least a pinch of logic in that head of yours?" },
        { "round keypad", "You spin my head right round with those fancy symbols." },
        { "listening", "Just got a strike. You need to listen to your experts better." },
        { "plumbing", "If you don't find a way out, try crawling through pipes." },
        { "shift puzzle", "You need serious help. From a shaman or a mystic, nothing less." },
        { "safety safe", "This bomb is so valuable it should be locked in a safe." },
        { "crazy talk", "You're talking crazy. Get out." },
        { "resistors", "Don't resist the temptation. Just do it." },
        { "silly slots", "You failed. What a silly slot you are." },
        { "microcontroller", "Trust your experts, don’t micro control them." },
        { "probing", "Bad news. You're actually being probed by aliens." },
        { "the gamepad", "Two kinds of people: those who use gamepad and the boring ones." },
        { "forget me not", "Well done. You forgot it not." },
        { "laundry", "Friends are coming over. Quick, hide all your dirty laundry!" },
        { "morsematics", "If you're bad with words, try Morsematics code instead." },
        { "3d maze", "Want to see this in 3D? There are special glasses for it." },
        { "simon states", "Too late. Simon states you should try again." },
        { "follow the leader", "If this bomb explodes, you're following the wrong political leader." },
        { "perspective pegs", "If you can't do it, try to look from a different perspective." },
        { "friendship", "They say Monopoly ruins friendships. They have not tried KTANE." },
        { "caesar cipher", "You dealt with it like a true Caesar." },
        { "the bulb", "You think you have a great idea? Nah, that's just a bulb above your head." },
        { "tic tac toe", "Lots of love, XOXO, The Bomb." },
        { "monsplode, fight!", "Why fight your family when you can fight Monsplodes? " },
        { "astrology", "No success? I guess your zodiac was under the wrong planet's influence." },
        { "foreign exchange rates", "No currency exchange point? You should've taken your card." },
        { "adventure game", "That's okay. There will be another adventure waiting for you soon." },
        { "combination lock", "Are you sure this is the combination you need?" },
        { "skewed slots", "Well, that's what happens when your world view is so skewed." },
        { "shape shift", "Why be a human when you can shapeshift into an owl?" },
        { "blind alley", "You're done. No way out from this blind alley." },
        { "needy math", "Math is hard." },
        { "english test", "Do you know the difference between your crap and you're crap?" },
        { "lights out", "Lights are out. Bomb's unavailable for 10 seconds." },
        { "mouse in themaze", "Tsk. Poor little mouse, can't do anything right." },
        { "motion sense", "Moves like Jagger? Not with that motion sense of yours." },
        { "turn the keys", "Just turn the keys and leave the room." },
        { "needy rotary phone", "Here, phone a friend for help. Old school style." },
        { "turn the key", "Just turn the key and leave the room." },
        { "needy advanced questions", "I know you hate answering questions, but we do have to test you." },
        { "tetris", "If you're tired of bombs, you could always play Tetris instead." },
        { "sea shells", "Who cares what she sells? She's not gonna sell it to you." },
        { "monsplode, who?", "Fighting Monsplodes? Identify them first." },
        { "murder", "If this doesn't get you murdered, I don't know what will." },
        { "filibuster", "Remember the title and just keep talking" },
        { "adjacent letters", "Bomb exploded. I was adjacent to it." },
        { "third base", "Don't tell me you're already in third base with this bomb?" },
        { "colored squares" , "It's cool to be square but only if your colored." },
        { "bitmaps" ,  "Lost in this bomb? Here's a bit of a map."},
        { "hexamaze", "Someone must have put a-maze-ing hex on you." },
        { "rock-paper-scissors-l.-sp.", "You're fighting a lizard. But don't use caber, use scissors." },
        { "square button", "Push the square button. Galvanize." },
        { "broken buttons", "Pressing these buttons is useless. They’re broken." },
        { "souviner", "This explosion will leave a few souvenirs on your body." },
        { "word search", "You’re searching for a word? It’s the bird." }
    };

    private List<string> WhiteCardText;
    private List<string> BlackCardText;

    private List<string> LeftCardText;
    private List<string> RightCardText;

    private Material LeftCardMaterial;
    private Material RightCardMaterial;

    private int LeftCardIndex;
    private int RightCardIndex;

    private int CorrectLeftCardIndex;
    private int CorrectRightCardIndex;

    private bool activated = false;


    private bool SwapWhiteBlack;
    private float limit;

    
    public static float GetWidth(TextMesh mesh) {
        float width = 0;
        foreach (char symbol in mesh.text) {
            CharacterInfo info;
            if (mesh.font.GetCharacterInfo(symbol, out info, mesh.fontSize, mesh.fontStyle)) {
                width += info.advance;
            }
        }
        return width * mesh.characterSize * 0.1f;
    }

    string FitTextInCard(string text) {
        
        StringBuilder txt = new StringBuilder();
        DummyMesh.text = "a";
        float spaceLength = GetWidth(DummyMesh);
        DummyMesh.text = "a a";
        spaceLength = GetWidth(DummyMesh) - spaceLength - spaceLength;
        float currAmount = 0;
        string[] split = text.Split(' ');
        txt.Append(split[0]);
        DummyMesh.text = split[0];
        currAmount = GetWidth(DummyMesh);
        for(int i = 1; i < split.Length; i++) {
            txt.Append(" ");
            DummyMesh.text = split[i];
            float currLength = GetWidth(DummyMesh);
            if((currAmount + spaceLength + currLength) > limit) {
                txt.Append("\n");
                currAmount = currLength;
            }else {
                currAmount += spaceLength + currLength;
            }
            
            txt.Append(DummyMesh.text);
        }
        return txt.ToString();
    }

    void UpdateLeftCardText() {
        LeftCardIndex += BlackCardText.Count;
        LeftCardIndex %= BlackCardText.Count;
        LeftCard.text = LeftCardText[LeftCardIndex];
    }

    void UpdateRightCardText() {
        RightCardIndex += BlackCardText.Count;
        RightCardIndex %= BlackCardText.Count;
        RightCard.text = RightCardText[RightCardIndex];
    }
    
    bool CheckSolve() {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, AcceptButton.transform);
        if (!activated)
            return false;
        StringBuilder s = new StringBuilder();
        s.AppendFormat("[ModulesAgainstHumanity] Submitted: Left: {0}, Right: {1} : ",LeftCardIndex,RightCardIndex);

        if(LeftCardIndex == CorrectLeftCardIndex && RightCardIndex == CorrectRightCardIndex) {
            s.Append("Pass");
            mBombModule.HandlePass();
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.CorrectChime,this.transform);
        } else {
            s.Append("Strike");
            mBombModule.HandleStrike();
            ResetButtonInteract();
        }
        Debug.Log(s.ToString());
        return false;
    }
    bool SpellPoop(string text) {
        int pCount = 0, oCount = 0;
        text = text.ToLower();
        foreach(char s in text) {
            if (s == 'p')
                pCount++;
            else if (s == 'o')
                oCount++;
        }
        return (pCount > 1 && oCount > 1);
    }

    int GetPortCount(ref int uniques) {
        int i = 0;
        List<string> portTypes = new List<string>();
        foreach(string s in mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_PORTS,null)) {
            foreach(string c in JsonConvert.DeserializeObject<Dictionary<string, string[]>>(s)["presentPorts"]) {
                if (!portTypes.Contains(c)) {
                    portTypes.Add(c);
                }
                i++;
            }
        }
        uniques = portTypes.Count;
        return i;
    }

    void GetLitUnlitCount(out int lit, out int unlit) {
        lit = 0;
        unlit = 0;
        List<string> Response = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_INDICATOR, null);
        foreach (string Value in Response) {
            Dictionary<string, string> IndicatorInfo = JsonConvert.DeserializeObject<Dictionary<string, string>>(Value);
            if (IndicatorInfo["on"] == "True") {
                lit++;
            } else if (IndicatorInfo["on"] == "False") {
                unlit++;
            }
        }
    }

    int GetNumberOfBatteries() {
        int i = 0;
        List<string> Response = mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_BATTERIES, null);
        foreach (string Value in Response) {
            Dictionary<string, int> batteryInfo = JsonConvert.DeserializeObject<Dictionary<string, int>>(Value);
            i += batteryInfo["numbatteries"];
        }
        return i;
    }

    bool GetSerialMAH() {
        string s = JsonConvert.DeserializeObject<Dictionary<string, string>>(mBombInfo.QueryWidgets(KMBombInfo.QUERYKEY_GET_SERIAL_NUMBER,null)[0])["serial"].ToLower();
        return s.Contains("m") || s.Contains("a") || s.Contains("h");
    }

    void OnActivate() {
        activated = true;
        List<string> moduleNames = mBombInfo.GetModuleNames();

        for(int i = 0; i < moduleNames.Count; i++) {
            moduleNames[i] = moduleNames[i].ToLower();
        }
        

        int tempWhiteIndex = 0;
        int tempBlackIndex = 0;
        bool containsMAH = GetSerialMAH();
        int uniquePorts = 0;
        int ports = GetPortCount(ref uniquePorts);

        int tempLit, tempUnlit;
        GetLitUnlitCount(out tempLit, out tempUnlit);
        if (SpellPoop(ModuleTexts[BlackCardText[tempBlackIndex]])) {
            tempBlackIndex = 1;
        } else {
            tempBlackIndex = (tempUnlit + ports-1 + BlackCardText.Count) % BlackCardText.Count;
        }

        if (SpellPoop(ModuleTexts[WhiteCardText[tempWhiteIndex]])) {
            tempWhiteIndex = 1;
        } else {
            int i = GetNumberOfBatteries();
            tempWhiteIndex = (tempLit + i - 1 + BlackCardText.Count) % BlackCardText.Count;
        }
        


        if (moduleNames.Contains(BlackCardText[tempBlackIndex]) && moduleNames.Contains(WhiteCardText[tempWhiteIndex])) {
            tempBlackIndex += 4;
            tempBlackIndex %= BlackCardText.Count;
            tempWhiteIndex += 3;
            tempWhiteIndex %= BlackCardText.Count;
        }else if (moduleNames.Contains(BlackCardText[tempBlackIndex])) {
            tempWhiteIndex += 2;
            tempWhiteIndex %= BlackCardText.Count;
        }else if (moduleNames.Contains(WhiteCardText[tempWhiteIndex])) {
            tempBlackIndex += 1;
            tempBlackIndex %= BlackCardText.Count;
        } else {
            if (containsMAH) {
                tempWhiteIndex -= 2;
                tempBlackIndex -= 2;
            }else if (!SwapWhiteBlack) {
                tempWhiteIndex = tempLit + tempUnlit - 1;
                tempBlackIndex = uniquePorts - 1;
            }else {
                tempBlackIndex = moduleNames.Count - 1;
            }

            tempWhiteIndex += BlackCardText.Count;
            tempBlackIndex += BlackCardText.Count;
            tempWhiteIndex %= BlackCardText.Count;
            tempBlackIndex %= BlackCardText.Count;
        }
        if (!SwapWhiteBlack) {
            CorrectLeftCardIndex = (tempBlackIndex + BlackCardText.Count) % BlackCardText.Count;
            CorrectRightCardIndex = (tempWhiteIndex + BlackCardText.Count) % BlackCardText.Count;
        } else {
            CorrectLeftCardIndex = (tempWhiteIndex + BlackCardText.Count) % BlackCardText.Count;
            CorrectRightCardIndex = (tempBlackIndex + BlackCardText.Count) % BlackCardText.Count;
        }

        Debug.Log("[ModulesAgainstHumanity] SwapWhiteBlack: " + SwapWhiteBlack.ToString() + ", CorrectLeft: " + CorrectLeftCardIndex.ToString() + ", CorrectRight: " + CorrectRightCardIndex.ToString());
        
    }

    void ChooseRandomCardTexts() {
        BlackCardText = new List<string>();
        WhiteCardText = new List<string>();
        StringBuilder Blacks = new StringBuilder();
        StringBuilder Whites = new StringBuilder();

        for(int i = 0; i < AMOUNT_OF_CARDS; i++) {
            string blackChosen = "";
            do {
                blackChosen = BlackModuleIDs[UnityEngine.Random.Range(0, BlackModuleIDs.Length - 1)];
            } while (BlackCardText.Contains(blackChosen));
            BlackCardText.Add(blackChosen);
            string whiteChosen = "";
            do {
                whiteChosen = WhiteModuleIDs[UnityEngine.Random.Range(0, WhiteModuleIDs.Length - 1)];
            } while (WhiteCardText.Contains(whiteChosen));
            WhiteCardText.Add(whiteChosen);
            Blacks.Append(blackChosen + "|");
            Whites.Append(whiteChosen + "|");
        }
        Debug.LogFormat("[ModulesAgainstHumanity] Chosen black cards: {0}, Chosen white cards {1}",Blacks.ToString(),Whites.ToString());
    }

    bool ResetButtonInteract() {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, ResetButton.transform);
        LeftCardIndex = 0;
        UpdateLeftCardText();
        RightCardIndex = 0;
        UpdateRightCardText();
        return false;
    }

    // Use this for initialization
    void Start () {
        limit = GetWidth(DummyMesh);

        if(LeftCardMat == null) {
            LeftCardMat = LeftCardMesh.material;
        }
        if(RightCardMat == null) {
            RightCardMat = RightCardMesh.material;
        }

        SwapWhiteBlack = UnityEngine.Random.Range(0, 1000) % 2 == 0;
        LeftCardMaterial = Instantiate(LeftCardMat);
        RightCardMaterial = Instantiate(RightCardMat);
        LeftCardMesh.material = LeftCardMaterial;
        RightCardMesh.material = RightCardMaterial;
        
        mGameInfo.OnLightsChange += OnLightsChange;

        if (!SwapWhiteBlack) {
            LeftCardMaterial.color = Color.black;
            RightCardMaterial.color = Color.white;
            LeftCard.color = Color.white;
            RightCard.color = Color.black;
        } else {
            LeftCardMaterial.color = Color.white;
            RightCardMaterial.color = Color.black;
            LeftCard.color = Color.black;
            RightCard.color = Color.white;
        }

        for (int i = 0; i < LeftCardButtons.Length; i++) {
            int j = i;
            LeftCardButtons[j].OnInteract += () => { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, LeftCardButtons[j].transform); LeftCardIndex += (2 * j) - 1; UpdateLeftCardText(); return false; };
        }

        for (int i = 0; i < RightCardButtons.Length; i++) {
            int j = i;
            RightCardButtons[j].OnInteract += () => { Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, RightCardButtons[j].transform); RightCardIndex += (2 * j) - 1; UpdateRightCardText(); return false; };
        }



        ResetButton.OnInteract += ResetButtonInteract;

        ChooseRandomCardTexts();
        GenerateCardTexts();

        
        AcceptButton.OnInteract += CheckSolve;
        mBombModule.OnActivate += OnActivate;
        
    }
    

    private void OnLightsChange(bool on) {
        if (on) {
            UpdateLeftCardText();
            UpdateRightCardText();
        }else {
            LeftCard.text = "";
            RightCard.text = "";
        }
    }
    private void GenerateCardTexts() {
        LeftCardText = new List<string>();
        RightCardText = new List<string>();
        for(int i = 0; i < BlackCardText.Count; i++) {
            string black = BlackCardText[i];
            string white = WhiteCardText[i];
            if (!SwapWhiteBlack) {
                LeftCardText.Add(FitTextInCard(ModuleTexts[black]));
                RightCardText.Add(FitTextInCard(ModuleTexts[white]));
            }else {
                LeftCardText.Add(FitTextInCard(ModuleTexts[white]));
                RightCardText.Add(FitTextInCard(ModuleTexts[black]));
            }
        }
    }
}
