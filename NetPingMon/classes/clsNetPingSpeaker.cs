/*
 * NetPingMon
 * Unit: NetPingMon Simple PC Speaker sealed Class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 30-05-2012
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
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
using System.Timers;

namespace NetPingMon.classes
{

    #region PC Speaker (system) class
    /// <summary>
    /// PC Speaker player
    /// </summary>
    public sealed class clsNetPingSpeaker
    {

        #region DLL Imports
        /// <summary>
        /// PC Speaker tone playing
        /// </summary>
        [DllImport("Kernel32.dll")]
        public static extern bool Beep(UInt32 frequency, UInt32 duration);
        #endregion

        #region Methods
        /// <summary>
        /// Plays a PC Speaker beep tone
        /// </summary>
        public static void playPCSpeakerTone(uint freq, uint duration)
        {
            Beep(freq, duration);
        }
        #endregion

    }
    #endregion


}
