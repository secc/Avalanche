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
using System.ComponentModel;
using System.ComponentModel.Composition;
using Rock;
using Rock.Attribute;
using Rock.Web.Cache;

namespace Avalanche.Components
{
    [Description( "Configuration component for Avalanche apps." )]
    [Export( typeof( Rock.Extension.Component ) )]
    [ExportMetadata( "ComponentName", "Avalanche Configuration" )]

    [LinkedPage( "Home Page", "Homepage of the app." )]
    [LinkedPage( "Footer Page", "Optional page representing the non-updating footer of the app.", false )]
    [TextField( "App Attributes", "Key Value attributes to be sent to the app to change it's behavior." )]
    [TextField( "Person Attributes", "List of ids of attributes to pass on to the app when requesting a person." )]
    [TextField( "Member Group Types", "List of group type is to check to see if the user is a member of." )]
    [TextField( "Member Groups", "List of group ids to check to see if the user is a member of." )]
    public class AvalancheComponent : Rock.Extension.Component
    {
        private const string homePageCacheKey = "AvalancheComponent_HomePage";
        private const string footerPageCacheKey = "AvalancheComponent_FooterPage";
        private const string appAttributesCacheKey = "AvalancheComponent_AppAttributes";
        private const string personAttributesCacheKey = "AvalancheComponent_PersonAttributes";
        private const string memberGroupTypesCacheKey = "AvalancheComponent_MemberGroupTypes";
        private const string memberGroupsCacheKey = "AvalancheComponent_MemberGroups";
        public int HomePageId
        {
            get
            {
                if ( RockCache.Get( homePageCacheKey ) is int value )
                {
                    return value;
                }
                value = GetAttributeValue( "HomePage" ).AsInteger();
                RockCache.AddOrUpdate( homePageCacheKey, value );
                return value;
            }
            set
            {
                SetAttributeValue( "HomePage", value.ToString() );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( homePageCacheKey, value );
            }
        }

        public int FooterPageId
        {
            get
            {
                if ( RockCache.Get( footerPageCacheKey ) is int value )
                {
                    return value;
                }
                value = GetAttributeValue( "FooterPage" ).AsInteger();
                RockCache.AddOrUpdate( footerPageCacheKey, value );
                return value;
            }
            set
            {
                SetAttributeValue( "FooterPage", value.ToString() );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( footerPageCacheKey, value );
            }
        }
        public string AppAttributes
        {
            get
            {
                if ( RockCache.Get( appAttributesCacheKey ) is string value )
                {
                    return value;
                }
                value = GetAttributeValue( "AppAttributes" );
                RockCache.AddOrUpdate( appAttributesCacheKey, value );
                return value ?? "";
            }
            set
            {
                SetAttributeValue( "AppAttributes", value );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( appAttributesCacheKey, value );
            }
        }

        public string PersonAttributes
        {
            get
            {
                if ( RockCache.Get( personAttributesCacheKey ) is string value )
                {
                    return value;
                }
                value = GetAttributeValue( "PersonAttributes" );
                RockCache.AddOrUpdate( personAttributesCacheKey, value );
                return value ?? "";
            }
            set
            {
                SetAttributeValue( "PersonAttributes", value );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( personAttributesCacheKey, value );
            }
        }

        public string MemberGroupTypes
        {
            get
            {
                if ( RockCache.Get( memberGroupTypesCacheKey ) is string value )
                {
                    return value;
                }
                value = GetAttributeValue( "MemberGroupTypes" );
                RockCache.AddOrUpdate( memberGroupTypesCacheKey, value );
                return value ?? "";
            }
            set
            {
                SetAttributeValue( "MemberGroupTypes", value );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( memberGroupTypesCacheKey, value );
            }
        }

        public string MemberGroups
        {
            get
            {
                if ( RockCache.Get( memberGroupsCacheKey ) is string value )
                {
                    return value;
                }
                value = GetAttributeValue( "MemberGroups" );
                RockCache.AddOrUpdate( memberGroupsCacheKey, value );
                return value ?? "";
            }
            set
            {
                SetAttributeValue( "MemberGroups", value );
                this.SaveAttributeValues();
                RockCache.AddOrUpdate( memberGroupsCacheKey, value );
            }
        }
    }
}
