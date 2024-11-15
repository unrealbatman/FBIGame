using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public  class Evidence : MonoBehaviour, IExaminable 


/*Represents any piece of collectible evidence in the crime scene.
Methods: collectEvidence(), examine(), getClueInfo()
Subclasses:
WeaponEvidence: Specific type of evidence, such as a weapon, with additional methods for retrieving specific weapon data (e.g., getFingerprintInfo()).
ClothingEvidence: Evidence type related to clothing, potentially with distinct interaction methods.
MiscellaneousEvidence: For items that do not fit into specific evidence categories but can still be examined for clues.
*/


{



    public string clueDescription;


   
    public void Examine()
    {

        //This method must  be concerend with uopdating UI elements and displaying clue info

        Debug.Log("Examined Object: "+this.gameObject.name);

    }





}
