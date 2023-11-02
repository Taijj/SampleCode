using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

namespace Taijj.Input
{
    /// <summary>
    /// Helper for common input features when dealing with the Unity
    /// (Canvas) UI.
    ///
    /// Use this to navigate layer UIs with the need to remember what
    /// was selected before.
    /// </summary>
    public class Ui
    {
        #region LifeCycle
        public Ui(EventSystem eventSystem) => EventSystem = eventSystem;
        private EventSystem EventSystem { get; set; }
        #endregion



        #region Layering
        public void EnterLayer(GameObject defaultSelection)
        {
            LayerStack.Push(EventSystem.currentSelectedGameObject);
            Select(defaultSelection);
        }

        public void ExitUILayer(int layersToSkip)
        {
            int prePop = Mathf.Min(layersToSkip-1, LayerStack.Count-1);
            for (int i = 0; i <= prePop; i++)
                LayerStack.Pop();

            if (LayerStack.Count > 0)
                Select(LayerStack.Pop());
        }

        private Stack<GameObject> LayerStack { get; set; }
        #endregion



        #region Selections
        /// <summary>
        /// By default, Unity does not ignore disabled selectables. Use
        /// this in order to rectify that.
        /// </summary>
        public bool TrySelect(Selectable selectable, Vector2Int direction)
        {
            bool canSelect = selectable.gameObject.activeInHierarchy && selectable.interactable;
            if(canSelect)
            {
                Select(selectable.gameObject);
                return true;
            }

            Navigation nav = selectable.navigation;
            Selectable next = null;
            if(direction.x > 0) next = nav.selectOnRight;
            if(direction.x < 0) next = nav.selectOnLeft;
            if(direction.y > 0) next = nav.selectOnUp;
            if(direction.y < 0) next = nav.selectOnDown;

            if(next.HasReference())
            {
                Select(next.gameObject);
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsAnySelected(Selectable[] selectables)
        {
            foreach (Selectable selectable in selectables)
            {
                if (EventSystem.currentSelectedGameObject == selectable.gameObject)
                    return true;
            }
            return false;
        }

        private void Select(GameObject gameObject) { EventSystem.SetSelectedGameObject(gameObject); }
        #endregion
    }
}