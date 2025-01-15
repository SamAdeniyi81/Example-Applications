using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuckZoneController : MonoBehaviour
{
    public GameObject gameOverScreen; // Reference to a Game Over UI or logic

    private List<GameObject> ducksInZone = new List<GameObject>(); // List to track ducks in the area

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering is tagged as a Duck
        if (other.CompareTag("Duck"))
        {
            ducksInZone.Add(other.gameObject);

            // If more than 3 ducks are in the area, remove them from the scene
            if (ducksInZone.Count > 3)
            {
                RemoveDucks(); //never reaches here TODO
            }

            // If more than 5 ducks are in the area, trigger Game Over
            if (ducksInZone.Count > 5)
            {
                GameOver();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the duck from the list when it leaves the area
        if (other.CompareTag("Duck"))
        {
            ducksInZone.Remove(other.gameObject);
        }
    }

    private void RemoveDucks()
    {
        // Remove each duck in the zone from the scene
        foreach (GameObject duck in ducksInZone)
        {
            Destroy(duck); // Destroy the duck game object
        }

        // Clear the list after all ducks have been removed
        ducksInZone.Clear();
    }

    private void GameOver()
    {
        // Stop any other game logic, show game over screen or UI
        Debug.Log("Game Over! Too many ducks have entered the area.");

        // Show the Game Over screen or logic
        if (gameOverScreen != null)
        {
            gameOverScreen.SetActive(true); // TODO - Show the game over UI 
        }

        Time.timeScale = 0; // Optional: freeze the game
    }
}
