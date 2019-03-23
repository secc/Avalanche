// <copyright>
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
using Rock.Attribute;
using Rock.Model;

namespace RockWeb.Plugins.Avalanche
{
    [DisplayName( "Button" )]
    [Category( "Avalanche" )]
    [Description( "A button." )]

    [ActionItemField( "Action Item", "", false )]
    [TextField( "Text", "The text of the label to be displayed. Lava enabled with the {{parameter}} available.", false )]
    [LavaCommandsField( "Enabled Lava Commands", "The Lava commands that should be enabled for this block.", false )]
    public partial class ButtonBlock : AvalancheBlock, IExpress
    {
        public Type GetExpressType()
        {
            return typeof( ButtonExpressBlock );
        }

        protected override void OnLoad( EventArgs e )
        {
            btnButton.Text = GetAttributeValue( "Text" );
        }

        public override MobileBlock GetMobile( string parameter )
        {

            throw new NotImplementedException();
        }

        protected void btnButton_Click( object sender, EventArgs e )
        {
        }
    }
}