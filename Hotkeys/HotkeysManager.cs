using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Data;
using System.Windows.Input;
using System.Windows.Forms;


namespace EpilepsyEmergencyContainer.Hotkeys
{
    /// <summary>
    /// https://www.youtube.com/watch?v=qLxqoh1JLnM
    /// C# - System-wide Global Hotkeys (using keyboard hooks) (and WPF)
    /// Edited version of ny4rlk0
    /// </summary>
    public static class HotkeysManager
    {

        public delegate void HotkeyEvent(GlobalHotkey hotkey);
        public static event HotkeyEvent HotkeyFired;
        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private static LowLevelKeyboardProc LowLevelProc = HookCallback;
        private static List<GlobalHotkey> Hotkeys { get; set; }
        private const int WH_KEYBOARD_LL = 13;
        private static IntPtr HookID = IntPtr.Zero;
        public static bool IsHookSetup{ get; private set; }
        static HotkeysManager(){Hotkeys=new List<GlobalHotkey>();}
        public static void SetupSystemHook(){
             if (!IsHookSetup) { 
                HookID = SetHook(LowLevelProc);
                IsHookSetup=true;            
             }
        }
        public static void ShutdownSystemHook() {
            if (IsHookSetup)
            {
                UnhookWindowsHookEx(HookID);
                IsHookSetup=false;
            }
        }
        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process currentProccess= Process.GetCurrentProcess())
            {
                using (ProcessModule currentModue = currentProccess.MainModule)
                {
                    return SetWindowsHookEx(WH_KEYBOARD_LL, proc, GetModuleHandle(currentModue.ModuleName), 0);
                }
            }
        }
        public static void AddHotkey(GlobalHotkey hotkey)
        {Hotkeys.Add(hotkey);}
        public static void RemoveHotkey(GlobalHotkey hotkey)
        {Hotkeys.Remove(hotkey);}
        public static List<GlobalHotkey> FindHotkeys(Key key)
        {
            List<GlobalHotkey> hotkeys = new List<GlobalHotkey>();
            foreach (GlobalHotkey hotkey in Hotkeys)
                if (hotkey.Key == key)
                    hotkeys.Add(hotkey);

            return hotkeys;
        }
        public static void AddHotkey(Key key, Action callbackMethod, bool canExecute=true)
        {AddHotkey(new GlobalHotkey(key, callbackMethod, canExecute));}
        public static void RemoveHotkey(Key key)
        {
            List<GlobalHotkey> originalHotkeys = Hotkeys;
            List<GlobalHotkey> toBeRemoved = FindHotkeys(key);
            foreach (GlobalHotkey hotkey in toBeRemoved)
                {originalHotkeys.Remove(hotkey);}
        }

        private static IntPtr HookCallback(int nCode, IntPtr wParam,IntPtr lParam)
        {
            if (nCode>=0)
            {
                foreach(GlobalHotkey hotkey in Hotkeys)
                {
                    if ( Keyboard.IsKeyDown(hotkey.Key))//Keyboard.Modifiers == hotkey.Modifier &&
                    {
                        if (hotkey.CanExecute)
                        {
                            hotkey.Callback?.Invoke();
                            HotkeyFired?.Invoke(hotkey);
                        }
                    }
                }
            }
            return CallNextHookEx(HookID, nCode, wParam, lParam);
        }
        //Importing the dll
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    }
}
