using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;


public class JWTtoken
{
    private readonly string _ak;
    private readonly string _sk;

    // Use constructor injection to get IConfiguration
    public JWTtoken(IConfiguration configuration)
    {
        _ak = configuration["JWT:AccessKey"];
        _sk = configuration["JWT:SecretKey"];
    }

    public string Sign()
    {
        try
        {
            // Expiration time: current time + 1800s (30 minutes)
            DateTime expiresAt = DateTime.UtcNow.AddMinutes(30);
            // Not before: current time - 5s
            DateTime notBefore = DateTime.UtcNow.AddSeconds(-5);

            // Create the security key and signing credentials
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_sk));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Create the token
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = _ak,
                NotBefore = notBefore,
                Expires = expiresAt,
                SigningCredentials = signingCredentials
            };

            var securityToken = tokenHandler.CreateJwtSecurityToken(tokenDescriptor);

            return tokenHandler.WriteToken(securityToken);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error generating token: {e.Message}");
            return null;
        }
    }
}