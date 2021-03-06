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
using CCNetConfig.Core.Serialization;
using System.ComponentModel;
using CCNetConfig.Core.Components;

namespace CCNetConfig.Core {
  /// <summary>
  /// <para>Task Blocks are the action elements of CruiseControl.Net. They're the elements that do things, like executing a program, running tests, 
  /// or send email results.</para>
  /// <para>Task blocks must appear in either the tasks section or the publishers section. The tasks section is always run before the publishers 
  /// section, regardless of the order in which they appear. However, within each of these sections, the tasks are run in the order they appear.</para> 
  /// </summary>
  public abstract class PublisherTask : ISerialize, ICCNetObject, ICloneable {
    private string _typeName = string.Empty;
    /// <summary>
    /// Initializes a new instance of the <see cref="PublisherTask"/> class.
    /// </summary>
    /// <param name="typeName">Name of the type.</param>
    protected PublisherTask ( string typeName ) {
      this._typeName = typeName;
    }

    /// <summary>
    /// Gets the name of the type.
    /// </summary>
    /// <value>The name of the type.</value>
    [Browsable(false), ReflectorIgnore]
    public string TypeName { get { return this._typeName; } }


    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </returns>
    public override string ToString () {
      return GetType ().Name;
    }
    #region ISerialize Members

    /// <summary>
    /// Serializes this instance.
    /// </summary>
    /// <returns></returns>
    public abstract System.Xml.XmlElement Serialize();
    /// <summary>
    /// Deserializes the specified element.
    /// </summary>
    /// <param name="element">The element.</param>
    public abstract void Deserialize( System.Xml.XmlElement element );

    #endregion

    #region ICloneable Members
    /// <summary>
    /// Creates a copy of this object.
    /// </summary>
    /// <returns></returns>
    public abstract PublisherTask Clone ();
    object ICloneable.Clone () {
      return this.Clone ();
    }

    #endregion
  }
}
