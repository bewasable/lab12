using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lab12.Services
{
    public interface IDialogService
    {
        void ShowInfo(string message);

        void ShowWarning(string message);

        void ShowError(string message);

        bool ShowConfirmation(string message);
    }
}
