/*
 * NetPingMon
 * Unit: NetPingMon Menu structure (GUI) sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 19-06-2011
 * Updated: 05-02-2013
 * 
 * This file is part of NetPingMon (Network Ping Monitor).
 *
 * NetPingMon is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * NetPingMon is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with NetPingMon.  If not, see <http://www.gnu.org/licenses/>.
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;

using System.Diagnostics;
using System.Threading;

using NetPingMon.classes;

namespace NetPingMon.classes
{

    #region ENUMS
    /// <summary>
    /// Finit state machine (FSM, this defines our logic states)
    /// </summary>
    public enum menuState : byte
    {
        msScanning = 0,
        msAddPC,
        msRemovePC,
        msEnableDisablePC,
        msFlushDns,
        msHelp,
        msAlerts,
        msAddPCError
    }
    #endregion

    /// <summary>
    /// NetPingMon Menu structure
    /// </summary>
    public sealed class clsNetPingMenu
    {

        #region Constants
        /// <summary>
        /// Private constants
        /// </summary>
        private const int MAX_HOSTS_TO_SCAN  = 15;        //max hosts that can be entered to the worker list
        #endregion

        #region Private Vars
        /// <summary>
        /// Private vars
        /// </summary>
        private delegate void ProcessPing();              //delegate to trigger the scanning state
        private List<clsNetPingControllerBase> _hostList; //list with hostnames to ping
        private menuState _state = menuState.msScanning;   //Menu state, init to "scanning" state        
        private ProcessPing processPing;                  //Our "ping" processor
        private volatile bool _alertsEnabled = false;     //Audible Alerts tone if one or more hosts are offline
        private volatile bool _hostsOnline = false;       //keep track if all hosts are online        
        private volatile int _startTime, _endTime;        //timer values
        //private clsNetPingSpeakerController speaker = new clsNetPingSpeakerController();
        #endregion

        #region CTOR's
        /// <summary>
        /// Constructor
        /// </summary>
        public clsNetPingMenu()
        {            
            this._hostList = new List<clsNetPingControllerBase>();  //create list with hosts to scan pinging
            this.AlertsEnabled = false;

            this.MenuState = menuState.msScanning;
            
            this.addNewHost("Add Item...", false, true); //we add visable item that is NOT a selectable item

            this.printBanner(); //print banner
            this.printMenu();   //update menu state

            this._startTime = Environment.TickCount; //init start time

            //spawn workerthread for Audio Alerts
            /*Thread audiothread = new Thread(new ThreadStart(this.generateAudibleAlert));
            audiothread.Name = "AlertThread";
            audiothread.IsBackground = true;
            audiothread.Priority = ThreadPriority.Lowest;
            audiothread.Start();
            //audiothread.Join();
            */
        }
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Audible Alerts
        /// </summary>
        public bool AlertsEnabled
        {
            get
            {
                return this._alertsEnabled;
            }
            set
            {
                this._alertsEnabled = value;
            }
        }

        /// <summary>
        /// Hosts tracking
        /// </summary>
        public bool HostsOnline
        {
            get
            {
                return this._hostsOnline;
            }
            set
            {
                this._hostsOnline = value;
            }
        }

        /// <summary>
        /// State
        /// </summary>
        public menuState MenuState
        {
            get
            {
                return this._state;
            }
            set
            {
                this._state = value;
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// General function to print text on screen
        /// </summary>
        private void printText(string msg, int x=0, int y=0, bool newline = true)
        {
            Console.SetCursorPosition(x, y);

            if (newline)
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.Write(msg);
            }

            Console.SetCursorPosition(Console.WindowWidth -1, Console.WindowHeight - 2);
        }

        /// <summary>
        /// Menu banner (header and footer)
        /// </summary>
        private void printBanner()
        {
            Console.Clear();

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.Clear(); //apply color to overall console wnd

            printText("", 0, 0);
            printText(" ++++++++++++++++++++++++++++++++++++++++++++", 0, 1);
            printText(" + " + clsNetPingVersion.getApplicationName() + "                     +", 0, 2);
            printText(" + Published under GPLv3 (App Version " + clsNetPingVersion.getApplicationVersion() + ") +", 0, 3);
            printText(" ++++++++++++++++++++++++++++++++++++++++++++", 0, 4);
            printText("", 0, 5);
            
#if DEBUG
            printText(" " + Convert.ToChar(016).ToString() + " DEBUG COMPILED", 55, 1);  //DEBUG
#endif

            Console.BackgroundColor = ConsoleColor.DarkGreen;

            //print correct status bar comments
            switch (this.MenuState) 
            {
                case menuState.msScanning:
                    printText(" ESC = Quit, H = Help.                                                        ", 1, Console.WindowHeight - 2);
                    break;
                case menuState.msAddPC:
                    printText(" Enter a hostname and press RETURN.                                           ", 1, Console.WindowHeight - 2);
                    break;
                case menuState.msFlushDns:
                    printText(" Press RETURN.                                                                ", 1, Console.WindowHeight - 2);
                    break;
                case menuState.msHelp:
                    printText(" Press RETURN.                                                                ", 1, Console.WindowHeight - 2);
                    break;
                case menuState.msAlerts:
                    printText(" Press RETURN.                                                                ", 1, Console.WindowHeight - 2);
                    break;
            }

            Console.BackgroundColor = ConsoleColor.DarkBlue;
        }
        
        /// <summary>
        /// Ping process. 
        /// (This is called from the Delegate.)
        /// </summary>
        private void processHostPings()
        {
            int ioffsetPosY = 7;       //starting screen offset position

            if (_hostList.Count > 0)
            {
                this.processPing();  //invoke delegate call

                foreach (clsNetPingControllerBase host in _hostList)
                {
                    if (host.Selected)
                    {
                        //print highlighted (selected) host
                        printText(" " + Convert.ToChar(016).ToString() + " " + host.Output, 0, ioffsetPosY++);
                    }
                    else
                    {
                        //print all non-highlighted hosts
                        printText("   " + host.Output, 0, ioffsetPosY++);
                    }

                    //check if one of the hosts is offline and Flag it
                    if (!host.Online)
                    {
                        this.HostsOnline = false;
                    }
                }

                this.generateAudibleAlert();
            }           
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Change logic flow control state of the Menu
        /// </summary>
        public void changeMenuState(menuState state)
        {
            this.MenuState = state;
        }

        /// <summary>
        /// Print the Menu, based on logic state
        /// </summary>
        public void printMenu()
        {
            switch (this.MenuState)
            {
                case menuState.msScanning:
                    //scan active hosts
                    //this.printBanner();
                    this.processHostPings();                    
                    break;
                case menuState.msAddPC:
                    //add new host to scan
                    this.printBanner();
                    printText("  Provide new hostname:", 0, 7);
                    Console.SetCursorPosition(4, 8);
                    string newHost = Console.ReadLine(); //read input name

                    if (!string.IsNullOrEmpty(newHost))
                    {
                        this.addNewHost(newHost, true, false);
                        this.MenuState = menuState.msScanning;
                    } else {
                        this.MenuState = menuState.msAddPCError;
                    }
                    
                    this.printBanner();
                    break;
                case menuState.msAddPCError:
                    //show error adding pc
                    this.printBanner();
                    printText("  Sorry, can not comply.", 0, 7);
                    printText("  New hostname is empty or invalid input!", 0, 8);
                    Console.SetCursorPosition(58, 10);
                    
                    Console.ReadKey();  //wait for a (any) keypress

                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
                case menuState.msRemovePC:
                    //remove selected host
                    this.removeSelectedHost();
                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
                case menuState.msEnableDisablePC:
                    //enable-disable host ping
                    this.swapHostActiveState();
                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
                case menuState.msFlushDns:
                    //flush DNS cache
                    this.printBanner();

                    this.flushDnsStatus();  

                    printText(" DNS cache flushed!", 0, 7);
                    printText(" " + Convert.ToChar(019).ToString() +  " New addresses will be updated in cache.", 0, 8);
                    printText(" (Don" + Convert.ToChar(039).ToString() + "t forget to delete entries in DNS server caches to.)", 0, 10);

                    Console.SetCursorPosition(58, 10);
                    Console.ReadKey();  //wait for a (any) keypress

                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
                case menuState.msHelp:
                    //inline help reference
                    this.printBanner();
                    
                    printText("  Help reference", 0, 6);
                    printText(" ================", 0, 7);

                    printText(" Keyboard shortcuts:", 0, 9);
                    printText(" ESC / Q  = Quit", 0, 10);
                    printText(" H        = This Help", 0, 11);                    
                    printText(" Up       = Move Up Select", 0, 12);
                    printText(" Down     = Move Down Select", 0, 13);
                    printText(" A        = Insert host to scan", 0, 14);
                    printText(" D        = Delete selected host", 0, 15);
                    printText(" RETURN   = Enable or Disable active scanning of selected host", 0, 16);
                    printText(" F        = Clear cached DNS records", 0, 17);
                    printText(" S        = Enable or Disable Audible Alerts", 0, 18);

                    printText(" www.rott2d.net / 2012-2013 / Pieter De Ridder", 0, 20);

                    Console.SetCursorPosition(1, 21);
                    Console.ReadKey(); //wait for a (any) keypress

                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
                case menuState.msAlerts:
                    //enable-disable Alert Tones
                    this.printBanner();

                    this.swapAudibleAlert();  //enable or disabled alerts
                    this.generateAudibleAlert();

                    if (this.AlertsEnabled)
                    {
                        printText(" Audible Alerts are ENABLED.", 0, 7);
                    }
                    else {
                        printText(" Audible Alerts are DISABLED.", 0, 7);
                    }                                    

                    Console.SetCursorPosition(1, 8);
                    Console.ReadKey();  //wait for a (any) keypress

                    this.MenuState = menuState.msScanning;
                    this.printBanner();
                    break;
            }

        }

        /// <summary>
        /// Get the selected menu item's name
        /// </summary>
        public string getSelectedItemName()
        {
            string name = String.Empty;

            if (_hostList.Count > 0)
            {
                foreach (clsNetPingControllerBase host in _hostList)
                {
                    if (host.Selected)
                    {
                        name = host.Output;
                    }
                }
            }

            return name;
        }

        /// <summary>
        /// Move up in pc list
        /// </summary>
        public void moveup()
        {
            int iLastindex = -1;

            if (this._hostList.Count >= 1)
            {
                //get current selected item and find next index
                foreach (clsNetPingControllerBase host in _hostList)
                {
                    if (host.Selected)
                    {
                        iLastindex = this._hostList.IndexOf(host);

                        if ((iLastindex - 1) >= 0)
                        {
                            host.Selected = false;
                        }
                    }
                }

                if (iLastindex >= 0)
                {
                    //increase up the row
                    iLastindex--;

                    //highlight next item up the row if exists
                    if (iLastindex >= 0)
                    {
                        clsNetPingControllerBase host = this._hostList[iLastindex];
                        host.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Move down in pc list
        /// </summary>
        public void movedown()
        {
            int iLastindex = -1;

            if (this._hostList.Count > 1)
            {
                //get current selected item and find next index
                foreach (clsNetPingControllerBase host in _hostList)
                {
                    if (host.Selected)
                    {
                        iLastindex = this._hostList.IndexOf(host);

                        if ( (iLastindex + 1) <= this._hostList.Count -1)
                        {
                            host.Selected = false;
                        }
                    }
                }

                if (iLastindex > -1)
                {
                    //increase down the row
                    iLastindex++;

                    //highlight next item down the row if exists
                    if (iLastindex < this._hostList.Count)
                    {
                        clsNetPingControllerBase host = this._hostList[iLastindex];
                        host.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Add a new hostname to ping
        /// </summary>
        public void addNewHost(string hostname, bool active = true, bool selected = false)
        {
            if (this._hostList.Count <= MAX_HOSTS_TO_SCAN)
            {
                clsNetPingControllerBase newhost = new clsNetPingControllerBase(hostname);
                newhost.Active = active;
                newhost.Selected = selected;

                this._hostList.Add(newhost);
                
                processPing += new ProcessPing(newhost.triggerPing);
            }
        }

        /// <summary>
        /// Delete a host from list (current selected)
        /// </summary>
        public void removeSelectedHost()
        {
            if (this._hostList.Count > 1)
            {
                foreach (clsNetPingControllerBase host in _hostList.ToArray())  // <- ToArray() to overcome exception error's 
                {
                    if (host.Selected)
                    {
                        if (host.Hostname.ToLower() != ("Add Item...").ToLower())
                        {
                            this.removeHost(host.Hostname);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Delete a host from list (by Name)
        /// </summary>
        public void removeHost(string hostname)
        {
            if (this._hostList.Count > 1)
            {
                //int index = -1;

                foreach (clsNetPingControllerBase host in _hostList.ToArray())   // <- ToArray() to overcome exception error's
                {
                    if (host.Hostname.ToLower() == hostname.ToLower())
                    {
                        host.Active = false;
                        processPing -= new ProcessPing(host.triggerPing); //remove from delegate action
                        //index = this._hostList.IndexOf(host);
                        this._hostList.Remove(host); //delete from worker list                     
                        //this._hostList.RemoveAt(index); //delete from worker list
                    }
                }

                /*if (index != -1)
                {
                    this._hostList.RemoveAt(index); //delete from worker list
                }*/

                foreach (clsNetPingControllerBase host in _hostList)
                {
                    if (host.Hostname.ToLower() == "Add Item...".ToLower())
                    {
                        host.Selected = true;
                    }
                }
            }
        }

        /// <summary>
        /// Enable/Disable monitoring (current selected host)
        /// </summary>
        public void swapHostActiveState()
        {
            foreach (clsNetPingControllerBase host in _hostList)
            {
                if (host.Selected)
                {
                    host.Active = !host.Active;
                }
            }
        }

        /// <summary>
        /// FlushDns entries
        /// </summary>
        public void flushDnsStatus()
        {
            //instead of powershell or WMI, we'll launch a classic flushdns cmd
            //for OS compatibility
            ProcessStartInfo flushdnsInfo = new ProcessStartInfo();
            flushdnsInfo.FileName = "cmd.exe";   
            flushdnsInfo.Arguments = "/c ipconfig /flushdns";
            flushdnsInfo.WindowStyle = ProcessWindowStyle.Hidden;     //hide the command box

            Process.Start(flushdnsInfo);
        }

        /// <summary>
        /// Enable/Disable Alert tone
        /// </summary>
        public void swapAudibleAlert()
        {
            this.AlertsEnabled = !this.AlertsEnabled;

            //speaker.MayRun = this.AlertsEnabled;
        }

        /// <summary>
        /// Generate Alert PC Speaker tone if one of the hosts is Offline
        /// </summary>
        public void generateAudibleAlert()
        {            
            if (this._hostList.Count > 1)
            {
                this._endTime = Environment.TickCount; //get ending time

                if ((!this.HostsOnline) && (this.AlertsEnabled))
                {
                    //compare interval
                    if ((this._startTime - this._endTime) < 1000.0d)
                    {
                        //trigger sounds
                        clsNetPingSpeaker.Beep(510, 40);
                        clsNetPingSpeaker.Beep(550, 60);
                        clsNetPingSpeaker.Beep(750, 100);

                        //reset first trigger
                        this._startTime = Environment.TickCount;
                    }
                }
            }            
        }
    }
    #endregion

}
