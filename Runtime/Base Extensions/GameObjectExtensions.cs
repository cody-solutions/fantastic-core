using UnityEngine;

namespace FantasticCore.Runtime.Base_Extensions
{
    public static class GameObjectExtensions
    {
        public static void ToggleActive(this GameObject gameObject, bool state)
        {
            if (state)
            {
                gameObject.Activate();
                return;
            }

            gameObject.Deactivate();
        }

        public static void Activate(this GameObject gameObject) => gameObject.SetActive(true);

        public static void Deactivate(this GameObject gameObject) => gameObject.SetActive(false);
    }
}