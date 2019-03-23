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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Avalanche.Components;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace RockWeb.Plugins.Avalanche
{
    /// <summary>
    /// Avalanche configuration
    /// </summary>
    [DisplayName( "Avalanche Configuration" )]
    [Category( "Avalanche > Settings" )]
    [Description( "Configuration settings for Avalanche." )]
    public partial class AvalancheConfiguration : Rock.Web.UI.RockBlock
    {

        protected override void OnLoad( EventArgs e )
        {
            if ( !Page.IsPostBack )
            {
                BindData();
            }
        }

        private void BindData()
        {
            var avalancheComponent = new AvalancheComponent();
            ppHome.SetValue( avalancheComponent.HomePageId );
            ppFooter.SetValue( avalancheComponent.FooterPageId );
            kvAttributes.CustomKeys = new System.Collections.Generic.Dictionary<string, string> { { "PreloadImages", "PreloadImages" } };
            kvAttributes.Value = avalancheComponent.AppAttributes;
            lPerson.Text = string.Join( ", ", GetPersonAttributes( avalancheComponent ).Select( gt => gt.Name ) );
            lGroupTypes.Text = string.Join( ", ", GetGroupTypes( avalancheComponent ).Select( gt => gt.Name ) );
            lGroups.Text = string.Join( ", ", GetGroups( avalancheComponent ).Select( g => g.Name ) );
        }

        private List<AttributeCache> GetPersonAttributes( AvalancheComponent avalancheComponent )
        {
            var aIds = avalancheComponent.PersonAttributes.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            var output = new List<AttributeCache>();
            foreach ( var id in aIds )
            {
                output.Add( AttributeCache.Get( id.AsInteger() ) );
            }
            return output;
        }

        private List<GroupTypeCache> GetGroupTypes( AvalancheComponent avalancheComponent )
        {
            var gtIds = avalancheComponent.MemberGroupTypes.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
            var output = new List<GroupTypeCache>();
            foreach ( var id in gtIds )
            {
                output.Add( GroupTypeCache.Get( id.AsInteger() ) );
            }
            return output;
        }

        private List<Group> GetGroups( AvalancheComponent avalancheComponent )
        {
            var groupids = avalancheComponent.MemberGroups.Split( ',' ).Select( s => s.AsInteger() ).ToList();
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            return groupService.GetByIds( groupids ).ToList();
        }

        protected void btnSave_Click( object sender, EventArgs e )
        {
            var avalancheComponents = new AvalancheComponent();

            avalancheComponents.HomePageId = ppHome.SelectedValue.AsInteger();
            avalancheComponents.FooterPageId = ppFooter.SelectedValue.AsInteger();
            avalancheComponents.AppAttributes = kvAttributes.Value;
            NavigateToParentPage();
        }

        protected void btnBack_Click( object sender, EventArgs e )
        {
            NavigateToParentPage();
        }

        protected void btnGroupTypes_Click( object sender, EventArgs e )
        {
            gtpGroupTypes.SetGroupTypes( GroupTypeCache.All() );
            gtpGroupTypes.SetValues( new AvalancheComponent().MemberGroupTypes.Split( ',' ) );
            mdGroupTypes.Show();
        }

        protected void mdGroupTypes_SaveClick( object sender, EventArgs e )
        {
            var avalancheComponent = new AvalancheComponent();
            avalancheComponent.MemberGroupTypes = string.Join( ",", gtpGroupTypes.SelectedValues );
            mdGroupTypes.Hide();
            BindData();
        }

        protected void btnGroups_Click( object sender, EventArgs e )
        {
            var securityGroupType = GroupTypeCache.Get( Rock.SystemGuid.GroupType.GROUPTYPE_SECURITY_ROLE ).Id;
            RockContext rockContext = new RockContext();
            GroupService groupService = new GroupService( rockContext );
            var groupIds = new AvalancheComponent().MemberGroups.Split( ',' ).Select( i => i.AsInteger() ).ToList();
            var groups = groupService.GetByIds( groupIds ).ToList();
            gpGroups.SetValues( groups.Where( g => g.GroupTypeId != securityGroupType ) );
            gtpSecurity.DataSource = groupService.Queryable().Where( g => g.GroupTypeId == securityGroupType ).ToList();
            gtpSecurity.DataBind();
            gtpSecurity.SetValues( groups.Where( g => g.GroupTypeId == securityGroupType ).Select( g => g.Id.ToString() ) );
            mdGroups.Show();
        }

        protected void mdGroups_SaveClick( object sender, EventArgs e )
        {
            var avalancheComponent = new AvalancheComponent();
            var groupIds = gpGroups.SelectedValues.ToList();
            groupIds.AddRange( gtpSecurity.SelectedValues );
            avalancheComponent.MemberGroups = string.Join( ",", groupIds );
            mdGroups.Hide();
            BindData();
        }

        protected void btnPerson_Click( object sender, EventArgs e )
        {
            var personEntityId = EntityTypeCache.Get( typeof( Person ) ).Id;
            var attributes = AttributeCache.All().Where( a => a.EntityTypeId == personEntityId );
            cblPerson.DataSource = attributes;
            cblPerson.DataBind();
            cblPerson.SetValues( new AvalancheComponent().PersonAttributes.Split( ',' ).Select( i => i.AsInteger() ) );
            mdPerson.Show();
        }

        protected void mdPerson_SaveClick( object sender, EventArgs e )
        {
            var avalanceComponent = new AvalancheComponent();
            avalanceComponent.PersonAttributes = string.Join( ",", cblPerson.SelectedValues );
            mdPerson.Hide();
            BindData();
        }
    }
}
