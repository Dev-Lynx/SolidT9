using Android.App;
using Android.Content;
using Android.Graphics;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Service.Autofill;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Java.Lang;
using JetBrains.Annotations;
using Kotlin.Text;
using Microsoft.Maui.Controls.Xaml;
using Microsoft.Maui.Platform;
using NLog;
using NLog.Fluent;
using SolidT9.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DroidView = Android.Views.View;

namespace SolidT9.Platforms.Droid
{
    [IntentFilter(new string[] { "android.view.InputMethod" })]
    [MetaData(name: "android.view.im", Resource = "@xml/ime_method")]
    [Service(Permission = "android.permission.BIND_INPUT_METHOD", Exported = true, Label = "SolidT9", Name = "io.github.devlynx.SolidT9IME")]
    internal class KeypadService : InputMethodService
    {
        private static int CURSOR_MONITOR_NOW = (int)(CursorUpdate.Immediate | CursorUpdate.Monitor);
        private const int IDEAL_WORD_LENGTH = 46; // pneumonoultramicroscopicsilicovolcanoconiosis = 45

        private readonly ILogger _logger;
        private DroidView _view;

        private long _lastInputTime;
        private int _lastKey;
        private readonly System.Text.StringBuilder _textBuilder;
        private CursorAnchorInfo _cursorInfo;

        public KeypadService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            _textBuilder = new();
        }

        public override void OnStartInput(EditorInfo attribute, bool restarting)
        {
            ShowT9UI();
            base.OnStartInput(attribute, restarting);
        }

        public override bool OnKeyDown([GeneratedEnum] global::Android.Views.Keycode keyCode, KeyEvent e)
        {
            IInputConnection ic = CurrentInputConnection;
            if (ic == null) return base.OnKeyDown(keyCode, e);

            int key = (int)keyCode;

            long lastInputTime = Interlocked.Exchange(ref _lastInputTime, e.EventTime);
            int lastKey = Interlocked.Exchange(ref _lastKey, key);

            


            if (key == lastKey && e.RepeatCount == 0 && lastInputTime == e.EventTime)
            {
                // Ignore repititons
                _logger.Warn("[LAG] {0}", keyCode);
                return true;
            }


            if (keyCode == Android.Views.Keycode.DpadLeft || keyCode == Android.Views.Keycode.DpadDownRight)
            {
                MarkSurrounding();
            }

            //_logger.Debug("KeyDown: {0} ({1})\tLast: {2}\tT: {3} vs {4}", keyCode, key, lastKey, e.EventTime, lastInputTime);
            return base.OnKeyDown(keyCode, e);
        }

        public override bool OnKeyUp([GeneratedEnum] Android.Views.Keycode keyCode, KeyEvent e)
        {

            //_logger.Debug("KeyUp: {0} {1}", keyCode, e.RepeatCount);
            return base.OnKeyUp(keyCode, e);
        }

        public override void OnUpdateCursorAnchorInfo(CursorAnchorInfo info)
        {
            _logger.Debug("Cursor updated: {0} => {1}", info.SelectionStart, info.SelectionEnd);
            _cursorInfo = info;
            MarkSurrounding();

            base.OnUpdateCursorAnchorInfo(info);
        }

        private void MarkSurrounding()
        {
            bool isHighlight = _cursorInfo.SelectionStart != _cursorInfo.SelectionEnd;
            int start = 0, end = 0;

            IInputConnection ic = CurrentInputConnection;
            int sel = _cursorInfo.SelectionStart;

            if (!isHighlight)
            {
                string bf = ic.GetTextBeforeCursor(IDEAL_WORD_LENGTH, GetTextFlags.None) ?? string.Empty;
                string af = ic.GetTextAfterCursor(IDEAL_WORD_LENGTH, GetTextFlags.None) ?? string.Empty;

                _logger.Debug("{0}|{1}", bf, af);
                var h = HighlightWord(bf, af);

                start = sel - h.Item1;
                end = sel + h.Item2;
            }


            _logger.Debug("Range Found: {0} - {1}", start, end);


            if (end - start <= 0)
                ic.FinishComposingText();
            else ic.SetComposingRegion(start, end);
        }

        private (int, int) HighlightWord(string before, string after)
        {
            char c;
            int start = 0, end;


            for (int i = before.Length - 1; i >= 0; i--, start++)
            {
                c = before[i];

                if (char.IsWhiteSpace(c) || char.IsSurrogate(c))
                    break;
            }

            for (end = 0; end < after.Length; end++)
            {
                c = after[end];

                if (char.IsWhiteSpace(c) || char.IsSurrogate(c))
                    break;
            }

            return (start, end);
        }

        private void ShowT9UI()
        {
            if (_view == null)
            {
                _view = new HintView().ToPlatform(
                    new MauiContext(ServiceProvider.Services, BaseContext));

                SetInputView(_view);
            }

            
            _textBuilder.Clear();

            try
            {
                IInputConnection ic = CurrentInputConnection;
                ic.RequestCursorUpdates(CURSOR_MONITOR_NOW);


                var req = ic?.GetExtractedText(new ExtractedTextRequest(), GetTextFlags.None);

                if (req != null)
                {


                    foreach (char c in req.Text)
                    {
                        _textBuilder.Append(c);

                        
                    }

                    string szText = _textBuilder.ToString();
                    _logger.Debug("Got Text: '{0}'\n\nS: {1} E: {2}", szText, req.SelectionStart, req.SelectionEnd);

                    
                    
                    

                    

                    //byte[] buffer = Encoding.Unicode.GetBytes(szText);
                    //_logger.Debug(BitConverter.ToString(buffer));

                }
            }
            catch (System.Exception ex)
            {
                _logger.Error(ex);
            }
            
        }
    }
}
