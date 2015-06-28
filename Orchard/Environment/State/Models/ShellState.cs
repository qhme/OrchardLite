using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.Environment.State.Models
{
    public class ShellState
    {
        public ShellState()
        {
            Features = new List<ShellFeatureState>();
        }

        public IEnumerable<ShellFeatureState> Features { get; set; }
    }

    public class ShellFeatureState
    {
        public string Name { get; set; }
        public State InstallState { get; set; }
        public State EnableState { get; set; }

        public bool IsInstalled { get { return InstallState == State.Up; } }
        public bool IsEnabled { get { return EnableState == State.Up; } }
        public bool IsDisabled { get { return EnableState == State.Down || EnableState == State.Undefined; } }

        public enum State
        {
            Undefined,
            Rising,
            Up,
            Falling,
            Down,
        }
    }

}
