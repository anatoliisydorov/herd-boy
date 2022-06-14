using Dev.Core;

namespace Dev.Input
{
    public class BaseController : SmartUpdate, IController
    {
        protected ControllersConnector connector { get; private set; }

        public virtual void InitializeController(ControllersConnector connector)
        {
            this.connector = connector;
        }
    }
}