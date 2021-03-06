/*
 * Copyright (c) 2006, Ryan Conrad. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
 * - Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
 * 
 * - Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the 
 *    documentation and/or other materials provided with the distribution.
 * 
 * - Neither the name of the Camalot Designs nor the names of its contributors may be used to endorse or promote products derived from this software 
 *    without specific prior written permission.
 * 
 * THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
 * LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
 * GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT 
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH 
 * DAMAGE.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using CCNetConfig.Updater.Core;
using CCNetConfig.Core.Enums;

namespace CCNetConfig.Updater {
	/// <summary>
	/// Updater GUI
	/// </summary>
	public static class Program {
		/// <summary>
		/// Main.
		/// </summary>
		/// <param name="arguments">The arguments.</param>
		[STAThread]
		public static void Main ( string[] arguments ) {
			//arguments = @"/mode=AllBuilds /version=1.0.918.12481 -app=D:\Projects\CCNetConfig\trunk\CCNetConfig\bin\Debug\ccnetconfig.exe -pv=1.0.0.0 -url=http:\\ccnetconfig.org\ccnetplugins\releasebuilds.xml".Split ( new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries );
			CommandLineArguments args = new CommandLineArguments ( arguments );
			if ( args.ContainsParam ( "app" ) )
				CallingApplication = args[ "app" ];

			if ( args.ContainsParam ( "url" ) ) {
				Program.UpdateUrl = new Uri ( args[ "url" ].Replace("\\","/"));
			}

			if ( args.ContainsParam ( "pv" ) ) {
				Program.ProductVersion = new Version ( args[ "pv" ] );
			} else {
				Program.ProductVersion = new Version ( Application.ProductVersion );
			}

			if ( args.ContainsParam ( "mode" ) )
				UpdateCheckType = StringToUpdateCheckType ( args[ "mode" ] );
			else
				UpdateCheckType = UpdateCheckType.ReleaseBuilds;

			if ( args.ContainsParam ( "version" ) )
				Program.GetUpdateVersion = new Version ( args[ "version" ] );

			Application.Run ( new MainForm () );

		}

		/// <summary>
		/// converts a string to <see cref="T:CCNetConfig.Updater.Core.UpdateCheckType"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns></returns>
		private static UpdateCheckType StringToUpdateCheckType ( string value ) {
			if ( !string.IsNullOrEmpty ( value ) ) {
				object obj = Enum.Parse ( typeof ( UpdateCheckType ), value, true );
				if ( obj != null )
					return (UpdateCheckType)obj;
				else
					return UpdateCheckType.ReleaseBuilds;
			} else
				return UpdateCheckType.ReleaseBuilds;
		}

		/// <summary>
		/// Gets the calling application.
		/// </summary>
		/// <value>The calling application.</value>
		public static string CallingApplication { get; private set; }

		/// <summary>
		/// Gets the type of the update check.
		/// </summary>
		/// <value>The type of the update check.</value>
		public static UpdateCheckType UpdateCheckType { get; private set; }

		/// <summary>
		/// Gets the get update version.
		/// </summary>
		/// <value>The get update version.</value>
		public static Version GetUpdateVersion { get; private set; }

		public static Version ProductVersion { get; private set; }

		public static Uri UpdateUrl { get; private set; }
	}
}
