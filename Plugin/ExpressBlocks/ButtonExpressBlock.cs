using System.Threading.Tasks;
using Avalanche.Models;

namespace Avalanche.ExpressBlocks
{
    public class ButtonExpressBlock : ExpressBlock
    {
        public async override Task<MobileBlock> GetMobile( string parameter )
        {
            CustomAttributes.Add( "Text", AvalancheUtilities.ProcessLava( GetAttributeValue( "Text" ),
                                                                          CurrentPerson,
                                                                          parameter,
                                                                          GetAttributeValue( "EnabledLavaCommands" ) ) );

            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                   CustomAttributes,
                                   CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                   GetAttributeValue( "EnabledLavaCommands" ),
                                   parameter );


            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.ButtonBlock",
                Attributes = CustomAttributes
            };
        }
    }
}
