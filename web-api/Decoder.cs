using System.IdentityModel.Tokens.Jwt;

public static class Decoder
{
    public static JwtSecurityToken Decode(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var token = handler.ReadJwtToken(jwtToken);
        return token;
    }
}

