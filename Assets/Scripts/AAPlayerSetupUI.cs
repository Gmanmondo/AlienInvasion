using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using TMPro;

public class AAPlayerSetupUI : MonoBehaviour
{
    public TMP_Dropdown playerCountDropdown;
    public GameObject nameInputPrefab;
    public Transform nameInputParent;
    private List<TMP_InputField> nameFields = new List<TMP_InputField>();

    void Start()
    {
        // Always start with 1 input field
        AddPlayerInputField();
        playerCountDropdown.onValueChanged.AddListener(delegate { OnPlayerCountChanged(); });
    }

    public void OnPlayerCountChanged()
    {
        int desiredCount = playerCountDropdown.value + 1; // Because dropdown index 0 = 1 player

        // Add missing fields
        while (nameFields.Count < desiredCount)
        {
            AddPlayerInputField();
        }

        // Remove extras
        while (nameFields.Count > desiredCount)
        {
            TMP_InputField fieldToRemove = nameFields[nameFields.Count - 1];
            nameFields.RemoveAt(nameFields.Count - 1);
            Destroy(fieldToRemove.gameObject);
        }
    }

    private void AddPlayerInputField()
    {
        GameObject inputGO = Instantiate(nameInputPrefab, nameInputParent);
        TMP_InputField inputField = inputGO.GetComponent<TMP_InputField>();
    
        if (inputField != null)
        {
            int playerNumber = nameFields.Count + 1;

            var placeholder = inputField.placeholder as TMP_Text;
            if (placeholder != null)
            {
                placeholder.text = $"Player {playerNumber}";
            }

            nameFields.Add(inputField);
        }
        else
        {
            Debug.LogError("Name input prefab is missing TMP_InputField component.");
        }
    }


    public void StartGame()
    {
        List<string> names = new List<string>();

        for (int i = 0; i < nameFields.Count; i++)
        {
            string inputName = nameFields[i].text;
            if (string.IsNullOrWhiteSpace(inputName))
            {
                inputName = $"Player {i + 1}";
            }

            names.Add(inputName);
        }

        AAPlayerManager.Instance.SetPlayers(names);
        SceneManager.LoadScene("AlienGameplay");
    }
}