using System;
using System.Collections.Generic;
using System.Timers;
using System.Text;

namespace NetPingMon.classes
{

    #region PC Speaker controller class
    /// <summary>
    /// PC Speaker controller class
    /// </summary>
    public sealed class clsNetPingSpeakerController
    {

        #region Public consts
        /// <summary>
        /// Public consts
        /// </summary>
        public const int AUDIO_TIMEOUT = 30000;    //30 seconds timeout
        #endregion

        #region Public consts
        /// <summary>
        /// Private vars
        /// </summary>
        private bool _mayRun = false;  //may run
        private Timer watchDogTimer;   //timer
        #endregion

        #region Getters & Setters
        /// <summary>
        /// Defines if sound keeps looping with interval
        /// </summary>
        public bool MayRun
        {
            get { return (this.watchDogTimer != null) ? this.watchDogTimer.Enabled : false; }
            set
            {
                this._mayRun = value;

                if (this.watchDogTimer != null)
                {
                    this.watchDogTimer.Enabled = this._mayRun;
                }
            }
        }
        #endregion

        #region Ctor's
        /// <summary>
        /// Ctor's
        /// </summary>
        public clsNetPingSpeakerController()
        {
            this.watchDogTimer = new Timer(AUDIO_TIMEOUT);
            this.watchDogTimer.Enabled = false;
            this.watchDogTimer.AutoReset = true;

            this.watchDogTimer.Elapsed += new ElapsedEventHandler(OnWatchDogBark);  //Assign the delegate
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Start
        /// </summary>
        public void Start()
        {
            this.MayRun = true;
        }

        /// <summary>
        /// Stop
        /// </summary>
        public void Stop()
        {
            this.MayRun = false;
        }

        /// <summary>
        /// Reset
        /// </summary>
        public void Reset()
        {
            if (this.MayRun)
            {
                //only reset if was running state
                this.MayRun = false;
                this.MayRun = true;
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// Event
        /// </summary>
        public void OnWatchDogBark(object source, ElapsedEventArgs e)
        {
            //stop doing something
            //this.watchDogTimer.Enabled = false;

            //trigger sounds
            clsNetPingSpeaker.Beep(510, 40);
            clsNetPingSpeaker.Beep(550, 60);
            clsNetPingSpeaker.Beep(750, 100);

            //start again if permitted
            if (this.MayRun)
            {
                this.watchDogTimer.Enabled = true;
            }

            //needed if Timer AutoReset is True
            GC.KeepAlive(this.watchDogTimer);
        }
        #endregion

    }
    #endregion


}
