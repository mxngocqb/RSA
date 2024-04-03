using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RsasController : ControllerBase
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

        public RsasController(WebApplication1Context context)
        {
            _context = context;
        }

        // GET: api/Rsas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Rsa>>> GetData()
        {
            return await _context.Data.ToListAsync();
        }

        // GET: api/Rsas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Rsa>> GetRsa(int id)
        {
            var rsa = await _context.Data.FindAsync(id);
          

            if (rsa == null)
            {
                return NotFound();
            }

             return rsa;
        }

        // PUT: api/Rsas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRsa(int id, Rsa rsa)
        {
            if (id != rsa.Id)
            {
                return BadRequest();
            }

            _context.Entry(rsa).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RsaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Rsas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Rsa>> PostRsa(Rsa rsa)
        {
            byte[] dataBytes = Encoding.UTF8.GetBytes(rsa.Value);
            byte[] signature = SignData(dataBytes, privateKey);
            rsa.Signature = Convert.ToBase64String(signature);
            _context.Data.Add(rsa);
            await _context.SaveChangesAsync();           

            return CreatedAtAction("GetRsa", new { id = rsa.Id }, rsa);
        }

        // DELETE: api/Rsas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRsa(int id)
        {
            var rsa = await _context.Data.FindAsync(id);
            if (rsa == null)
            {
                return NotFound();
            }

            _context.Data.Remove(rsa);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool RsaExists(int id)
        {
            return _context.Data.Any(e => e.Id == id);
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

        // Function to verify signature
        static bool VerifyData(byte[] data, byte[] signature, RSAParameters publicKey)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(publicKey);
                return rsa.VerifyData(data, signature, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }

    }
}
