using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

public abstract class Evidence : MonoBehaviour


/*Represents any piece of collectible evidence in the crime scene.
Methods: collectEvidence(), examine(), getClueInfo()
Subclasses:
WeaponEvidence: Specific type of evidence, such as a weapon, with additional methods for retrieving specific weapon data (e.g., getFingerprintInfo()).
ClothingEvidence: Evidence type related to clothing, potentially with distinct interaction methods.
MiscellaneousEvidence: For items that do not fit into specific evidence categories but can still be examined for clues.
*/


{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
