using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

namespace Taijj.Input
{
    /// <summary>
    /// Top level input API.
    /// </summary>
    public class UserInput : MonoBehaviour
    {
        #region LifeCycle
        private const int MAX_LAYER_DEPTH = 10;
        private const int NONE_INDEX = -1;

        [Space, Header("Main")]
        [SerializeField] private EventSystem _eventSystem;
        [SerializeField] private InputSystemUIInputModule _uiModule;

        [SerializeField, HideInInspector] private Model _model;
        [SerializeField, HideInInspector] private InputActionAsset _asset;
        [SerializeField, HideInInspector] private ActionMap[] _maps;

        public void Wake()
        {
            Model = _model;
            PeripheralSwitcher = new PeripheralSwitcher(_model.SupportedPeripherals);
            Rumble = new Rumble(_model);

            object actions = Model.InstantiateInputActions();
            for (int i = 0; i < _maps.Length; i++)
                _maps[i].Wake(actions);

            Stack = new Stack<ActionMap>(MAX_LAYER_DEPTH);
            UpdateMaps();
            WakeUi();
        }

        public void SetUp()
        {
            PeripheralSwitcher.SetUp(OnPeripheralChanged);
            Rumble.SetUp(GetCurrentPlatform);
            IsReady = true;
        }

        public void Update()
        {
            if (false == IsReady)
                return;

            Rumble.Update();
            for(int i = 0; i < _maps.Length; i++)
                _maps[i].Update();
        }

        public void CleanUp()
        {
            PeripheralSwitcher.CleanUp();
            Rumble.CleanUp();

            for (int i = 0; i < _maps.Length; i++)
                _maps[i].CleanUp();
        }



        private PeripheralSwitcher PeripheralSwitcher { get; set; }
        private Rumble Rumble { get; set; }

        private bool IsReady { get; set; }
        #endregion



        #region Static
        public static AnyAction CreateAnyAction() => new AnyAction(Model.AnyActionExclusions);
        public static Model Model { get; private set; }
        #endregion



        #region Map Changes
        /// <summary>
        /// Pushes the given map to the top of the ActionMap Stack.
        /// The topmost map is always the only one that is activated,
        /// i.e. the only one receiving and sending input events!
        /// </summary>
        /// <param name="clearStack">If true, the current Stack
        /// is cleared, before the given map is pushed.</param>
        public void PushMap(ActionMap map, bool clearStack = false)
        {
            if (clearStack)
                Stack.Clear();

            Stack.Push(map);
            UpdateMap();
        }

        /// <summary>
        /// Pops the topmost map from the ActionMap Stack.
        ///
        /// </summary>
        public void PopMap()
        {
            if (IsStackEmpty)
                return;

            Stack.Pop();
            UpdateMap();
        }



        private void UpdateMap()
        {
            Rumble.Stop();
            UpdateMaps();
        }

        private void UpdateMaps()
        {
            ActionMap current = IsStackEmpty ? null : Stack.Peek();
            for (int i = 0; i < _maps.Length; i++)
            {
                ActionMap mo = _maps[i];
                mo.IsEnabled = mo == current;
            }

            if (_model.LogOperations)
            {
                string name = current.IsNull() ? "None" : current.Name;
                Note.Log($"Input Mode changed to {name}!", ColorAddons.Aqua);
            }
        }

        private Stack<ActionMap> Stack { get; set; }
        private bool IsStackEmpty => Stack.Count == 0;
        #endregion



        #region Peripherals
        private void OnPeripheralChanged()
        {
            if (_model.LogOperations)
                Note.Log($"Peripheral changed to {PeripheralSwitcher.Current.name}", ColorAddons.Aqua);

            StopRumble();
            OnDeviceChanged?.Invoke();
        }

        private Peripheral GetCurrentPlatform() => PeripheralSwitcher.Current;
        public TMP_SpriteAsset CurrentSpriteAsset => GetCurrentPlatform().IconsSpriteAsset;
        public InputDevice CurrentDevice => GetCurrentPlatform().Device;

        public static event Action OnDeviceChanged;



        public void TryRumble(Rumble.Data data, bool ignoreIfAlreadyRumbling = false)
        {
            if (_model.LogOperations)
                Note.Log("TryRumble", ColorAddons.Aqua);

            if (Rumble.IsRumbling && ignoreIfAlreadyRumbling)
                return;

            Rumble.Start(data);
        }

        public void StopRumble() => Rumble.Stop();

        public bool IsCursorEnabled { set => Cursor.visible = value; }
        #endregion



        #region Ui
        private void WakeUi()
        {
            Ui = new Ui(_eventSystem);
            for(int i = 0; i < _maps.Length; i++)
            {
                if (_maps[i].IsUi)
                {
                    UiMap = _maps[i];
                    return;
                }
            }
            throw new Exception("No dedicated Ui InputMap is set! Please define one!");
        }



        public void EnterUiLayer(GameObject defaultSelection)
        {
            PushMap(UiMap);
            Ui.EnterLayer(defaultSelection);
        }

        public void ExitUiLayer(int layersToSkip = 0)
        {
            Ui.ExitUILayer(layersToSkip);
            PopMap();
        }



        public bool TrySelect(Selectable selectable, Vector2Int direction)
        {
            return Ui.TrySelect(selectable, direction);
        }

        public bool IsAnySelected(Selectable[] selectables) => Ui.IsAnySelected(selectables);



        private ActionMap UiMap { get; set; }
        private Ui Ui { get; set; }
        #endregion
    }
}