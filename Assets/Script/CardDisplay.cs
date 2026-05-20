using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class CardDisplay : MonoBehaviour
{
    public TMP_Text displayText;
    public Firestoreinicialize cardAccess;

    void Start()
    {
        cardAccess = GameObject.FindGameObjectWithTag("BD").GetComponent<Firestoreinicialize>();
    }
    public void OnTargetFound(Transform imageTargetTransform)
    {
        string cardName = imageTargetTransform.name;
        Debug.Log($"Carta enocntrada: {cardName}");

        displayText = imageTargetTransform.Find("Text").GetComponent<TextMeshPro>();
        if (displayText != null)
        {
            //Actividad 2. Recuperar datos desde firestore
            cardAccess.FetchCardDataFromFirestore(cardName, displayText);
        }
        else
        {
            Debug.LogError("Objeto de texto no encontrado");
        }
    }

    public void OnTargetLost(Transform imageTargetTransform)
    {
        displayText = imageTargetTransform.Find ("Text").GetComponent<TextMeshPro>();
        if (displayText != null)
        {
            displayText.text = "Buscando Carta...";
        }
        else
            Debug.LogError("Objeto de texto no encontrado");
    }
}
