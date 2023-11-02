using UnityEngine;
using System;

namespace Taijj.Input
{
    /// <summary>
    /// Helper to control Gamepad Rumble in a unified matter.
    /// </summary>
    public class Rumble
    {
        #region Common
        [Serializable]
        public class Data
        {
            public Intensity intensity;
            public float duration = 0.2f;
            public AnimationCurve fadeCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);
        }

        public enum Intensity
        {
            None = 0,

            Weak,
            Medium,
            Strong
        }



        public Rumble(Model model) => Model = model;
        public void SetUp(Func<Peripheral> peripheralGetter) => GetPeripheral = peripheralGetter;
        public void CleanUp() => ExecuteStopped();

        private Model Model { get; set; }
        private Func<Peripheral> GetPeripheral { get; set; }
        #endregion



        #region Flow
        //States are used here because PS5 had caused trouble when the
        //Pads Motors were set in quick succession.
        private enum State
        {
            None = 0,

            Started,
            Rumbling,
            Stopped
        }

        public void Start(Data data)
        {
            if (false == Model.IsRumbleEnabled)
                return;

            if (false == GetPeripheral().HasGamepad)
                return;

            if (data.duration <= 0f)
                return;

            CurrentData = data;
            CurrentState = State.Started;
        }

        public void Stop()
        {
            if (CurrentState != State.None)
                CurrentState = State.Stopped;
        }

        public void Update()
        {
            if (CurrentState == State.None)
                return;

            switch (CurrentState)
            {
                case State.Started:   ExecuteStarted(); return;
                case State.Rumbling:  ExecuteRumbling(); return;
                case State.Stopped:   ExecuteStopped(); return;
            }
        }



        private void ExecuteStarted()
        {
            CompletedTime = Time.time + CurrentData.duration;
            CurrentState = State.Rumbling;
        }

        private void ExecuteRumbling()
        {
            float time = Time.time;
            if (time > CompletedTime)
            {
                Stop();
                return;
            }

            float intens = CalculateIntensity(time);
            GetPeripheral().SetMotors(intens);
        }

        private void ExecuteStopped()
        {
            if (false == GetPeripheral().HasGamepad)
                return;

            CompletedTime = 0f;
            GetPeripheral().SetMotors(0f);
            CurrentState = State.None;
        }



        private float CalculateIntensity(float time)
        {
            float result = 0f;
            switch (CurrentData.intensity)
            {
                case Intensity.Weak:   result = Model.WeakIntensity; break;
                case Intensity.Medium: result = Model.MediumIntensity; break;
                case Intensity.Strong: result = Model.StrongIntensity; break;
                case Intensity.None: Note.LogWarning("Rumble Intensity is set to None!"); return 0f;
            }

            float t = (CompletedTime - time) / CurrentData.duration;
            result *= CurrentData.fadeCurve.Evaluate(1f - t);
            return result;
        }



        private State CurrentState { get; set; }
        private Data CurrentData { get; set; }
        private float CompletedTime { get; set; }

        public bool IsRumbling => CurrentState == State.Rumbling || CurrentState == State.Started;
        #endregion
    }
}