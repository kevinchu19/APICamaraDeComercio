namespace APICamaraDeComercio.Models.Usuario
{
    public class TerminosYCondicionesDTO
    {
        public bool terminosYCondiciones { get; set; }
        public int? registrosActualizados { get; set; }
        public DateTime? fechaAprobacion { get; set; }
        public string? mensaje { get; set; }
    }
}
