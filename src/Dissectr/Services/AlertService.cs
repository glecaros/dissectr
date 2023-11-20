using DocumentFormat.OpenXml.Math;
using Microsoft.Maui.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dissectr.Services;

public interface IAlertService
{
    Task ShowAlertAsync(string title, string message);
    Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel);
}

internal class AlertService : IAlertService
{
    public Task ShowAlertAsync(string title, string message)
    {
        if (Application.Current?.MainPage is Page mainPage)
        {
            return mainPage.DisplayAlert(title, message, "Ok");
        }
        throw new ApplicationException($"Unexpected error: {nameof(Application.Current.MainPage)} is null");
    }

    public Task<bool> ShowConfirmationAsync(string title, string message, string accept, string cancel)
    {
        if (Application.Current?.MainPage is Page mainPage)
        {
            return mainPage.DisplayAlert(title, message, accept, cancel);
        }
        throw new ApplicationException($"Unexpected error: {nameof(Application.Current.MainPage)} is null");
    }
}
