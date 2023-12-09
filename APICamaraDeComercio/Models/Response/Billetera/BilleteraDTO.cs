namespace APICamaraDeComercio.Models.Response.Billetera
{
    public class BilleteraDTO
    {
        public DateTime fecha { get; set; }
        public string numeroVEP { get; set; }
        public decimal importe { get; set; }
        public string generador { get; set; }
    }
}
