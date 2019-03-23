using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using Rock;
using Rock.Data;
using Rock.Model;
using Rock.Web.Cache;

namespace Avalanche.Models
{
    public class ExpressBlock : Model<ExpressBlock>
    {
        private Dictionary<string, string> _customAtributes;
        public Dictionary<string, string> CustomAttributes
        {
            get
            {
                if ( _customAtributes == null )
                {
                    _customAtributes = new Dictionary<string, string>();
                    var customs = GetAttributeValue( "CustomAttributes" ).ToKeyValuePairList();
                    foreach ( var item in customs )
                    {
                        _customAtributes[item.Key] = HttpUtility.UrlDecode( ( string ) item.Value );
                    }
                }
                return _customAtributes;
            }
        }

        public int? CurrentPersonId { get => CurrentUser?.Person?.Id; }
        public PersonAlias CurrentPersonAlias { get => CurrentUser?.Person?.PrimaryAlias; }
        public UserLogin CurrentUser { get; private set; }
        public int? CurrentPersonAliasId { get => CurrentUser?.Person?.PrimaryAliasId; }
        public Person CurrentPerson { get => CurrentUser?.Person; }
        public PageCache PageCache { get; set; }
        public BlockCache BlockCache { get; set; }

        public void SetBlock( PageCache pageCache, BlockCache blockCache, UserLogin currentUser )
        {
            PageCache = pageCache;
            BlockCache = blockCache;
            CurrentUser = currentUser;
            Attributes = blockCache.Attributes;
            AttributeValues = blockCache.AttributeValues;
        }

        public virtual async Task<MobileBlock> GetMobile( string parameter )
        {
            return new MobileBlock { BlockType = "Null" };
        }

        public virtual async Task<MobileBlockResponse> HandleRequest( string request, Dictionary<string, string> Body )
        {
            throw new NotImplementedException();
        }
    }
}
