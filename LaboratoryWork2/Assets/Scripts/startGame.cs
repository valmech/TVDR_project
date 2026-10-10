using UnityEngine;
using UnityEngine.SceneManagement;

public class startGame : MonoBehaviour
{
    public void OnTap()
    {
        SceneManager.LoadScene("SampleScene");
    }


}
