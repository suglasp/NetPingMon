/*
 * NetPingMon
 * Unit: NetPingMon controller sealed class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 19-06-2011
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

namespace NetPingMon.classes
{
    /// <summary>
    /// Core Controller class for NetPingMon
    /// </summary>
    public sealed class clsNetPingControllerBase
    {

        #region Consts
        /// <summary>
        /// Private constants
        /// </summary>
        public const int TIMEOUT_SKIPOVER_RETRIES = 20;   //timeout between each ping to a host entry
        #endregion

        #region Private vars
        /// <summary>
        /// Private vars
        /// </summary>
        private char[] _arrCharAnimation = new char[5] { '-', '\\', '|', '/', Convert.ToChar(004) };  //animation characters
        private int _iAnimationPos = 0;  //animation loop counter
        private bool _active = false; //is active?
        private bool _guiselected = false; //is selected in GUI?
        private bool _online = false;
        private bool _wasLastOffline = false;
        private int _iWasLastOfflineTimeout = 0;
        private string _sTargetHostname = String.Empty;  //moved to private vars instead of protected -> sealed class
        private string _pingOutputBuffer = String.Empty; //moved to private vars instead of protected -> sealed class
        #endregion

        #region getters & setters
        /// <summary>
        /// Getters & setters
        /// </summary>
        public bool Selected
        {
            get { return this._guiselected; }
            set { this._guiselected = value; }
        }

        public string Hostname
        {
            get { return this._sTargetHostname; }
        }

        public bool Active
        {
            get { return this._active; }
            set {
                if (this.Hostname != "Add Item...")
                {
                    this._active = value; 
                }
            }
        }

        public bool Online
        {
            get { return this._online; }
        }

        public string Output
        {
            get { return this._pingOutputBuffer; }
        }
        #endregion

        #region CTOR's
        /// <summary>
        /// Constructor
        /// </summary>
        public clsNetPingControllerBase(string targetHostname)
        {
            this._sTargetHostname = targetHostname;
            this.Selected = false;
            this.Active = false;
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Increase animation (Private method)
        /// </summary>
        private char incAnimation()
        {
            if (this.Active)
            {
                _iAnimationPos++;

                if (_iAnimationPos >= _arrCharAnimation.Length -1)
                {
                    _iAnimationPos = 0;
                }
            }
            else
            {
                _iAnimationPos = 4;
            }

            return _arrCharAnimation[_iAnimationPos];
        }

        /// <summary>
        /// Clear buffer
        /// </summary>
        private void clearOutputBuffer()
        {
            this._pingOutputBuffer = String.Empty;
        }

        /// <summary>
        /// Fill buffer
        /// </summary>
        private void fillOutputBuffer(string msg, bool showAnimation = true)
        {
            //this.clearOutputBuffer();

            if (showAnimation)
            {
                this._pingOutputBuffer = "[" + this.incAnimation() + "] " + msg;
            }
            else {
                this._pingOutputBuffer = msg;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Trigger a status update of target host
        /// </summary>
        public void triggerPing()
        {
            if (this.Hostname != "Add Item...")
            {
                if (this.Active)
                {
                    //build-in a timeout when a host is offline
                    if (this._wasLastOffline)
                    {
                        this.fillOutputBuffer(this.Hostname + " offline  ");

                        this._iWasLastOfflineTimeout++;

                        if (this._iWasLastOfflineTimeout >= TIMEOUT_SKIPOVER_RETRIES) //skip "n" ping tries
                        {
                            this._wasLastOffline = false;
                        }
                    }
                    else
                    {
                        //do ping commands
                        if (clsNetPing.pingHost(this.Hostname))
                        {
                            this.fillOutputBuffer(this.Hostname + " online  ");
                            this._online = true;
                        }
                        else
                        {
                            this.fillOutputBuffer(this.Hostname + " offline  ");
                            this._iWasLastOfflineTimeout = 0; //reset
                            this._wasLastOffline = true; //set timeout flag to true
                            this._online = false;
                        }
                    }
                }
                else
                {
                    this.fillOutputBuffer(this.Hostname + " inactive...  ");
                    this._online = false;
                }
            }
            else {
                this.fillOutputBuffer(this.Hostname, false);
                this._online = true;
            }
        }
        #endregion

    }
}
