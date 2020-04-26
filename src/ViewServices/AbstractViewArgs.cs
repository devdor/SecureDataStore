using Serilog.Core;
using System;

namespace SecureDataStore.ViewServices {
    class AbstractViewArgs {
        #region Fields and Properties
        public string ViewHeader {
            get;
            set;
        }

        public Logger Log {
            get;
            set;
        }
        #endregion

        public AbstractViewArgs(Logger logger, string viewHeader) {

            if (logger == null)
                throw new ArgumentNullException("Logger");

            if (String.IsNullOrWhiteSpace(viewHeader))
                throw new ArgumentNullException("ViewHeader");

            this.Log = logger;
            this.ViewHeader = viewHeader;
        }
    }
}
