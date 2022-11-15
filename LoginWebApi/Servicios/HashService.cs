using System.Security.Cryptography;
using LoginWebApi.DTOs;

namespace LoginWebApi.Servicios;

public class HashService
{
    public ResultadoHash Hash(string textoPlano)
    {

        var sal = new byte[16];
        using(var random = RandomNumberGenerator.Create())
        {
            random.GetBytes(sal);
        }


        return Hash(textoPlano, sal);
    }

    public ResultadoHash Hash()
    {
        
    }
}