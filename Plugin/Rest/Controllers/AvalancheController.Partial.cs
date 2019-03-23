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
using System.Data.Entity;
using System.Linq;
using System.Net.Http;
using System.ServiceModel.Channels;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Compilation;
using System.Web.Http;
using Avalanche.Components;
using Avalanche.Models;
using Avalanche.Rest.Helpers;
using Avalanche.Transactions;
using Newtonsoft.Json;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Rest;
using Rock.Rest.Filters;
using Rock.Security;
using Rock.Transactions;
using Rock.Web.Cache;
using Rock.Web.UI;

namespace Avalanche.Rest.Controllers
{
    public class AvalancheController : ApiControllerBase
    {
        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/home" )]
        public async Task<IHttpActionResult> GetHome()
        {
            this.Configuration.Formatters.Add( new BrowserJsonFormatter() );

            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );
            var avalancheComponent = new AvalancheComponent();

            var homeRequest = new HomeRequest
            {
                Attributes = avalancheComponent
                    .AppAttributes.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )
                    .Select( p => p.Split( '^' ) )
                    .ToDictionary( p => p[0], p => p[1] )
            };

            var footer = avalancheComponent.FooterPageId;
            if ( footer != 0 )
            {
                homeRequest.Footer = await RenderPage( footer );
            }

            homeRequest.Page = await RenderPage( avalancheComponent.HomePageId );

            if ( homeRequest.Page.CacheDuration > 0 )
            {
                return new CachedOkResult<HomeRequest>( homeRequest, TimeSpan.FromSeconds( homeRequest.Page.CacheDuration ), this );
            }
            else
            {
                return Ok( homeRequest );
            }
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/page/{id}" )]
        [System.Web.Http.Route( "api/avalanche/page/{id}/{*parameter}" )]
        public async Task<IHttpActionResult> GetPage( int id, string parameter = "" )
        {
            this.Configuration.Formatters.Add( new BrowserJsonFormatter() );

            var page = await RenderPage( id, parameter );
            if ( page.CacheDuration > 0 )
            {
                return new CachedOkResult<MobilePage>( page, TimeSpan.FromSeconds( page.CacheDuration ), this );
            }
            else
            {
                return Ok( page );
            }
        }

        private async Task<MobilePage> RenderPage( int id, string parameter = "" )
        {
            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            if ( !HttpContext.Current.Items.Contains( "CurrentPerson" ) )
            {
                HttpContext.Current.Items.Add( "CurrentPerson", person );
            }

            var pageCache = PageCache.Get( id );
            if ( pageCache == null || !pageCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                return new MobilePage();
            }

            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            RockPage cmsPage = ( RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            MobilePage mobilePage = new MobilePage
            {
                Layout = AvalancheUtilities.GetLayout( pageCache.Layout.Name ),
                Title = pageCache.PageTitle,
                ShowTitle = pageCache.PageDisplayTitle,
                CacheDuration = pageCache.OutputCacheDuration
            };

            foreach ( var attribute in pageCache.Attributes )
            {
                mobilePage.Attributes.Add( attribute.Key, pageCache.GetAttributeValue( attribute.Key ) );
            }
            foreach ( var block in pageCache.Blocks )
            {
                if ( block.IsAuthorized( Authorization.VIEW, person ) )
                {
                    var blockCache = BlockCache.Get( block.Id );
                    try
                    {
                        var blockType = RockCache.Get( "AvalancheBlock_" + blockCache.Id.ToString() );
                        if ( blockType != null )
                        {
                            var expressBlock = Activator.CreateInstance( ( Type ) blockType ) as ExpressBlock;
                            expressBlock.SetBlock( pageCache, blockCache, currentUser );
                            var mobileBlock = await expressBlock.GetMobile( parameter );
                            mobileBlock.BlockId = blockCache.Id;
                            mobileBlock.Zone = blockCache.Zone;
                            mobilePage.Blocks.Add( mobileBlock );
                        }
                        else
                        {
                            var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                            if ( control is IExpress )
                            {
                                var type = ( ( IExpress ) control ).GetExpressType();
                                RockCache.AddOrUpdate( "AvalancheBlock_" + blockCache.Id.ToString(), type );
                                var expressBlock = Activator.CreateInstance( type ) as ExpressBlock;
                                expressBlock.SetBlock( pageCache, blockCache, currentUser );
                                var mobileBlock = await expressBlock.GetMobile( parameter );
                                mobileBlock.BlockId = blockCache.Id;
                                mobileBlock.Zone = blockCache.Zone;
                                mobilePage.Blocks.Add( mobileBlock );
                            }
                            else if ( control is RockBlock && control is IMobileResource )
                            {
                                control.SetBlock( pageCache, blockCache );
                                var mobileResource = control as IMobileResource;
                                var mobileBlock = mobileResource.GetMobile( parameter );
                                mobileBlock.BlockId = blockCache.Id;
                                mobileBlock.Zone = blockCache.Zone;
                                mobilePage.Blocks.Add( mobileBlock );
                            }
                        }
                    }
                    catch ( Exception e )
                    {
                        ExceptionLogService.LogException( e, HttpContext.Current );
                    }
                }
            }
            return mobilePage;
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*request}" )]
        public async Task<IHttpActionResult> BlockGetRequest( int id, string request = "" )
        {
            this.Configuration.Formatters.Add( new BrowserJsonFormatter() );

            var blockResponse = await RenderBlockGetRequest( id, request );
            if ( blockResponse.CacheDuration > 0 )
            {
                return new CachedOkResult<MobileBlockResponse>( blockResponse, TimeSpan.FromSeconds( blockResponse.CacheDuration ), this );
            }
            else
            {
                return Ok( blockResponse );
            }
        }

        private async Task<MobileBlockResponse> RenderBlockGetRequest( int id, string request )
        {
            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            if ( !HttpContext.Current.Items.Contains( "CurrentPerson" ) )
            {
                HttpContext.Current.Items.Add( "CurrentPerson", person );
            }
            var blockCache = BlockCache.Get( id );
            if ( blockCache == null )
            {
                return new MobileBlockResponse();
            }
            var pageCache = PageCache.Get( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                var blockType = RockCache.Get( "AvalancheBlock_" + blockCache.Id.ToString() );
                if ( blockType != null )
                {
                    var expressBlock = Activator.CreateInstance( ( Type ) blockType ) as ExpressBlock;
                    expressBlock.SetBlock( pageCache, blockCache, currentUser );
                    var mobileBlockResponse = await expressBlock.HandleRequest( request, new Dictionary<string, string>() );
                    return mobileBlockResponse;
                }
                else
                {
                    var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                    if ( control is IExpress )
                    {
                        var type = ( ( IExpress ) control ).GetExpressType();
                        RockCache.AddOrUpdate( "AvalancheBlock_" + blockCache.Id.ToString(), type );
                        var expressBlock = Activator.CreateInstance( type ) as ExpressBlock;
                        expressBlock.SetBlock( pageCache, blockCache, currentUser );
                        var mobileBlockResponse = await expressBlock.HandleRequest( request, new Dictionary<string, string>() );
                        return mobileBlockResponse;
                    }
                    else if ( control is RockBlock && control is IMobileResource )
                    {
                        control.SetBlock( pageCache, blockCache );
                        var mobileResource = control as IMobileResource;
                        var mobileBlockResponse = mobileResource.HandleRequest( request, new Dictionary<string, string>() );
                        return mobileBlockResponse;
                    }
                }
            }
            return new MobileBlockResponse();
        }


        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/block/{id}" )]
        [System.Web.Http.Route( "api/avalanche/block/{id}/{*arg}" )]
        public async Task<MobileBlockResponse> BlockPostRequest( int id, string arg = "" )
        {
            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            if ( !HttpContext.Current.Items.Contains( "CurrentPerson" ) )
            {
                HttpContext.Current.Items.Add( "CurrentPerson", person );
            }
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            var body = JsonConvert.DeserializeObject<Dictionary<string, string>>( content );
            var blockCache = BlockCache.Get( id );
            var pageCache = PageCache.Get( blockCache.PageId ?? 0 );
            string theme = pageCache.Layout.Site.Theme;
            string layout = pageCache.Layout.FileName;
            string layoutPath = PageCache.FormatPath( theme, layout );
            Rock.Web.UI.RockPage cmsPage = ( Rock.Web.UI.RockPage ) BuildManager.CreateInstanceFromVirtualPath( layoutPath, typeof( Rock.Web.UI.RockPage ) );

            if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
            {
                if ( blockCache.IsAuthorized( Authorization.VIEW, person ) )
                {
                    var blockType = RockCache.Get( "AvalancheBlock_" + blockCache.Id.ToString() );
                    if ( blockType != null )
                    {
                        var expressBlock = Activator.CreateInstance( ( Type ) blockType ) as ExpressBlock;
                        expressBlock.SetBlock( pageCache, blockCache, currentUser );
                        var mobileBlockResponse = await expressBlock.HandleRequest( arg, body );
                        mobileBlockResponse.CacheDuration = 0;
                        return mobileBlockResponse;
                    }
                    else
                    {
                        var control = ( RockBlock ) cmsPage.TemplateControl.LoadControl( blockCache.BlockType.Path );

                        if ( control is IExpress )
                        {
                            var type = ( ( IExpress ) control ).GetExpressType();
                            RockCache.AddOrUpdate( "AvalancheBlock_" + blockCache.Id.ToString(), type );
                            var expressBlock = Activator.CreateInstance( type ) as ExpressBlock;
                            expressBlock.SetBlock( pageCache, blockCache, currentUser );
                            var mobileBlockResponse = await expressBlock.HandleRequest( arg, body );
                            mobileBlockResponse.CacheDuration = 0;
                            return mobileBlockResponse;
                        }
                        else if ( control is RockBlock && control is IMobileResource )
                        {
                            control.SetBlock( pageCache, blockCache );
                            var mobileResource = control as IMobileResource;
                            var mobileBlockResponse = mobileResource.HandleRequest( arg, body );
                            mobileBlockResponse.CacheDuration = 0;
                            return mobileBlockResponse;
                        }
                    }
                }
            }
            return new MobileBlockResponse();
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/token" )]
        public async Task<RckipidToken> GetToken()
        {
            this.Configuration.Formatters.Add( new BrowserJsonFormatter() );

            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            if ( person == null )
            {
                return null;
            }
            var expiration = Rock.RockDateTime.Now.AddDays( 7 );
            RockContext rockContext = new RockContext();
            var token = Rock.Security.Encryption.GenerateUniqueToken();

            //We are going to create our own person token so we can do it async
            PersonToken personToken = new PersonToken();
            PersonAliasService personAliasService = new PersonAliasService( rockContext );
            var personAlias = await personAliasService.Queryable()
                .FirstOrDefaultAsync( pa => pa.PersonId == person.Id && pa.AliasPersonId == person.Id );

            personToken.PersonAliasId = personAlias.Id;

            personToken.ExpireDateTime = expiration;

            personToken.TimesUsed = 0;
            personToken.UsageLimit = 1;

            personToken.PageId = null;

            var personTokenService = new PersonTokenService( rockContext );
            personTokenService.Add( personToken );
            await rockContext.SaveChangesAsync();

            var encryptedToken = Rock.Security.Encryption.EncryptString( token );

            // do a Replace('%', '!') after we UrlEncode it (to make it more safely embeddable in HTML and cross browser compatible)
            encryptedToken = System.Web.HttpUtility.UrlEncode( encryptedToken ).Replace( '%', '!' );

            return new RckipidToken
            {
                Expiration = expiration,
                Token = encryptedToken
            };
        }

        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/interaction" )]
        public Dictionary<string, string> PostInteraction()
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            InteractionInformation interactionInformation = JsonConvert.DeserializeObject<InteractionInformation>( content );

            var homePageId = GlobalAttributesCache.Value( "AvalancheHomePage" ).AsInteger();
            var pageCache = PageCache.Get( homePageId );
            var siteId = pageCache.SiteId;
            var person = GetPerson();

            AppInteractionTransaction transaction = new AppInteractionTransaction()
            {
                ComponentName = "Mobile App",
                SiteId = siteId,
                PageId = interactionInformation.PageId.AsIntegerOrNull(),
                PageTitle = interactionInformation.PageTitle,
                DateViewed = Rock.RockDateTime.Now,
                Operation = interactionInformation.Operation,
                PersonAliasId = person?.PrimaryAliasId,
                InteractionData = interactionInformation.InteractionData,
                InteractionSummary = interactionInformation.InteractionSummary,
                IPAddress = GetClientIp( Request ),
                UserAgent = Request.Headers.UserAgent.ToString()
            };
            RockQueue.TransactionQueue.Enqueue( transaction );
            return new Dictionary<string, string> { { "Status", "Ok" } };
        }

        [HttpGet]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/personsummary" )]
        public async Task<PersonSummary> GetPersonSummary()
        {
            this.Configuration.Formatters.Add( new BrowserJsonFormatter() );

            AvalancheComponent avalancheComponent = new AvalancheComponent();
            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            if ( person == null )
            {
                return null;
            }
            var personSummary = new PersonSummary
            {
                NickName = person.NickName,
                FirstName = person.FirstName,
                LastName = person.LastName
            };

            RockContext rockContext = new RockContext();
            AttributeValueService attributeValueService = new AttributeValueService( rockContext );
            var attributeIds = avalancheComponent.PersonAttributes.Split( ',' ).Select( i => i.AsInteger() ).ToList();
            personSummary.Attributes = await attributeValueService.Queryable()
                .Where( av => av.EntityId == person.Id && attributeIds.Contains( av.AttributeId ) )
                .ToDictionaryAsync( av => av.Attribute.Key, av => av.Value );

            GroupMemberService groupMemberService = new GroupMemberService( rockContext );
            var groupIds = avalancheComponent.MemberGroups.Split( ',' ).Select( i => i.AsInteger() ).ToList();
            var groupTypeIds = avalancheComponent.MemberGroupTypes.Split( ',' ).Select( i => i.AsInteger() ).ToList();

            personSummary.Groups = await groupMemberService.Queryable().Where( gm => gm.PersonId == person.Id
                                    && ( groupIds.Contains( gm.GroupId ) || groupTypeIds.Contains( gm.Group.GroupTypeId ) ) )
               .Select( gm => new GroupSummary { Id = gm.GroupId, Name = gm.Group.Name, IsLeader = gm.GroupRole.IsLeader } )
               .ToListAsync();

            return personSummary;
        }

        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/registerfcm" )]
        public async Task<Dictionary<string, string>> RegisterFCM()
        {
            HttpContent requestContent = Request.Content;
            string content = requestContent.ReadAsStringAsync().Result;
            Dictionary<string, string> tokenDictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>( content );
            var user = await GetUser();
            var person = await GetPerson( user );

            if ( tokenDictionary.ContainsKey( "Token" )
                 && !string.IsNullOrWhiteSpace( tokenDictionary["Token"] )
                 && person != null )
            {
                var userAgent = Request.Headers.UserAgent.ToString();
                var deviceId = Regex.Match( userAgent, "(?<=-).+(?=\\))" ).Value.Trim();
                if ( deviceId.Length > 20 )
                {
                    deviceId = deviceId.Substring( 0, 20 );
                }
                RockContext rockContext = new RockContext();
                PersonalDevice personalDevice = await AvalancheUtilities.GetPersonalDeviceAsync( deviceId, person.PrimaryAliasId, rockContext );
                if ( personalDevice != null && personalDevice.DeviceRegistrationId != tokenDictionary["Token"] )
                {
                    personalDevice.DeviceRegistrationId = tokenDictionary["Token"];
                    personalDevice.NotificationsEnabled = true;
                    await rockContext.SaveChangesAsync();
                }
            }
            return new Dictionary<string, string> { { "Status", "Ok" } };
        }

        [HttpPost]
        [Authenticate]
        [System.Web.Http.Route( "api/avalanche/savepageviewinteractions" )]
        private async Task<Dictionary<string, string>> SavePageViewInteraction( List<PageInteraction> interactions )
        {
            UserLogin currentUser = await GetUser();
            Person person = await GetPerson( currentUser );

            foreach ( var interaction in interactions )
            {
                var page = PageCache.Get( interaction.PageId );

                AppInteractionTransaction transaction = new AppInteractionTransaction()
                {
                    PageId = page.Id,
                    SiteId = page.SiteId,
                    DateViewed = interaction.InteractionTime,
                    PageTitle = page.PageTitle,
                    Operation = "View",
                    PersonAliasId = person?.PrimaryAliasId,
                    InteractionSummary = page.PageTitle,
                    InteractionData = string.Format( "/api/avalanche/{0}/{1}", interaction.PageId, interaction.Parameter ),
                    IPAddress = GetClientIp( Request ),
                    UserAgent = Request.Headers.UserAgent.ToString()
                };
                RockQueue.TransactionQueue.Enqueue( transaction );
            }
            return new Dictionary<string, string> { { "Status", "Ok" } };
        }

        private string GetClientIp( HttpRequestMessage request )
        {
            if ( request.Properties.ContainsKey( "MS_HttpContext" ) )
            {
                return ( ( HttpContextWrapper ) request.Properties["MS_HttpContext"] ).Request.UserHostAddress;
            }

            if ( request.Properties.ContainsKey( RemoteEndpointMessageProperty.Name ) )
            {
                RemoteEndpointMessageProperty prop;
                prop = ( RemoteEndpointMessageProperty ) request.Properties[RemoteEndpointMessageProperty.Name];
                return prop.Address;
            }

            return null;
        }

        private async Task<UserLogin> GetUser()
        {
            var principal = ControllerContext.Request.GetUserPrincipal();
            if ( principal != null && principal.Identity != null )
            {
                var userLoginService = new Rock.Model.UserLoginService( new RockContext() );
                var userLogin = await userLoginService.Queryable().FirstOrDefaultAsync( u => u.UserName == principal.Identity.Name );
                return userLogin;
            }
            return null;
        }

        private async Task<Person> GetPerson( UserLogin userLogin )
        {
            if ( userLogin == null )
            {
                return null;
            }
            var personId = userLogin.PersonId;
            RockContext rockContext = new RockContext();
            PersonService personService = new PersonService( rockContext );
            var person = await personService.Queryable().FirstOrDefaultAsync( p => p.Id == personId );
            return person;

        }
    }
}