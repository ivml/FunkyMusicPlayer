using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Notifications;

namespace MusicPlayerProject.Commands
{
    public class ToastNotificator
    {
        public static void ShowNotificationSongChange(string trackName, string author)
        {
            var toastXmlString = "<toast>"
                               + "<visual version='1'>"
                               + "<binding template='ToastText04'>"
                               + "<text id='1'>Now playing</text>"
                               + "<text id='2'>" + trackName + "</text>"
                               + "<text id='3'>" + author + "</text>"
                               + "</binding>"
                               + "</visual>"
                               + "<audio silent='true'/>"
                               + "</toast>";
            Windows.Data.Xml.Dom.XmlDocument toastDOM = new Windows.Data.Xml.Dom.XmlDocument();
            toastDOM.LoadXml(toastXmlString);

            ToastNotification toast = new ToastNotification(toastDOM);
            ToastNotificationManager.CreateToastNotifier().Show(toast);
        }
    }
}
