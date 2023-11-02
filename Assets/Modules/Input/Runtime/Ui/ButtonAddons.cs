
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Taijj.Input
{
    public static class ButtonAddons
    {
        #region Listeners
        /// <summary>
        /// Adds a listener for the Select event to the Button.
        /// Will automatically add an EventTrigger component, if none is attached, yet.
        /// </summary>
        public static void AddSelectListener(this Button @this, UnityAction callback) => @this.AddListener(EventTriggerType.Select, callback);

        /// <summary>
        /// Adds a listener for the Submit event to the Button.
        /// Will automatically add an EventTrigger component, if none is attached, yet.
        /// </summary>
        public static void AddSubmitListener(this Button @this, UnityAction callback) => @this.AddListener(EventTriggerType.Submit, callback);

        private static void AddListener(this Button @this, EventTriggerType type, UnityAction callback)
        {
            void WrapCallback (BaseEventData _) => callback();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = type;
            entry.callback.AddListener(WrapCallback);
            GetTriggerFrom(@this).triggers.Add(entry);
        }

        private static EventTrigger GetTriggerFrom(Button button)
        {
            if (false == button.TryGetComponent(out EventTrigger trigger))
                trigger = button.gameObject.AddComponent<EventTrigger>();
            return trigger;
        }



        /// <summary>
        /// Removes all listeners from the Button and also the EventTrigger component.
        /// </summary>
        public static void RemoveAllListeners(this Button @this)
        {
            @this.onClick.RemoveAllListeners();
            if (false == @this.TryGetComponent(out EventTrigger trigger))
                return;

            foreach (EventTrigger.Entry entry in trigger.triggers)
                entry.callback.RemoveAllListeners();
            UnityEngine.Object.Destroy(trigger);
        }
        #endregion



        #region Navigation
        /// <summary>
        /// Sets an explicit Navigation to the next/previous selectable sibling of this Button.
        /// Automatically cycles between the first and last buttons.
        /// </summary>
        public static void SetDefaultNavigation(this Button @this)
        {
            Selectable[] siblings = @this.transform.parent.GetComponentsInChildren<Selectable>(true);
            int index = siblings.IndexOf(@this);

            Navigation nav = new Navigation();
            nav.mode = Navigation.Mode.Explicit;
                nav.selectOnUp = index == 0 ? siblings[siblings.Length - 1] : siblings[index - 1];
                nav.selectOnDown = index == siblings.Length - 1 ? siblings[0] : siblings[index + 1];
                @this.navigation = nav;
        }
        #endregion
    }
}