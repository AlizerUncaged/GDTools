using log4net.Appender;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities {
    public class Appender_Pauser {

        private Dictionary<IAppender, log4net.Core.Level> originalThresholds = new();

        public void PauseInterfaceOfType<T>() where T : IAppender {
            var appenders = Logging.LogRespository.GetAppenders().Where(x => x is T);
            foreach (var appender in appenders) {
                if (appender is AppenderSkeleton appenderSkeleton) {
                    originalThresholds.Add(appender, appenderSkeleton.Threshold);
                    appenderSkeleton.Threshold = log4net.Core.Level.Off;
                }
            }
        }

        public void ResumeInterfaceOfType<T>() where T : IAppender {
            var appenders = Logging.LogRespository.GetAppenders().Where(x => x is T);
            foreach (var appender in appenders) {
                if (appender is AppenderSkeleton appenderSkeleton) {
                    appenderSkeleton.Threshold = originalThresholds[appender];
                }
            }
        }


    }
}
