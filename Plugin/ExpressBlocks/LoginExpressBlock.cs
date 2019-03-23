using System.Threading.Tasks;
using Avalanche.Models;

namespace Avalanche.ExpressBlocks
{
    public class LoginExpressBlock : ExpressBlock
    {
        public async override Task<MobileBlock> GetMobile( string parameter )
        {
            return new MobileBlock()
            {
                BlockType = "Avalanche.Blocks.Login",
                Attributes = CustomAttributes
            };
        }
    }
}
