using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Firebase.Extensions;
using Firebase.Firestore;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System;
using UnityEngine.SceneManagement;

public class Firestoreinicialize : MonoBehaviour
{
    private static FirebaseFirestore firestore;
    [SerializeField]
    private TMP_InputField cardNameInput;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        firestore = FirebaseFirestore.DefaultInstance;

    }

    public void FetchAndStoreCardData()
    {
        string cardName = cardNameInput.text;
        StartCoroutine(GetCardData(cardName));
    }

    //Actividad 1

    private IEnumerator GetCardData(string cardName)
    {
        string url = $"https://db.ygoprodeck.com/api/v7/cardinfo.php?name={UnityWebRequest.EscapeURL(cardName)}";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string json = request.downloadHandler.text;
            ProcessCardData(json);
        }
        else 
        {
            Debug.LogError("Error obteniendo datos: "+request.error);
        }
    }

    private void ProcessCardData(string json)
    {
        //Parsero JSON data
        var cardData = JsonUtility.FromJson<CardDataResponse>(json);

        if (cardData.data.Length > 0)
        {
            var card = cardData.data[0];
            string cardType = card.type;
            //Estructura en firestore
            string collection = DetermineCollection(cardType);
            string cardName = card.name;

            //Crear documento de referencia basado en el tipo de carta y su nombre
            DocumentReference docRef = firestore
                .Collection("cartas")
                .Document(collection)
                .Collection(cardName)
                .Document("Datos");

            //Preparar datos para guardar en Firestore

            var cardInfo = new Dictionary<string, object>
            {
                { "ATK",card.atk},
                { "DEF",card.def},
                { "Desc",card.desc},
                { "Level",card.level},
            };

            //Guardar datos en Firestore

            docRef.SetAsync(cardInfo).ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"Datos de {cardName} almacenados correctamente");
                }
                else
                {
                    Debug.LogError("Error guardando datos" + task.ToString());
                }
            });
        }
        else 
        {
            Debug.LogError("Carta no encontrada");
        }
    }

    public void TxtAR()
    {
        SceneManager.LoadScene("AR");
    }

    public void FetchCardDataFromFirestore(string cardName,TMP_Text textMesh)
    {
        FetchCardData(cardName,textMesh);
    }

    private async void FetchCardData(string cardName, TMP_Text textMesh)
    {
        textMesh.text = "Buscando datos...";

        List<string> collections = new List<string> {"Magia","Monstruo","Trampa","Otros"};
        List<Task<DocumentSnapshot>> tasks = new List<Task<DocumentSnapshot>>();

        //Iniciar todas las consultas
        foreach (string collection in collections) 
        {
            DocumentReference docRef = firestore.Collection("cartas").Document(collection).Collection(cardName).Document("Datos");
            tasks.Add(docRef.GetSnapshotAsync());
        }

        try
        {
            //Esperar a que todas las consultas terminen
            DocumentSnapshot[] snapshots = await Task.WhenAll(tasks);

            //Buscar la primera que tenga datos
            foreach (var snapshot in snapshots) 
            {
                if (snapshot.Exists)
                {
                    Dictionary<string, object> cardData = snapshot.ToDictionary();
                    textMesh.text = $"Nombre: {cardName}\n " + $"Ataque: {cardData["ATK"]}\n" +
                        $"Defensa: {cardData["DEF"]}\n " + $"Descripción: \n {cardData["Desc"]}\n" +
                        $"Nivel: {cardData["Level"]}\n";
                    return;
                }
            }

            //Si niguna tiene datos

            Debug.Log("Carta no encontrada");
            textMesh.text = "Carta no encontrada";
        }
        catch (Exception ex)
        {
            Debug.LogError("Error al buscar la carta: "+ex.Message);
            textMesh.text = "Error al buscar la carta: " + ex.Message;
        }
    }

    private string DetermineCollection(string cardType)
    {
        if (cardType.Contains("Monster"))
            return "Monstruo";
        if (cardType.Contains("Spell"))
            return "Magia";
        if (cardType.Contains("Trap"))
            return "Trampa";
        else
            return "Otros";
    }
    void Start()
    {
        
    }

        void Update()
    {
        
    }
}

//Clase para mapear la respuesta JSON
[System.Serializable]

public class CardDataResponse
{
    public CardData[] data;

}

[System.Serializable]
public class CardData
{
    public string name;
    public string type;
    public string desc;
    public int atk;
    public int def;
    public int level;

}
