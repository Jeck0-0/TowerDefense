using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense
{
    public class TutorialButton : MonoBehaviour
    {
        public GameObject button;
        public GameObject tutorial;

        public void Press()
        {
            button.SetActive(!button.activeSelf);
            tutorial.SetActive(!tutorial.activeSelf);
            
            Time.timeScale = tutorial.activeSelf ? 0 : 1;
        }
    }
}
