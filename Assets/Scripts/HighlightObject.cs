using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour
{
    public Material human_original, animal_original, barrel_original, human_highlight, animal_highlight, barrel_highlight;
    public Transform CF_Human, AP_Animal, RR_Human, RR_Animal, DR_Human;

    private int layerMask;
    private bool isHighlight;
    private int selectedCharacter;

    private GameObject highlightedBarrel;
    // Start is called before the first frame update
    void Start()
    {
        layerMask = LayerMask.GetMask("Highlight Objects");
        isHighlight = false;
        selectedCharacter = 0;
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));

        if(Physics.Raycast(ray, out hit, layerMask))
        {
            GameObject hitObject = hit.transform.gameObject;

            switch (hit.collider.tag)
            {
                case "CF":
                    highlightObject(hitObject, "human");
                    selectedCharacter = 1;
                    break;
                case "AP":
                    highlightObject(hitObject, "animal");
                    selectedCharacter = 2;
                    break;
                case "RR":
                    highlightObject(hitObject, "animal");
                    isHighlight = false;
                    highlightObject(RR_Human.gameObject, "human");
                    selectedCharacter = 3;
                    break;
                case "DR":
                    highlightObject(hitObject, "human");
                    selectedCharacter = 4;
                    break;
                case "Barrel_CF":
                    highlightObject(hitObject, "barrel");
                    selectedCharacter = 0;
                    break;
                case "Barrel_AP":
                    highlightObject(hitObject, "barrel");
                    selectedCharacter = 0;
                    break;
                case "Barrel_RR":
                    highlightObject(hitObject, "barrel");
                    selectedCharacter = 0;
                    break;
                case "Barrel_DR":
                    highlightObject(hitObject, "barrel");
                    selectedCharacter = 0;
                    break;
                default:
                    unHighlightObject(selectedCharacter);
                    selectedCharacter = 0;
                    break;
            }
        }
    }

    private void highlightObject(GameObject hitObj, string objType)
    {
        if (!isHighlight)
        {
            if (objType == "animal")
            {
                hitObj.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = animal_highlight;
            }
            else if (objType == "human")
            {
                hitObj.transform.GetChild(0).gameObject.GetComponent<Renderer>().material = human_highlight;
            }
            else if(objType == "barrel")
            {
                hitObj.GetComponent<Renderer>().material = barrel_highlight;
                highlightedBarrel = hitObj;
            }

            isHighlight = true;
        }
            
    }

    private void unHighlightObject(int selectedObj)
    {
        if (isHighlight)
        {
            switch (selectedObj)
            {
                case 0:
                    highlightedBarrel.GetComponent<Renderer>().material = barrel_original;
                    break;
                case 1:
                    CF_Human.GetChild(0).gameObject.GetComponent<Renderer>().material = human_original;
                    break;
                case 2:
                    AP_Animal.GetChild(0).gameObject.GetComponent<Renderer>().material = animal_original;
                    break;
                case 3:
                    RR_Animal.GetChild(0).gameObject.GetComponent<Renderer>().material = animal_original;
                    RR_Human.GetChild(0).gameObject.GetComponent<Renderer>().material = human_original;
                    break;
                case 4:
                    DR_Human.GetChild(0).gameObject.GetComponent<Renderer>().material = human_original;
                    break;
                default:
                    break;
            }

            isHighlight = false;
        }
    }
}
