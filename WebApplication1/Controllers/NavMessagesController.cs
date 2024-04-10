using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NavMessagesController : ControllerBase
    {
        // Public and private keys (from your previous output)
        RSAParameters publicKey = new RSAParameters
        {
            Modulus = Convert.FromBase64String("2h/vNGgtsnzM9si8Zr5e4Wy6X1v3WoiCnnKBUpK3nf5OIVjtq5raJVx/CY/u0HqZpPr8WodcNENBtHKOt9LH/SAtqS0UxNhzurMiDNxZCzsuoT9dGmBche7nUmxkclZO+0aBb+kw7TJl2pC0USr2njl9tbLdTjwxrxFQK/sidmU="),
            Exponent = Convert.FromBase64String("AQAB")
        };

        RSAParameters privateKey = new RSAParameters
        {
            Modulus = Convert.FromBase64String("2h/vNGgtsnzM9si8Zr5e4Wy6X1v3WoiCnnKBUpK3nf5OIVjtq5raJVx/CY/u0HqZpPr8WodcNENBtHKOt9LH/SAtqS0UxNhzurMiDNxZCzsuoT9dGmBche7nUmxkclZO+0aBb+kw7TJl2pC0USr2njl9tbLdTjwxrxFQK/sidmU="),
            Exponent = Convert.FromBase64String("AQAB"),
            D = Convert.FromBase64String("WxdlBQTQuK5nxlP0Yg/0fb2zY2l7tiI2MkdtfHdrGR7/r+t8beLgYSPspainOgdJLN3oD0JiHi1MPjhtI5VyEJzr1XSLaVo5SVlRjg5mcAk2g/nv0WsJWLt+lV8SxwJJdLxqKsfgvTbxfJX0gXz+YFvhgzRjI4zcI3S5ozOO0e0="),
            P = Convert.FromBase64String("+tXfXSo8oGydmvb4RGz0Px9/+YDCkAZHMS+hDLs04/AMokk3pPmfBKWid9P06y4Losw5nm2P8AAZlFMAsniHzw=="),
            Q = Convert.FromBase64String("3p2lr1HmddSqAF+dyw3TBjZF8b7KoZbtfy0VGJwA+/ME2ScelxzQoxOS6r5MsHo96sF4Ii7iv7II3eRRLtj3iw=="),
            DP = Convert.FromBase64String("0oh2fPz5e/EOa8YE5XHJo72trV5Mb5RFZtxCQaxTnUmbYQ8xPDAkL0NS1V67ADZan97oMbhmCpwa3Cq6uBGAcw=="),
            DQ = Convert.FromBase64String("ex0MSE0u+vNFoTc/+NAIaGMTg4JUZdEmPzMbe6SrFqtrfvyXelLBP67PjWr41pCENZQRcDYlyIYZST2/d/0dyQ=="),
            InverseQ = Convert.FromBase64String("NRsBBn1YQcazRsdIHdlOazszZX8P3f9yZcCpFGl3l5Dz750O3zoQtbyI2fbhq9zdNoI/TiIRCPa5xMAljCiG/g==")
        };
        private readonly WebApplication1Context _context;

        public NavMessagesController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: api/NavMessages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<NavMessage>>> GetNavMessage()
        {
            return await _context.NavMessage.ToListAsync();
        }

        // GET: api/NavMessages/4/2039/1
        [HttpGet("{svId}/{week}/{tow}")]
        public async Task<ActionResult<NavMessage>> GetNavMessage(int svId, int week, int tow)
        {
            var navMessage = await _context.NavMessage
                 .FirstOrDefaultAsync(n => n.SvId == svId && n.Week == week && n.Tow == tow);
            

            if (navMessage == null)
            {
                return NotFound();
            }

            return navMessage;
        }

        //// PUT: api/NavMessages/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutNavMessage(int id, NavMessage navMessage)
        //{
        //    if (id != navMessage.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(navMessage).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!NavMessageExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        // POST: api/NavMessages
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<NavMessage>> PostNavMessage(NavMessage navMessage)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(navMessage.NavigationMessage);
            byte[] signature = SignData(dataBytes, privateKey);
            navMessage.Signature = Convert.ToBase64String(signature);
            _context.NavMessage.Add(navMessage);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNavMessage", new { id = navMessage.Id }, navMessage);
        }

        //// DELETE: api/NavMessages/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteNavMessage(int id)
        //{
        //    var navMessage = await _context.NavMessage.FindAsync(id);
        //    if (navMessage == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.NavMessage.Remove(navMessage);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool NavMessageExists(int id)
        {
            return _context.NavMessage.Any(e => e.Id == id);
        }

        // Function to sign data
        static byte[] SignData(byte[] data, RSAParameters privateKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(privateKey);
                return rsa.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
}
