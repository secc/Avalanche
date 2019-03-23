﻿// <copyright>
// Copyright Southeast Christian Church
//
// Licensed under the  Southeast Christian Church License (the "License");
// you may not use this file except in compliance with the License.
// A copy of the License shoud be included with this file.
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>
//
using System;
using System.ComponentModel;
using Avalanche;
using Avalanche.Attribute;
using Avalanche.ExpressBlocks;
using Avalanche.Models;
using Rock;
using Rock.Attribute;
using Rock.Model;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Label Block" )]
    [Category( "Avalanche" )]
    [Description( "A button." )]

    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    [ActionItemField( "Action Item", "", false )]
    [TextField( "Text", "The text of the label to be displayed.", false )]
    public partial class LabelBlock : AvalancheBlock, IExpress
    {

        public Type GetExpressType()
        {
            return typeof( LabelExpressBlock );
        }

        public override MobileBlock GetMobile( string parameter )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load" /> event.
        /// </summarysni>
        /// <param name="e">The <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnLoad( EventArgs e )
        {
            lbLabel.Text = GetAttributeValue( "Text" );
            var fontsize = GetAttributeValue( "FontSize" ).AsInteger();
            if ( fontsize != 0 )
            {
                lbLabel.Style.Add( "font-size", GetAttributeValue( "FontSize" ) + "px" );
            }
            if ( !string.IsNullOrWhiteSpace( GetAttributeValue( "TextColor" ) ) )
            {
                lbLabel.Style.Add( "color", GetAttributeValue( "TextColor" ) );
            }
        }
    }
}