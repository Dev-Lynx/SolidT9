using Android.App;
using Android.Content;
using Android.Graphics;
using Android.InputMethodServices;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Views.InputMethods;
using Android.Widget;
using Kotlin.Text;
using Microsoft.Maui.Platform;
using NLog;
using NLog.Fluent;
using SolidT9.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Droid = Android;
using DroidView = Android.Views.View;

namespace SolidT9.Platforms.Android
{
    [IntentFilter(new string[] { "android.view.InputMethod" })]
    [MetaData(name: "android.view.im", Resource = "@xml/ime_method")]
    [Service(Permission = "android.permission.BIND_INPUT_METHOD", Exported = true, Label = "SolidT9", Name = "io.github.devlynx.SolidT9IME")]
    internal class KeypadService : InputMethodService
    {
        private readonly ILogger _logger;
        private DroidView _view;

        public KeypadService()
        {
            _logger = LogManager.GetCurrentClassLogger();
            
        }

        public override void OnStartInput(EditorInfo attribute, bool restarting)
        {
            ShowT9UI();
            base.OnStartInput(attribute, restarting);
        }

        public override bool OnKeyDown([GeneratedEnum] global::Android.Views.Keycode keyCode, KeyEvent e)
        {
            _logger.Debug("Key {0}", e.KeyCode);

            return base.OnKeyDown(keyCode, e);
        }

        private void ShowT9UI()
        {
            if (_view == null)
            {
                _view = new HintView().ToPlatform(
                    new MauiContext(ServiceProvider.Services, BaseContext));
            }

            SetInputView(_view);
        }
    }
}
