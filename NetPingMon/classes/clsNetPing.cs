/*
 * NetPingMon
 * Unit: NetPingMon controller sealed class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 18-06-2011
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

using System.Net;
using System.Net.NetworkInformation;

namespace NetPingMon.classes
{
    /// <summary>
    /// NetPingMon ping class
    /// </summary>
    public sealed class clsNetPing
    {

        #region Static Methods
        /// <summary>
        /// !!! STATIC METHOD !!!
        /// Ping network host (is Alive?)
        /// </summary>
        public static bool pingHost(string strHostname, int timeout = 120, int ttl = 128)
        {
            bool status = false;

            try
            {
                Ping pingSender = new Ping();
                PingOptions poOptions = new PingOptions();

                poOptions.DontFragment = true;
                poOptions.Ttl = ttl;  //set TTL to 128 (default = 128)

                string strData = "abc123";  //send some chars to the remote host
                byte[] bBuffer = Encoding.ASCII.GetBytes(strData);
                int iTimeout = timeout;  //set timeout to 120

                PingReply pingReply = pingSender.Send(strHostname, iTimeout, bBuffer, poOptions);

                if (pingReply.Status == IPStatus.Success)
                {
                    status = true;
                }
            }
            catch
            {
                //drop exceptions
            }

            return status;
        }
        #endregion

    }
}
