using System.ComponentModel.DataAnnotations;
using LoginWebApi.Exception;

namespace LoginWebApi;

public class Test
{

    [PrimeraLetraMayuscula]
    public string name { get; set; }
    [CreditCard]
    public string tarjetaCredito { get; set; }
    
}