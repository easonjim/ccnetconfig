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
using System.IO;
using System.Xml;
using CCNetConfig.Core;
using System.ComponentModel;
using CCNetConfig.Core.Components;
using System.Drawing.Design;

namespace CCNetConfig.CCNet {
  /// <summary>
  /// The <see cref="CCNetConfig.CCNet.MSBuildTask">MSBuild Task</see> is used to execute MsBuild projects, which are the default project format for 
  /// Visual Studio 2005 projects and can also be compiled by using the MSBuild application that ships with the .NET 2 Framework.
  /// In order to work with the results of MsBuild it is important to use a custom xml logger to format the build results. 
  /// For details on this, and a tutorial on how to use the task, see 
  /// <a href="http://confluence.public.thoughtworks.org/display/CCNET/Using+CruiseControl.NET+with+MSBuild">Using CruiseControl.NET with MSBuild</a>.
  /// </summary>
  /// <remarks>
  /// <p>The following parameters are passed to MSBuild as command-line arguments:</p>
  /// <table class="confluenceTable">
  /// <tbody><tr>
  /// <th class="confluenceTh"> Label </th>
  /// <th class="confluenceTh"> Description </th>
  /// <th class="confluenceTh"> Example </th>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetBuildCondition </td>
  /// <td class="confluenceTd"> The condition used to trigger the build,
  /// indicating if the build was triggered by new modifications or if it was
  /// forced. Legal values are: "IfModificationExists" or "ForceBuild" </td>
  /// <td class="confluenceTd"> ForceBuild </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetIntegrationStatus </td>
  /// <td class="confluenceTd"> The status of the current integration. Could be Success, Failure, Exception or Unknown  </td>
  /// <td class="confluenceTd"> Success </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetLabel </td>
  /// <td class="confluenceTd"> The label used to identify the CCNet build.  This label is generated by the CCNet labeller. </td>
  /// <td class="confluenceTd"> 1.0.2.120 </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetLastIntegrationStatus </td>
  /// <td class="confluenceTd"> The status of the previous integration. Could be Success, Failure, Exception or Unknown  </td>
  /// <td class="confluenceTd"> Success </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetProject </td>
  /// <td class="confluenceTd"> The name of the CCNet project that is being integrated. </td>
  /// <td class="confluenceTd"> MyProject </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetBuildDate </td>
  /// <td class="confluenceTd"> The date of the build (in yyyy-MM-dd format) </td>
  /// <td class="confluenceTd"> 2005-08-10 </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetBuildTime </td>
  /// <td class="confluenceTd"> The time of the start of the build (in HH:mm:ss format) </td>
  /// <td class="confluenceTd"> 08:45:12 </td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetArtifactDirectory </td>
  /// <td class="confluenceTd"> The <a href="http://confluence.public.thoughtworks.org/display/CCNET/Project+Configuration+Block#ProjectConfigurationBlock-artifactDirectory" title="artifactDirectory on Project Configuration Block">project artifact directory</a> </td>
  /// <td class="confluenceTd"> <tt>c:\program files\CruiseControl.NET\Server\MyProject\Artifacts</tt></td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd"> CCNetWorkingDirectory </td>
  /// <td class="confluenceTd"> The <a href="http://confluence.public.thoughtworks.org/display/CCNET/Project+Configuration+Block#ProjectConfigurationBlock-workingDirectory" title="workingDirectory on Project Configuration Block">project working directory</a> </td>
  /// <td class="confluenceTd"> <tt>c:\program files\CruiseControl.NET\Server\MyProject\WorkingDirectory</tt></td>
  /// </tr>
  /// <tr>
  /// <td class="confluenceTd">CCNetRequestSource</td>
  /// <td class="confluenceTd">The source of the integration request; this
  /// will generally be the name of the trigger that raised the request.
  /// (Added in CCNet 1.1)</td>
  /// <td class="confluenceTd">IntervalTrigger</td></tr></tbody></table>
  /// </remarks>
  [ MinimumVersion( "1.0" ) ]
  public class MSBuildTask : PublisherTask, ICCNetDocumentation {
    private string _executable = null;
    private string _workingDirectory = null;
    private string _projectFile = string.Empty;
    private string _buildArgs = string.Empty;
    private CloneableList<string> _targets;
    private int? _timeout = null;
    private string _logger = string.Empty;

    /// <summary>
    /// Initializes a new instance of the <see cref="MSBuildTask"/> class.
    /// </summary>
    public MSBuildTask () : base ( "msbuild" ) {

    }

    /// <summary>
    /// The location of the MSBuild.exe executable.
    /// </summary>
    [Description ( "The location of the MSBuild.exe executable." ), DefaultValue ( null ), Category ( "Optional" ),
    Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ), FileTypeFilter ( "MSBuild|msbuild.exe" ),
    OpenFileDialogTitle ( "Select MSBuild executable." )]
    public string Executable { get { return this._executable; } set { this._executable = value; } }
    /// <summary>
    /// The directory to run MSBuild in � this is generally the directory containing your build project. If relative, it is a subdirectory of 
    /// the <see cref="CCNetConfig.Core.Project.WorkingDirectory">Project Working Directory</see>
    /// </summary>
    [Description ("The directory to run MSBuild in � this is generally the directory containing your build project. If relative, it is a subdirectory of " +
    "the project working directory" ), DefaultValue ( null ), Category ( "Optional" ),
 Editor ( typeof ( BrowseForFolderUIEditor ), typeof ( UITypeEditor ) ),
BrowseForFolderDescription ( "Select path to the working directory." )]
    public string WorkingDirectory { get { return this._workingDirectory; } set { this._workingDirectory = value; } }
    /// <summary>
    /// The name of the build project to run, relative to the <see cref="CCNetConfig.CCNet.MSBuildTask.WorkingDirectory">WorkingDirectory</see>.
    /// </summary>
    [Description ( "The name of the build project to run, relative to the WorkingDirectory." ), DefaultValue ( null ),
   Category ( "Optional" ),
   Editor ( typeof ( OpenFileDialogUIEditor ), typeof ( UITypeEditor ) ), FileTypeFilter ( "Build Scripts|*.msbuild;*.csproj;*.vbproj;*.sln;*.vjproj|All Files|*.*" ),
   OpenFileDialogTitle ( "Select MSBuild script" )]
    public string MSBuildFile { get { return this._projectFile; } set { this._projectFile = value; } }
    /// <summary>
    /// Any extra arguments to pass through to MSBuild.
    /// </summary>
    [Description ( "Any extra arguments to pass through to MSBuild." ), DefaultValue ( null ), Category ( "Optional" )]
    public string BuildArguments { get { return this._buildArgs; } set { this._buildArgs = value; } }
    /// <summary>
    /// A list of the targets to run
    /// </summary>
    [Description ("A list of the targets to run"), DefaultValue (null), Editor (typeof (StringListUIEditor), typeof (UITypeEditor)),
   TypeConverter ( typeof ( StringListTypeConverter ) ), Category ( "Optional" )]
    public CloneableList<string> Targets { get { return this._targets; } set { this._targets = value; } }
    /// <summary>
    /// Number of seconds to wait before assuming that the process has hung and should be killed.
    /// </summary>
    [Description ( "Number of seconds to wait before assuming that the process has hung and should be killed." ), DefaultValue ( null ), 
    Category ( "Optional" )]
    public int? Timeout { get { return this._timeout; } set { this._timeout = value; } }
    /// <summary>
    /// The fully qualified name (class and assembly) of the custom logger to use. Arguments can be passed to the logger by 
    /// appending them after the logger name separated by a semicolon
    /// </summary>
    [Description ("The fully qualified name (class and assembly) of the custom logger to use. Arguments can be passed to the logger by " +
     "appending them after the logger name separated by a semicolon." ), DefaultValue ( null ), Category ( "Optional" )]
    public string Logger { get { return this._logger; } set { this._logger = value; } }

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public override System.Xml.XmlElement Serialize () {
      XmlDocument doc = new XmlDocument ();
      XmlElement root = doc.CreateElement ( this.TypeName );
      //root.SetAttribute ("ccnetconfigType", string.Format ("{0}, {1}", this.GetType ().FullName, this.GetType ().Assembly.GetName ().Name));

      if ( !string.IsNullOrEmpty(this.Executable) ) {
        XmlElement ele = doc.CreateElement ( "executable" );
        ele.InnerText = this.Executable;
        root.AppendChild ( ele );
      }

      if ( !string.IsNullOrEmpty(this.WorkingDirectory) ) {
        XmlElement ele = doc.CreateElement ( "workingDirectory" );
        ele.InnerText = this.WorkingDirectory;
        root.AppendChild ( ele );
      }

      if ( !string.IsNullOrEmpty ( this.MSBuildFile ) ) {
        XmlElement ele = doc.CreateElement ( "projectFile" );
        ele.InnerText = this.MSBuildFile;
        root.AppendChild ( ele );
      }

      if ( !string.IsNullOrEmpty ( this.BuildArguments ) ) {
        XmlElement ele = doc.CreateElement ( "buildArgs" );
        ele.InnerText = this.BuildArguments;
        root.AppendChild ( ele );
      }

      if ( this.Targets != null && this.Targets.Count > 0 ) {
        StringBuilder targs = new StringBuilder ();
        foreach ( string s in this.Targets )
          targs.Append ( string.Format ( "{0};", s ) );
        if ( targs.Length > 0 )
          targs.Length = targs.Length - 1;

        XmlElement ele = doc.CreateElement ( "targets" );
        ele.InnerText = targs.ToString();
        root.AppendChild ( ele );
      }

      if ( !string.IsNullOrEmpty ( this.Logger ) ) {
        XmlElement ele = doc.CreateElement ( "logger" );
        ele.InnerText = this.Logger;
        root.AppendChild ( ele );
      }

      if ( this.Timeout.HasValue ) {
        XmlElement ele = doc.CreateElement ( "timeout" );
        ele.InnerText = this.Timeout.Value.ToString();
        root.AppendChild ( ele );
      }
      return root;
    }

    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public override PublisherTask Clone () {
      MSBuildTask mbt = this.MemberwiseClone () as MSBuildTask;
      mbt.Targets = this.Targets.Clone ();
      return mbt;
    }

    #region ICCNetDocumentation Members
    /// <summary>
    /// Gets the documentation URI.
    /// </summary>
    /// <value>The documentation URI.</value>
    [Browsable(false)]
    public Uri DocumentationUri {
      get { return new Uri ("http://confluence.public.thoughtworks.org/display/CCNET/MsBuild+Task?decorator=printable"); }
    }

    #endregion

    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public override void Deserialize( XmlElement element ) {
      this.BuildArguments = string.Empty;
      this.Executable = string.Empty;
      this.Logger = string.Empty;
      this.MSBuildFile = string.Empty;
      this.Targets = new CloneableList<string> ();
      this.Timeout = null;
      this.WorkingDirectory = string.Empty;

      if ( string.Compare (element.Name, this.TypeName, false) != 0 )
        throw new InvalidCastException (string.Format ("Unable to convert {0} to a {1}", element.Name, this.TypeName));

      string s = Util.GetElementOrAttributeValue ("executable", element);
      if ( !string.IsNullOrEmpty (s) )
        this.Executable = s;

      s = Util.GetElementOrAttributeValue ("workingDirectory", element);
      if ( !string.IsNullOrEmpty (s) )
        this.WorkingDirectory = s;

      s = Util.GetElementOrAttributeValue ("projectFile", element);
      if ( !string.IsNullOrEmpty (s) )
        this.MSBuildFile = s;

      s = Util.GetElementOrAttributeValue ("buildArgs", element);
      if ( !string.IsNullOrEmpty (s) )
        this.BuildArguments = s;

      s = Util.GetElementOrAttributeValue ("logger", element);
      if ( !string.IsNullOrEmpty (s) )
        this.Logger = s;

      s = Util.GetElementOrAttributeValue ("targets", element);
      if ( !string.IsNullOrEmpty (s) ) {
        string[] targs = s.Split (new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
        foreach ( string starg in targs )
          this.Targets.Add (starg);
      }

      s = Util.GetElementOrAttributeValue ("timeout", element);
      if ( !string.IsNullOrEmpty (s) ) {
        int i = 0;
        if ( int.TryParse (s, out i) )
          this.Timeout = i;
      }

    }
  }
}
