using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TraceSystem.Models
{
    public interface IDispatch
    {
        bool CheckAccess();
        void Invoke(Action action);
    }
}
