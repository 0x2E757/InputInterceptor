using System;
using System.Collections.Generic;
using System.Threading;

using Filter = System.UInt16;

namespace InputInterceptorNS {

    public class KeyboardHook : Hook<KeyStroke> {

        private struct KeyData {

            public KeyCode Code;
            public Boolean Shift;

        }

        private static readonly Dictionary<Char, KeyData> KeyDictionary;
        private static readonly KeyData QuestionMark;

        static KeyboardHook() {
            KeyDictionary = new Dictionary<Char, KeyData>();
            KeyDictionary.Add('a', new KeyData { Code = KeyCode.A, Shift = false });
            KeyDictionary.Add('b', new KeyData { Code = KeyCode.B, Shift = false });
            KeyDictionary.Add('c', new KeyData { Code = KeyCode.C, Shift = false });
            KeyDictionary.Add('d', new KeyData { Code = KeyCode.D, Shift = false });
            KeyDictionary.Add('e', new KeyData { Code = KeyCode.E, Shift = false });
            KeyDictionary.Add('f', new KeyData { Code = KeyCode.F, Shift = false });
            KeyDictionary.Add('g', new KeyData { Code = KeyCode.G, Shift = false });
            KeyDictionary.Add('h', new KeyData { Code = KeyCode.H, Shift = false });
            KeyDictionary.Add('i', new KeyData { Code = KeyCode.I, Shift = false });
            KeyDictionary.Add('j', new KeyData { Code = KeyCode.J, Shift = false });
            KeyDictionary.Add('k', new KeyData { Code = KeyCode.K, Shift = false });
            KeyDictionary.Add('l', new KeyData { Code = KeyCode.L, Shift = false });
            KeyDictionary.Add('m', new KeyData { Code = KeyCode.M, Shift = false });
            KeyDictionary.Add('n', new KeyData { Code = KeyCode.N, Shift = false });
            KeyDictionary.Add('o', new KeyData { Code = KeyCode.O, Shift = false });
            KeyDictionary.Add('p', new KeyData { Code = KeyCode.P, Shift = false });
            KeyDictionary.Add('q', new KeyData { Code = KeyCode.Q, Shift = false });
            KeyDictionary.Add('r', new KeyData { Code = KeyCode.R, Shift = false });
            KeyDictionary.Add('s', new KeyData { Code = KeyCode.S, Shift = false });
            KeyDictionary.Add('t', new KeyData { Code = KeyCode.T, Shift = false });
            KeyDictionary.Add('u', new KeyData { Code = KeyCode.U, Shift = false });
            KeyDictionary.Add('v', new KeyData { Code = KeyCode.V, Shift = false });
            KeyDictionary.Add('w', new KeyData { Code = KeyCode.W, Shift = false });
            KeyDictionary.Add('x', new KeyData { Code = KeyCode.X, Shift = false });
            KeyDictionary.Add('y', new KeyData { Code = KeyCode.Y, Shift = false });
            KeyDictionary.Add('z', new KeyData { Code = KeyCode.Z, Shift = false });
            KeyDictionary.Add('A', new KeyData { Code = KeyCode.A, Shift = true });
            KeyDictionary.Add('B', new KeyData { Code = KeyCode.B, Shift = true });
            KeyDictionary.Add('C', new KeyData { Code = KeyCode.C, Shift = true });
            KeyDictionary.Add('D', new KeyData { Code = KeyCode.D, Shift = true });
            KeyDictionary.Add('E', new KeyData { Code = KeyCode.E, Shift = true });
            KeyDictionary.Add('F', new KeyData { Code = KeyCode.F, Shift = true });
            KeyDictionary.Add('G', new KeyData { Code = KeyCode.G, Shift = true });
            KeyDictionary.Add('H', new KeyData { Code = KeyCode.H, Shift = true });
            KeyDictionary.Add('I', new KeyData { Code = KeyCode.I, Shift = true });
            KeyDictionary.Add('J', new KeyData { Code = KeyCode.J, Shift = true });
            KeyDictionary.Add('K', new KeyData { Code = KeyCode.K, Shift = true });
            KeyDictionary.Add('L', new KeyData { Code = KeyCode.L, Shift = true });
            KeyDictionary.Add('M', new KeyData { Code = KeyCode.M, Shift = true });
            KeyDictionary.Add('N', new KeyData { Code = KeyCode.N, Shift = true });
            KeyDictionary.Add('O', new KeyData { Code = KeyCode.O, Shift = true });
            KeyDictionary.Add('P', new KeyData { Code = KeyCode.P, Shift = true });
            KeyDictionary.Add('Q', new KeyData { Code = KeyCode.Q, Shift = true });
            KeyDictionary.Add('R', new KeyData { Code = KeyCode.R, Shift = true });
            KeyDictionary.Add('S', new KeyData { Code = KeyCode.S, Shift = true });
            KeyDictionary.Add('T', new KeyData { Code = KeyCode.T, Shift = true });
            KeyDictionary.Add('U', new KeyData { Code = KeyCode.U, Shift = true });
            KeyDictionary.Add('V', new KeyData { Code = KeyCode.V, Shift = true });
            KeyDictionary.Add('W', new KeyData { Code = KeyCode.W, Shift = true });
            KeyDictionary.Add('X', new KeyData { Code = KeyCode.X, Shift = true });
            KeyDictionary.Add('Y', new KeyData { Code = KeyCode.Y, Shift = true });
            KeyDictionary.Add('Z', new KeyData { Code = KeyCode.Z, Shift = true });
            KeyDictionary.Add('1', new KeyData { Code = KeyCode.One, Shift = false });
            KeyDictionary.Add('2', new KeyData { Code = KeyCode.Two, Shift = false });
            KeyDictionary.Add('3', new KeyData { Code = KeyCode.Three, Shift = false });
            KeyDictionary.Add('4', new KeyData { Code = KeyCode.Four, Shift = false });
            KeyDictionary.Add('5', new KeyData { Code = KeyCode.Five, Shift = false });
            KeyDictionary.Add('6', new KeyData { Code = KeyCode.Six, Shift = false });
            KeyDictionary.Add('7', new KeyData { Code = KeyCode.Seven, Shift = false });
            KeyDictionary.Add('8', new KeyData { Code = KeyCode.Eight, Shift = false });
            KeyDictionary.Add('9', new KeyData { Code = KeyCode.Nine, Shift = false });
            KeyDictionary.Add('0', new KeyData { Code = KeyCode.Zero, Shift = false });
            KeyDictionary.Add('`', new KeyData { Code = KeyCode.Tilde, Shift = false });
            KeyDictionary.Add('-', new KeyData { Code = KeyCode.Dash, Shift = false });
            KeyDictionary.Add('=', new KeyData { Code = KeyCode.Equals, Shift = false });
            KeyDictionary.Add('[', new KeyData { Code = KeyCode.OpenBracketBrace, Shift = false });
            KeyDictionary.Add(']', new KeyData { Code = KeyCode.CloseBracketBrace, Shift = false });
            KeyDictionary.Add(';', new KeyData { Code = KeyCode.Semicolon, Shift = false });
            KeyDictionary.Add('\'', new KeyData { Code = KeyCode.Apostrophe, Shift = false });
            KeyDictionary.Add('\\', new KeyData { Code = KeyCode.Backslash, Shift = false });
            KeyDictionary.Add(',', new KeyData { Code = KeyCode.Comma, Shift = false });
            KeyDictionary.Add('.', new KeyData { Code = KeyCode.Dot, Shift = false });
            KeyDictionary.Add('/', new KeyData { Code = KeyCode.Slash, Shift = false });
            KeyDictionary.Add('~', new KeyData { Code = KeyCode.Tilde, Shift = true });
            KeyDictionary.Add('!', new KeyData { Code = KeyCode.One, Shift = true });
            KeyDictionary.Add('@', new KeyData { Code = KeyCode.Two, Shift = true });
            KeyDictionary.Add('#', new KeyData { Code = KeyCode.Three, Shift = true });
            KeyDictionary.Add('$', new KeyData { Code = KeyCode.Four, Shift = true });
            KeyDictionary.Add('%', new KeyData { Code = KeyCode.Five, Shift = true });
            KeyDictionary.Add('^', new KeyData { Code = KeyCode.Six, Shift = true });
            KeyDictionary.Add('&', new KeyData { Code = KeyCode.Seven, Shift = true });
            KeyDictionary.Add('*', new KeyData { Code = KeyCode.Eight, Shift = true });
            KeyDictionary.Add('(', new KeyData { Code = KeyCode.Nine, Shift = true });
            KeyDictionary.Add(')', new KeyData { Code = KeyCode.Zero, Shift = true });
            KeyDictionary.Add('_', new KeyData { Code = KeyCode.Dash, Shift = true });
            KeyDictionary.Add('+', new KeyData { Code = KeyCode.Equals, Shift = true });
            KeyDictionary.Add('{', new KeyData { Code = KeyCode.OpenBracketBrace, Shift = true });
            KeyDictionary.Add('}', new KeyData { Code = KeyCode.CloseBracketBrace, Shift = true });
            KeyDictionary.Add(':', new KeyData { Code = KeyCode.Semicolon, Shift = true });
            KeyDictionary.Add('"', new KeyData { Code = KeyCode.Apostrophe, Shift = true });
            KeyDictionary.Add('|', new KeyData { Code = KeyCode.Backslash, Shift = true });
            KeyDictionary.Add('<', new KeyData { Code = KeyCode.Comma, Shift = true });
            KeyDictionary.Add('>', new KeyData { Code = KeyCode.Dot, Shift = true });
            KeyDictionary.Add('?', new KeyData { Code = KeyCode.Slash, Shift = true });
            KeyDictionary.Add(' ', new KeyData { Code = KeyCode.Space, Shift = false });
            QuestionMark = new KeyData { Code = KeyCode.Slash, Shift = true };
        }

        public KeyboardHook(KeyboardFilter filter = KeyboardFilter.All, CallbackAction callback = null) :
            base((Filter)filter, InputInterceptor.IsKeyboard, callback) { }

        public KeyboardHook(CallbackAction callback) :
            base((Filter)KeyboardFilter.All, InputInterceptor.IsKeyboard, callback) { }

        protected override void CallbackWrapper(ref Stroke stroke) {
            this.Callback(ref stroke.Key);
        }

        public Boolean SetKeyState(KeyCode code, KeyState state) {
            if (this.CanSimulateInput) {
                Stroke stroke = new Stroke();
                stroke.Key.Code = code;
                stroke.Key.State = state;
                return InputInterceptor.Send(this.Context, this.AnyDevice, ref stroke, 1) == 1;
            }
            return false;
        }

        private Boolean SimulateKeyPress(KeyCode code, KeyState firstState, KeyState secondState, Int32 releaseDelay) {
            if (this.SetKeyState(code, firstState)) {
                Thread.Sleep(releaseDelay);
                return this.SetKeyState(code, secondState);
            }
            return false;
        }

        public Boolean SimulateKeyPress(KeyCode code, Int32 releaseDelay = 75) {
            return this.SimulateKeyPress(code, KeyState.Down, KeyState.Up, releaseDelay);
        }

        public Boolean SimulateInput(String text, Int32 delayBetweenKeyPresses = 100, Int32 releaseDelay = 75) {
            Boolean shiftDown = false;
            foreach (Char letter in text) {
                KeyData keyData;
                if (KeyDictionary.TryGetValue(letter, out keyData) == false) keyData = QuestionMark;
                if (keyData.Shift != shiftDown) {
                    if (keyData.Shift) {
                        if (this.SetKeyState(KeyCode.LeftShift, KeyState.Down) == false) return false;
                    } else {
                        if (this.SetKeyState(KeyCode.LeftShift, KeyState.Up) == false) return false;
                    }
                    Thread.Sleep(delayBetweenKeyPresses / 2);
                    shiftDown = keyData.Shift;
                }
                if (this.SimulateKeyPress(keyData.Code, releaseDelay) == false) return false;
                Thread.Sleep(delayBetweenKeyPresses);
            }
            if (shiftDown) {
                if (this.SetKeyState(KeyCode.LeftShift, KeyState.Up) == false) return false;
            }
            return true;
        }

    }

}
