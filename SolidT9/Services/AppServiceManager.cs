using Android.Content;
using Microsoft.Extensions.Logging;
using SolidT9.Platforms.Android;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ILogger = NLog.ILogger;

namespace SolidT9.Services
{
    public interface IAppServiceManager
    {
        void InitServices();
    }

    public class AppServiceManager : IAppServiceManager
    {
        private bool _init;
        private ILogger _logger;

        public AppServiceManager()
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public void InitServices()
        {
            if (_init) return;
            
#if ANDROID
            var actv = Platform.CurrentActivity;
            Intent intent;

            if (actv == null)
                return;

            intent = new Intent(actv, typeof(KeypadService));
            intent.PutExtra("inputExtra", "T9 Service");
            actv.StartService(intent);
#endif

            _logger.Debug("All services successfully initialized");
            _init = true;
        }
    }
}
