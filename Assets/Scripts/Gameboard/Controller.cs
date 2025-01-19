using System;
using UnityEditor.SearchService;
using UnityEngine;




namespace torethelloController
{
    public interface Scenario
    {
        public void OnActivate();

        public void Update();

        public void OnDeactivate();
    }


    //Base class for handling multiple scenes (In menu, In game....)
    public class Controller
    {
        private Scenario activeScenario;
        private Settings settings;

        public void Init(GameManager parent)
        {
            Cursor.lockState = CursorLockMode.Confined;
            //Cursor.SetCursor(cursorTex, new(), CursorMode.ForceSoftware);
            Cursor.visible = true;

        }

        public void SetScenario(Scenario scenario)
        {
            activeScenario.OnDeactivate();
            activeScenario = scenario;
            activeScenario.OnActivate();
        }


        public void Update()
        {
            activeScenario.Update();
        }

    }




    [Serializable]
    public class Settings
    {
        private Vector2 camAngles;
        private Vector3 camPos;
    }
}