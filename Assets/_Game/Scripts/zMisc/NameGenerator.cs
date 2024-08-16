using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class NameGenerator
{
    public static void GenerateFirstName()
    {
        string filePath = Path.Combine(Application.dataPath, "_Game", "Imports", "first-names.txt");

        List<string> names = new();

        if (File.Exists(filePath))
        {
            string[] lines = File.ReadAllLines(filePath);
            names.AddRange(lines.Select(line => line.Trim()));

            if (names.Count > 0)
            {
                int randomIndex = Random.Range(0, names.Count);
                string randomName = names[randomIndex];

                Debug.Log("Randomly selected name: " + randomName);
            }
            else
            {
                Debug.LogWarning("No names found in the file.");
            }
        }
        else
        {
            Debug.LogError("File not found: " + filePath);
        }
    }
}