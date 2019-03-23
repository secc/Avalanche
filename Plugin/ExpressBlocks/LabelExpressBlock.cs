using System.Threading.Tasks;
using Avalanche.Models;

namespace Avalanche.ExpressBlocks
{
    public class LabelExpressBlock : ExpressBlock
    {
        public override async Task<MobileBlock> GetMobile( string parameter )
        {
            AvalancheUtilities.SetActionItems( GetAttributeValue( "ActionItem" ),
                                                           CustomAttributes,
                                                           CurrentPerson, AvalancheUtilities.GetMergeFields( CurrentPerson ),
                                                           GetAttributeValue( "EnabledLavaCommands" ),
                                                           parameter );

            CustomAttributes["Text"] = AvalancheUtilities.ProcessLava( GetAttributeValue( "Text" ),
                                                                       CurrentPerson,
                                                                       parameter,
                                                                       GetAttributeValue( "EnabledLavaCommands" ) );

            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.LabelBlock",
                Attributes = CustomAttributes
            };
        }
    }
}
