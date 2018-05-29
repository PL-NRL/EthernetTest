using System;
using System.Collections;
using System.Threading;
using System.Text;
using Microsoft.SPOT;
using Microsoft.SPOT.Presentation;
using Microsoft.SPOT.Presentation.Controls;
using Microsoft.SPOT.Presentation.Media;
using Microsoft.SPOT.Presentation.Shapes;
using Microsoft.SPOT.Time;
using Microsoft.SPOT.Touch;
using Microsoft.SPOT.Net;
using System.Net;
using Gadgeteer.Networking;
using GT = Gadgeteer;
using GTM = Gadgeteer.Modules;
using Gadgeteer.Modules.GHIElectronics;

namespace EthernetTest
{
    public partial class Program
    {

        byte[] HTML = Encoding.UTF8.GetBytes(
                            "<html><body>" +
                                "<h1>Hosted on .NET Gadgeteer</h1>" +
                                "<p>Lets scare someone!</p>" +
                                "<form action=\"\" method=\"post\">" +
                                    "<input type=\"submit\" value=\"Toggle LED!\">" +
                                    "<input name=\"testo\" type=\"text\" value=\"Testo qui..\">"+
                                "</form>" +
                            "</body></html>");

        // This method is run when the mainboard is powered up or reset.   
        void ProgramStarted()
        {
            Debug.Print("Program Started!");
            ethernetJ11D.UseThisNetworkInterface();
            //ethernetJ11D.UseStaticIP("")
            new Thread(SetTime).Start();

            ethernetJ11D.NetworkUp += ethernetJ11D_NetworkUp;
            ethernetJ11D.NetworkDown += ethernetJ11D_NetworkDown;
            //new Thread(RunWebServer).Start();
            
         }
           
        void ethernetJ11D_NetworkDown(GTM.Module.NetworkModule sender,
            GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is down!");
        }
        void ethernetJ11D_NetworkUp(GTM.Module.NetworkModule sender,
                GTM.Module.NetworkModule.NetworkState state)
        {
            Debug.Print("Network is up!");
            Debug.Print("My IP is: " + ethernetJ11D.NetworkSettings.IPAddress);
        }
       

        void SetTime()
        {
            Debug.Print("setting time");

            TimeService.SystemTimeChanged += new SystemTimeChangedEventHandler(TimeService_SystemTimeChanged);
            TimeService.TimeSyncFailed += new TimeSyncFailedEventHandler(TimeService_TimeSyncFailed);

            var settings = new TimeServiceSettings();
            settings.ForceSyncAtWakeUp = true;

            // refresh time is in seconds   
            settings.RefreshTime = 1800;

            settings.PrimaryServer = GetTimeServerAddress();

            TimeService.Settings = settings;
            TimeService.SetTimeZoneOffset(0);
            TimeService.Start();
        }

        byte[] GetTimeServerAddress()
        {
            Debug.Print("SONO NEL GetTimeServerAddress");
            IPAddress[] address = Dns.GetHostEntry("time.windows.com").AddressList;
            Debug.Print("SONO NEL GetTimeServerAddress2");
            if (address != null && address.Length > 0)
            {
                return address[0].GetAddressBytes();
            }
            throw new ApplicationException("Could not get time server address");
        }

        void TimeService_TimeSyncFailed(object sender, TimeSyncFailedEventArgs e)
        {
            Debug.Print("error synchronizing time with NTP server: " + e.ErrorCode);
        }

        void TimeService_SystemTimeChanged(object sender, SystemTimeChangedEventArgs e)
        {
            Debug.Print("network time received. Current Date Time is " + DateTime.Now.ToLocalTime().ToString());
        }   
 


    }
}
