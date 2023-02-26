using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Documents;

namespace EpilepsyEmergencyContainer.Hotkeys
{
    public class GlobalHotkey
    {
        /// <summary>
        /// https://www.youtube.com/watch?v=qLxqoh1JLnM
        /// C# - System-wide Global Hotkeys (using keyboard hooks) (and WPF)
        /// </summary>
        //public ModifierKeys Modifier { get; set; }
        public Key Key { get; set; }
        public Action Callback { get; set; }
        public bool CanExecute { get; set; }
        // public GlobalHotkey(ModifierKeys  modifier, Key key,Action callback, bool canExecute=true) {
        public GlobalHotkey(Key key,Action callbackMethod, bool canExecute=true) {
            this.Key = key;
            this.Callback = callbackMethod;
            this.CanExecute = canExecute;
        }   
    }
}
