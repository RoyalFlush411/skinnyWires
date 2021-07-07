using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;

public class wireModuleScript : MonoBehaviour
{
    public KMBombInfo Bomb;
    public KMAudio Audio;
    public KMSelectable[] allWires;
    public KMSelectable[] selectedWires;

    public GameObject[] wireObjects;
    public GameObject[] cutWireObjects;
    public Material[] wireMaterialOptions;
    private int[] selectedWireIndices = new int[5];
    public String[] selectedColours = new String[5];
    private String[] selectedLetterPorts = new String[5];
    private String[] selectedNumberPorts = new String[5];
    private List<int> selectedIndices = new List<int>();

    private List<string> coloursInA = new List<string>();
    private List<string> coloursInB = new List<string>();
    private List<string> coloursInC = new List<string>();
    private List<string> coloursIn1 = new List<string>();
    private List<string> coloursIn2 = new List<string>();
    private List<string> coloursIn3 = new List<string>();
    private bool rule14;
    private bool rule15;
    private bool rule16;
    private bool rule17;
    private bool rule18;
    private bool rule19;
    private bool rule20;
    private int correctRule = 0;

    //Logging
    static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

    void Awake()
    {
        moduleId = moduleIdCounter++;
    }


    void Start()
    {
        PickFiveWires();
        AssignSelectables();
        CalculateCorrectWire();
        LogCorrectWire();
    }

    void PickFiveWires()
    {
        for(int i = 0; i <= 8; i++)
        {
            wireObjects[i].SetActive(false);
        }
        for(int i = 0; i <= 4; i++)
        {
            selectedWireIndices[i] = UnityEngine.Random.Range(0,9);
            while(selectedIndices.Contains(selectedWireIndices[i]))
            {
                selectedWireIndices[i] = UnityEngine.Random.Range(0,9);
            }
            selectedIndices.Add(selectedWireIndices[i]);
            wireObjects[selectedWireIndices[i]].SetActive(true);
            int matIndex = UnityEngine.Random.Range(0,8);
            wireObjects[selectedWireIndices[i]].GetComponent<Renderer>().material = wireMaterialOptions[matIndex];
            selectedWires[i] = allWires[selectedWireIndices[i]];
            selectedWires[i].GetComponent<WireDetails>().wireColour = wireMaterialOptions[matIndex].name;
            selectedColours[i] = wireMaterialOptions[matIndex].name;
            selectedLetterPorts[i] = selectedWires[i].GetComponent<WireDetails>().letterIndex;
            selectedNumberPorts[i] = selectedWires[i].GetComponent<WireDetails>().numberIndex;
            selectedWires[i].GetComponent<WireDetails>().materialIndex = matIndex;
            Debug.LogFormat("[Skinny Wires #{0}] Wire #{1} is ported at {2}{3} and is {4}.", moduleId, i + 1, selectedWires[i].GetComponent<WireDetails>().letterIndex, selectedWires[i].GetComponent<WireDetails>().numberIndex, selectedWires[i].GetComponent<WireDetails>().wireColour);
            if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "1")
            {
                coloursIn1.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
            if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "2")
            {
                coloursIn2.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
            if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "3")
            {
                coloursIn3.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
            if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "A")
            {
                coloursInA.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
            if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "B")
            {
                coloursInB.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
            if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "C")
            {
                coloursInC.Add(selectedWires[i].GetComponent<WireDetails>().wireColour);
            }
        }
        selectedIndices.Clear();
    }

    void AssignSelectables()
    {
        foreach (KMSelectable wire in selectedWires)
        {
            KMSelectable clickedWire = wire;
            wire.OnInteract += delegate () { WireSnip(clickedWire); return false; };
        }
    }

    void CalculateCorrectWire()
    {
        if(selectedColours.Where((x) => x.Equals("red")).Count() == 1 && selectedColours.Where((x) => x.Equals("black")).Count() == 1 && selectedColours.Where((x) => x.Equals("white")).Count() == 1 && selectedColours.Where((x) => x.Equals("green")).Count() == 1 && selectedColours.Where((x) => x.Equals("orange")).Count() == 1)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "red")
                {
                    wire.GetComponent<WireDetails>().correctWire = true;
                    correctRule = 1;
                    return;
                }
            }
        }

        if(selectedLetterPorts.Where((x) => x.Equals("A")).Count() == 0)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "black" && wire.GetComponent<WireDetails>().numberIndex == "3")
                {
                    for(int i = 0; i <= 4; i++)
                    {
                        if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "B")
                        {
                            selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                            correctRule = 2;
                        }
                    }
                    return;
                }
            }
        }

        if(selectedNumberPorts.Where((x) => x.Equals("2")).Count() == 2)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "green")
                {
                    if(wire.GetComponent<WireDetails>().numberIndex == "2")
                    {
                        foreach(KMSelectable wire2 in selectedWires)
                        {
                            wire2.GetComponent<WireDetails>().correctWire = true;
                            if(wire2.GetComponent<WireDetails>().numberIndex == "2")
                            {
                                wire2.GetComponent<WireDetails>().correctWire = false;
                            }

                        }
                        correctRule = 3;
                        return;
                    }
                }
            }
        }

        if(selectedColours.Distinct().Count() == 2)
        {
            if(selectedColours.Where((x) => x.Equals("black")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "black")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("blue")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "blue")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("green")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "green")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("orange")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "orange")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("pink")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "pink")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("red")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "red")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("white")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "white")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("yellow")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "yellow")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            correctRule = 4;
            return;
        }

        if((coloursIn1.Count() >= 3 && coloursIn1.Distinct().Count() == 1) || (coloursIn2.Count() >= 3 && coloursIn2.Distinct().Count() == 1) || (coloursIn3.Count() >= 3 && coloursIn3.Distinct().Count() == 1) || (coloursInA.Count() >= 3 && coloursInA.Distinct().Count() == 1) || (coloursInB.Count() >= 3 && coloursInB.Distinct().Count() == 1) || (coloursInC.Count() >= 3 && coloursInC.Distinct().Count() == 1))
        {
            if(selectedColours.Where((x) => x.Equals("yellow")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "yellow")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("white")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "white")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("red")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "red")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("pink")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "pink")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("orange")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "orange")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("green")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "green")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("blue")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "blue")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            else if(selectedColours.Where((x) => x.Equals("black")).Count() >= 1)
            {
                foreach(KMSelectable wire in selectedWires)
                {
                    if(wire.GetComponent<WireDetails>().wireColour == "black")
                    {
                        wire.GetComponent<WireDetails>().correctWire = true;
                    }
                }
            }
            correctRule = 5;
            return;
        }

        foreach(KMSelectable wire in selectedWires)
        {
            if(wire.GetComponent<WireDetails>().wireColour == "blue" && wire.GetComponent<WireDetails>().numberIndex == "3")
            {
                wire.GetComponent<WireDetails>().correctWire = true;
                correctRule = 6;
            }
        }
        if(correctRule == 6)
        {
            return;
        }

        if(selectedColours.Where((x) => x.Equals("green")).Count() == 1 && selectedColours.Where((x) => x.Equals("orange")).Count() >= 1)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "green")
                {
                    wire.GetComponent<WireDetails>().correctWire = true;
                    correctRule = 7;
                    return;
                }
            }
        }

        if(selectedColours.Where((x) => x.Equals("black")).Count() == 1 && selectedColours.Where((x) => x.Equals("white")).Count() == 1)
        {
            for(int i = 0; i <= 4; i++)
            {
                if(selectedWires[i].GetComponent<WireDetails>().wireColour == "black" && selectedWires[i].GetComponent<WireDetails>().numberIndex == "1")
                {
                    foreach(KMSelectable wire in selectedWires)
                    {
                        if(wire.GetComponent<WireDetails>().wireColour == "white" && wire.GetComponent<WireDetails>().numberIndex != "1")
                        {
                            wire.GetComponent<WireDetails>().correctWire = true;
                            correctRule = 8;
                            return;
                        }
                    }
                }
                else if(selectedWires[i].GetComponent<WireDetails>().wireColour == "white" && selectedWires[i].GetComponent<WireDetails>().numberIndex == "1")
                {
                    foreach(KMSelectable wire in selectedWires)
                    {
                        if(wire.GetComponent<WireDetails>().wireColour == "black" && wire.GetComponent<WireDetails>().numberIndex != "1")
                        {
                            wire.GetComponent<WireDetails>().correctWire = true;
                            correctRule = 8;
                            return;
                        }
                    }
                }
            }
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedWires[i].GetComponent<WireDetails>().wireColour == "yellow" && selectedWires[i].GetComponent<WireDetails>().letterIndex == "C")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                correctRule = 9;
            }
            return;
        }

        if(selectedColours.Where((x) => x.Equals("pink")).Count() > 1)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "pink")
                {
                    wire.GetComponent<WireDetails>().correctWire = true;
                    correctRule = 10;
                }
            }
            return;
        }

        if(selectedColours.Where((x) => x.Equals("red")).Count() >= 1 && selectedColours.Where((x) => x.Equals("orange")).Count() >= 1 && selectedColours.Where((x) => x.Equals("blue")).Count() == 0)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().wireColour == "orange")
                {
                    wire.GetComponent<WireDetails>().correctWire = true;
                    correctRule = 11;
                }
            }
            return;
        }

        if(selectedNumberPorts.Where((x) => x.Equals("3")).Count() == 0)
        {
            foreach(KMSelectable wire in selectedWires)
            {
                if(wire.GetComponent<WireDetails>().numberIndex == "1")
                {
                    wire.GetComponent<WireDetails>().correctWire = true;
                    correctRule = 12;
                }
            }
            return;
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "A" && selectedWires[i].GetComponent<WireDetails>().numberIndex == "2")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                correctRule = 13;
                return;
            }
        }

        if(selectedColours.Where((x) => x.Equals("green")).Count() == 0)
        {
            for(int i = 0; i <=4; i++)
            {
                if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "A")
                {
                    selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                    rule14 = true;
                    correctRule = 14;
                }
            }
            if(!rule14)
            {
                for(int i = 0; i <=4; i++)
                {
                    if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "B")
                    {
                        selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                        rule14 = true;
                        correctRule = 14;
                    }
                }
            }
            if(!rule14)
            {
                for(int i = 0; i <=4; i++)
                {
                    if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "C")
                    {
                        selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                        rule14 = true;
                        correctRule = 14;
                    }
                }
            }
            return;
        }

        if(selectedColours.Where((x) => x.Equals("blue")).Count() == 0)
        {
            for(int i = 0; i <=4; i++)
            {
                if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "3")
                {
                    selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                    rule15 = true;
                    correctRule = 15;
                }
            }
            if(!rule15)
            {
                for(int i = 0; i <=4; i++)
                {
                    if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "2")
                    {
                        selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                        rule15 = true;
                        correctRule = 15;
                    }
                }
            }
            if(!rule15)
            {
                for(int i = 0; i <=4; i++)
                {
                    if(selectedWires[i].GetComponent<WireDetails>().numberIndex == "1")
                    {
                        selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                        rule15 = true;
                        correctRule = 15;
                    }
                }
            }
            return;
        }

        for(int i = 1; i <=4; i++)
        {
            if(selectedColours[0] == selectedColours[i])
            {
                selectedWires[0].GetComponent<WireDetails>().correctWire = true;
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule16 = true;
            }
        }
        for(int i = 2; i <= 4; i++)
        {
            if(selectedColours[1] == selectedColours[i])
            {
                selectedWires[1].GetComponent<WireDetails>().correctWire = true;
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule16 = true;
            }
        }
        for(int i = 3; i <= 4; i++)
        {
            if(selectedColours[2] == selectedColours[i])
            {
                selectedWires[2].GetComponent<WireDetails>().correctWire = true;
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule16 = true;
            }
        }
        if(selectedColours[3] == selectedColours[4])
        {
            selectedWires[3].GetComponent<WireDetails>().correctWire = true;
            selectedWires[4].GetComponent<WireDetails>().correctWire = true;
            rule16 = true;
        }

        if(rule16)
        {
            correctRule = 16;
            return;
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedColours[i] == "yellow")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule17 = true;
            }
        }
        if(rule17)
        {
            correctRule = 17;
            return;
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedColours[i] == "black")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule18 = true;
            }
        }
        if(rule18)
        {
            correctRule = 18;
            return;
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedColours[i] == "white")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule19 = true;
            }
        }
        if(rule19)
        {
            correctRule = 19;
            return;
        }

        for(int i = 0; i <= 4; i++)
        {
            if(selectedWires[i].GetComponent<WireDetails>().letterIndex == "A")
            {
                selectedWires[i].GetComponent<WireDetails>().correctWire = true;
                rule20 = true;
            }
        }
        if(rule20)
        {
            correctRule = 20;
            return;
        }
    }

    void LogCorrectWire()
    {
        Debug.LogFormat("[Skinny Wires #{0}] The correct rule is #{1}.", moduleId, correctRule);
        for(int i = 0; i <=4; i++)
        {
            if(selectedWires[i].GetComponent<WireDetails>().correctWire)
            {
                Debug.LogFormat("[Skinny Wires #{0}] You may cut {1}{2}.", moduleId, selectedWires[i].GetComponent<WireDetails>().letterIndex, selectedWires[i].GetComponent<WireDetails>().numberIndex);
            }
        }
        Debug.LogFormat("[Skinny Wires #{0}] Cutting any other wire will yield a strike.", moduleId, correctRule);
    }

    void WireSnip(KMSelectable clickedWire)
    {
        Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.WireSnip, clickedWire.transform);
        clickedWire.AddInteractionPunch();
        cutWireObjects[clickedWire.GetComponent<WireDetails>().wireIndex].SetActive(true);
        cutWireObjects[clickedWire.GetComponent<WireDetails>().wireIndex].GetComponentInChildren<Renderer>().material = wireMaterialOptions[clickedWire.GetComponent<WireDetails>().materialIndex];
        if(moduleSolved)
        {
            Debug.LogFormat("[Skinny Wires #{0}] Strike! Continuing to cut wires after the module has been disarmed can only end badly.", moduleId);
            GetComponent<KMBombModule>().HandleStrike();
            clickedWire.GetComponent<WireDetails>().wireObject.SetActive(false);
            return;
        }
        if(clickedWire.GetComponent<WireDetails>().correctWire)
        {
            Debug.LogFormat("[Skinny Wires #{0}] You cut wire {1}{2}. That is correct. Module disarmed.", moduleId, clickedWire.GetComponent<WireDetails>().letterIndex, clickedWire.GetComponent<WireDetails>().numberIndex);
            GetComponent<KMBombModule>().HandlePass();
            moduleSolved = true;
        }
        else
        {
            Debug.LogFormat("[Skinny Wires #{0}] Strike! You cut wire {1}{2}. That is incorrect.", moduleId, clickedWire.GetComponent<WireDetails>().letterIndex, clickedWire.GetComponent<WireDetails>().numberIndex);
            GetComponent<KMBombModule>().HandleStrike();
        }
        clickedWire.GetComponent<WireDetails>().wireObject.SetActive(false);
    }
}
