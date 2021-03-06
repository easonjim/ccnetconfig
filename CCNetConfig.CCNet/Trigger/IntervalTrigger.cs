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
using System.Xml;
using System.ComponentModel;
using CCNetConfig.Core;
using CCNetConfig.Core.Components;
using System.Drawing.Design;

namespace CCNetConfig.CCNet {
  /// <summary>
  /// <para>The <see cref="CCNetConfig.CCNet.IntervalTrigger">Interval Trigger</see> is used to specify that an integration should be run periodically, 
  /// after a certain amount of time. By default, an integration will only be triggered if modifications have been detected since the last 
  /// integration. The <see cref="CCNetConfig.Core.Trigger">Trigger</see> can also be configured to force a build even if no changes have occurred to 
  /// source control.</para>
  /// </summary>
  [TypeConverter (typeof (ExpandableObjectConverter)),MinimumVersion( "1.0" )]
  public class IntervalTrigger : Trigger, ICCNetDocumentation {
    private string _name = string.Empty;
    private int? _seconds = null;
    private Core.Enums.BuildCondition? _buildCondition = null;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntervalTrigger"/> class.
    /// </summary>
    public IntervalTrigger() : base("intervalTrigger") {

    }

    /// <summary>
    /// The name of the trigger. This name is passed to external tools as a means to identify the trigger that requested the build. (Added in CCNet 1.1)
    /// </summary>
    [Description("The name of the trigger. This name is passed to external tools as a means to identify the trigger that requested the build. (Added in CCNet 1.1)"),
   DefaultValue ( null ), Category ( "Optional" )]
    public string Name { get { return this._name; } set { this._name = value; } }
    /// <summary>
    /// <para>The number of seconds after an integration cycle completes before triggering the next integration cycle.</para>
    /// </summary>
    [Description ("The number of seconds after an integration cycle completes before triggering the next integration cycle."),
   DefaultValue ( null ), Category ( "Optional" )]
    public int? Seconds { get { return this._seconds; } set { this._seconds = value; } }
    /// <summary>
    /// <para>The condition that should be used to launch the integration. By default, this value is 
    /// <see cref="CCNetConfig.Core.Enums.BuildCondition.IfModificationExists">IfModificationExists</see>, meaning that an integration will only be 
    /// triggered if modifications have been detected. Set this attribute to 
    /// <see cref="CCNetConfig.Core.Enums.BuildCondition.ForceBuild">ForceBuild</see> in order to ensure that a build should be launched regardless of 
    /// whether new modifications are detected.</para>
    /// </summary>
    /// <seealso cref="CCNetConfig.Core.Enums.BuildCondition" />
    [Description("The condition that should be used to launch the integration. By default, this value is " +
      "IfModificationExists, meaning that an integration will only be triggered if modifications have been detected. Set this attribute to " +
      "ForceBuild in order to ensure that a build should be launched regardless of whether new modifications are detected."),
      Editor (typeof (DefaultableEnumUIEditor), typeof (UITypeEditor)),
     TypeConverter ( typeof ( DefaultableEnumTypeConverter ) ), DefaultValue ( null ), Category ( "Optional" )]
    public Core.Enums.BuildCondition? BuildCondition { get { return this._buildCondition; } set { this._buildCondition = value; } }

    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public override Trigger Clone () {
      return this.MemberwiseClone () as IntervalTrigger;
    }
    /// <summary>
    /// Serializes the object to a <see cref="System.Xml.XmlElement"/>
    /// </summary>
    /// <returns></returns>
    public override System.Xml.XmlElement Serialize () {
      XmlDocument doc = new XmlDocument ();
      XmlElement root = doc.CreateElement (this.TypeName);
      //root.SetAttribute ("ccnetconfigType", string.Format ("{0}, {1}", this.GetType ().FullName, this.GetType ().Assembly.GetName ().Name));

      if ( !string.IsNullOrEmpty (this.Name) )
        root.SetAttribute ("name", this.Name);
      if ( this.Seconds.HasValue )
        root.SetAttribute ("seconds", this.Seconds.Value.ToString ());
      if ( this.BuildCondition.HasValue )
        root.SetAttribute ("buildCondition", this.BuildCondition.Value.ToString ());
      return root;
    }

    #region ICCNetDocumentation Members
    /// <summary>
    /// Gets the documentation URI.
    /// </summary>
    /// <value>The documentation URI.</value>
    [Browsable(false)]
    public Uri DocumentationUri {
      get { return new Uri ( "http://confluence.public.thoughtworks.org/display/CCNET/Interval+Trigger?decorator=printable" ); }
    }

    #endregion

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void Deserialize( XmlElement element ) {
      this.BuildCondition = null;
      this.Seconds = null;
      this.Name = string.Empty;

      if ( string.Compare (element.Name, this.TypeName, false) != 0 )
        throw new InvalidCastException (string.Format ("Unable to convert {0} to a {1}", element.Name, this.TypeName));

      if ( !string.IsNullOrEmpty (element.GetAttribute ("name")) )
        this.Name = element.GetAttribute ("name");
      
      if ( !string.IsNullOrEmpty (element.GetAttribute ("seconds")) ) {
        int i = 0;
        if ( int.TryParse (element.GetAttribute ("seconds"), out i) )
          this.Seconds = i;
        else
          this.Seconds = null;
      }

      if ( !string.IsNullOrEmpty (element.GetAttribute ("buildCondition")) ) {
        Core.Enums.BuildCondition bc = (Core.Enums.BuildCondition)Enum.Parse (typeof (Core.Enums.BuildCondition), element.GetAttribute("buildCondition"),true);
        this.BuildCondition = bc;
      }

    }
  }
}
