using System;
using UnityEngine;

[Serializable]
public class EvidenceData
{
    public string clueName; // Name of the evidence
    public string clueDescription; // Description of the evidence
    public Sprite clueIcon; // Icon representing the evidence

    public EvidenceData(string name, string description, Sprite icon)
    {
        clueName = name;
        clueDescription = description;
        clueIcon = icon;
    }
}
