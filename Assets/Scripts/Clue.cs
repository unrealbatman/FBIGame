using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public  class Clue : MonoBehaviour


/*
Represents a clue derived from examining evidence.
Methods: matchWithSuspect(suspect: Suspect), addScoreForClue()
Subclasses:
PhysicalClue: Clues based on physical evidence (e.g., fingerprints).
BehavioralClue: Clues based on behavior or whereabouts of a suspect.
LocationClue: Clues specific to the location where the suspect may have been seen.*/


{
     public string clueName;
     public string description;
     public Image  clueIcon;
}
