using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.UI;

namespace EEMod.Common.Managers
{
    public class UIManager : IUpdateableGameTime
    {
        private readonly Dictionary<string, UserInterface> _uiInterfaces = new Dictionary<string, UserInterface>();
        private readonly Dictionary<string, UIState> _uiStates = new Dictionary<string, UIState>();
        private readonly Dictionary<UserInterface, UIState> _binds = new Dictionary<UserInterface, UIState>();

        public void AddUIState(string uiStateName, UIState uiState)
        {
            if (_uiStates.ContainsKey(uiStateName))
                throw new InvalidOperationException($"State name already used: {uiStateName}");

            _uiStates.Add(uiStateName, uiState);
        }

        public void AddInterface(string uiInterfaceName, string bind = "")
        {
            if (_uiInterfaces.ContainsKey(uiInterfaceName))
                throw new InvalidOperationException($"Interface name already used: {uiInterfaceName}");

            _uiInterfaces.Add(uiInterfaceName, new UserInterface());

            if (bind != "")
                BindInterfaceToState(uiInterfaceName, bind);
        }

        public void RemoveInterface(string uiInterfaceName)
        {
            if (!_uiInterfaces.ContainsKey(uiInterfaceName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}");

            _uiInterfaces.Remove(uiInterfaceName);
        }

        public void SetInterfaceState(string uiInterfaceName, string uiStateName)
        {
            if (!_uiInterfaces.ContainsKey(uiInterfaceName) && !_uiStates.ContainsKey(uiStateName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}" +
                    $"\nState doesn't exist: {uiStateName}");
            else if (!_uiStates.ContainsKey(uiStateName))
                throw new InvalidOperationException($"State doesn't exist: {uiStateName}");
            else if (!_uiInterfaces.ContainsKey(uiInterfaceName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}");

            _uiInterfaces[uiInterfaceName].SetState(_uiStates[uiStateName]);
        }

        public void NullifyCurrentInterfaceState(string uiInterfaceName)
        {
            if (!_uiInterfaces.ContainsKey(uiInterfaceName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}");

            _uiInterfaces[uiInterfaceName].SetState(null);
        }

        public void BindInterfaceToState(string uiInterfaceName, string uiStateName)
        {
            if (!_uiInterfaces.ContainsKey(uiInterfaceName) && !_uiStates.ContainsKey(uiStateName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}" +
                    $"\nState doesn't exist: {uiStateName}");
            else if (!_uiStates.ContainsKey(uiStateName))
                throw new InvalidOperationException($"State doesn't exist: {uiStateName}");
            else if (!_uiInterfaces.ContainsKey(uiInterfaceName))
                throw new InvalidOperationException($"Interface doesn't exist: {uiInterfaceName}");

            _binds.Add(_uiInterfaces[uiInterfaceName], _uiStates[uiStateName]);
        }

        public void SetToBindedState(string uiInterfaceName)
        {
            if (IsBinded(uiInterfaceName))
                _uiInterfaces[uiInterfaceName].SetState(_binds[_uiInterfaces[uiInterfaceName]]);
        }

        public bool IsActive(string uiInterfaceName) => _uiInterfaces[uiInterfaceName].CurrentState != null;

        public bool IsBinded(string uiInterfaceName) => _uiInterfaces.ContainsKey(uiInterfaceName) && _binds.ContainsKey(_uiInterfaces[uiInterfaceName]);

        public void SwitchBindedState(string UIInterfaceName)
        {
            if (IsBinded(UIInterfaceName))
                if (IsActive(UIInterfaceName))
                    NullifyCurrentInterfaceState(UIInterfaceName);
                else
                    SetToBindedState(UIInterfaceName);
        }

        public void Load()
        {
            for (int i = 0; i < _uiStates.Count; i++)
                _uiStates.Values.ToArray()[i].OnActivate();
        }

        public void Unload()
        {
            _uiInterfaces.Clear();
            _uiStates.Clear();
            _binds.Clear();
        }

        public void Update(GameTime gameTime)
        {
            foreach (UserInterface item in _uiInterfaces.Values)
                if (item.CurrentState != null)
                    item.Update(gameTime);
        }

        public void Draw(GameTime gameTime)
        {
            foreach (UserInterface item in _uiInterfaces.Values)
                if (item.CurrentState != null)
                    item.Draw(Main.spriteBatch, gameTime);
        }
    }
}