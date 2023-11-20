using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissectr.Services;

internal interface IAlertService
{
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
}

internal class AlertService : IAlertService
{
    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage is Page mainPage)
        {
            return mainPage.DisplayAlert(title, message, accept, cancel);
        }
        throw new ApplicationException($"Unexpected error: {nameof(Application.Current.MainPage)} is null");
    }
}
