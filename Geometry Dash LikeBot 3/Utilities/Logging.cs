using log4net;
using log4net.Appender;
using log4net.Config;
using log4net.Repository;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Geometry_Dash_LikeBot_3.Utilities {
    public static class Logging {

        public static Appender_Pauser PauseConsoleAppenders {
            get {
                var pause = new Appender_Pauser();
                pause.PauseInterfaceOfType<ConsoleAppender>();
                return pause;
            }
        }

        public static ILoggerRepository LogRespository = LogManager.GetRepository(Assembly.GetEntryAssembly());

        public static void Initialize() {
            LogRespository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(LogRespository, new FileInfo("log4net.config"));
        }
    }
}
