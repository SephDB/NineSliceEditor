using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NineSliceEditor.Helpers
{
    public class RelayCommand : ICommand
    {
        public Action<object?> ExecuteImpl { get; set; }
        public Func<object?, bool>? CanExecuteImpl { get; set; }

        public RelayCommand(Action<object?> execute)
        {
            ExecuteImpl = execute;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            return CanExecuteImpl?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            ExecuteImpl(parameter);
        }
    }
}
