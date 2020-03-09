/*
 * NetPingMon
 * Unit: NetPingMon application version sealed class
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 30-06-2012
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
    public sealed class clsNetPingVersion
    {

        #region consts
        private const string APP_NAME = "Network Ping Monitor";
        #endregion
        
        #region static methods
        /// <summary>
        /// Name string
        /// </summary>
        public static string getApplicationName()
        {
            return APP_NAME;
        }

        /// <summary>
        /// Version string
        /// </summary>
        public static string getApplicationVersion()
        {
            /*
             * versie bepaald in Properties\AssemblyInfo.cs
             *  [assembly: System.Reflection.Assembly.AssemblyVersion("x.x.x.x")]
             *  [assembly: AssemblyFileVersion("x.x.x.x")]
             */

            string[] arrVersionNumbers;  //array met nummers

            System.Reflection.Assembly a = System.Reflection.Assembly.GetExecutingAssembly();
            System.Reflection.AssemblyName an = a.GetName();

            arrVersionNumbers = an.Version.ToString().Split('.');  //zet versie nummers om naar array

            return arrVersionNumbers[0] + "." + arrVersionNumbers[1] + arrVersionNumbers[2]; //geef enkel eerste twee versie nummers (vb: 1.0)
        }
        #endregion

    }
}
