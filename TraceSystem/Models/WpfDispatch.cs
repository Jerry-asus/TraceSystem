using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TraceSystem.Models
{
    public class WpfDispatch : IDispatch
    {
        private readonly Dispatcher _dispatcher;

        public WpfDispatch(Dispatcher dispatcher) =>
       _dispatcher = dispatcher;
        public bool CheckAccess() => _dispatcher.CheckAccess();
        public void Invoke(Action action) => _dispatcher.Invoke(action);
    }
}
