/*
 * NetPingMon
 * Unit: NetPingMon Main program loop
 * Project owner & creator: Pieter De Ridder
 * Project website: http://www.rott2d.net
 * Creation date: 31-05-2012
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

using System.Windows.Forms;
//using System.Drawing;         //Invoke Icon
//using System.Reflection;      //Invoke Assembly
using System.Threading;       //Invoke Threading

using NetPingMon.classes;

namespace NetPingMon
{
    public class Program
    {

        #region Public consts
        /// <summary>
        /// Public vars
        /// </summary>
        public const int SPEED_STEPS = 100;
        #endregion

        #region Public vars
        /// <summary>
        /// Public vars
        /// </summary>
        public static clsNetPingMenu menu;   //menu class
        public static ConsoleKeyInfo cki;    //keyboard information class
        #endregion

        /// <summary>
        /// NetPingMon Main loop
        /// </summary>
        static void Main(string[] args)
        {           
            //init app
            initApp();

            //start process loop
            while (true)
            {
                Thread.Sleep(SPEED_STEPS);  //slow down process

                //poll keyboard input
                if (Console.KeyAvailable)
                {                  
                    cki = Console.ReadKey(true);

                    if (menu != null)
                    {
                        switch (cki.Key)
                        {
                            case ConsoleKey.DownArrow:
                                menu.movedown();
                                break;
                            case ConsoleKey.UpArrow:
                                menu.moveup();
                                break;
                            case ConsoleKey.A:
                                menu.changeMenuState(menuState.msAddPC);
                                break;
                            case ConsoleKey.D:
                                menu.changeMenuState(menuState.msRemovePC);
                                break;
                            case ConsoleKey.Insert:
                                menu.changeMenuState(menuState.msAddPC);
                                break;
                            case ConsoleKey.Delete:
                                menu.changeMenuState(menuState.msRemovePC);
                                break;
                            case ConsoleKey.Enter:
                                if (menu.getSelectedItemName().ToLower() == "add item...")
                                {
                                    menu.changeMenuState(menuState.msAddPC);
                                }
                                else
                                {
                                    menu.changeMenuState(menuState.msEnableDisablePC);
                                }
                                break;
                            case ConsoleKey.F:
                                menu.changeMenuState(menuState.msFlushDns);
                                break;
                            case ConsoleKey.H:
                                menu.changeMenuState(menuState.msHelp);
                                break;
                            case ConsoleKey.S:
                                menu.changeMenuState(menuState.msAlerts);
                                break;
                            case ConsoleKey.Escape:
                                Environment.Exit(0);
                                break;
                            case ConsoleKey.Q:
                                Environment.Exit(0);
                                break;
                            default:
                                break;
                        } //switch - end                            
                    }   //if (menu != null) - end
                }  //if key available - end

                menu.printMenu(); //update screen

            }  //while(true) - end

            //deinitApp();     //commented: never excuted by compiler!
        }

        /// <summary>
        /// Init application vars
        /// </summary>
        private static void initApp()
        {
            //title
            Console.Title = "Network Ping Monitor";

            Thread.CurrentThread.Name = "Main";

            //icon from exe
            /*            
            try
            {
                Icon appIcon = Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location);
            }
            catch (ArgumentException iconEx)
            {
                MessageBox.Show("error loading icon resource!");
            }   
            */
            
            //create "menu" object in memory
            menu = new clsNetPingMenu();

            //create "keyboard info" object in memory
            cki = new ConsoleKeyInfo();
        }

        /// <summary>
        /// Cleanup memory
        /// </summary>
        private static void deinitApp()
        {
           
        }      

    }
}
