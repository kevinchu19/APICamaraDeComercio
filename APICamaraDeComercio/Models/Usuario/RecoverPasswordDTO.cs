namespace APICamaraDeComercio.Models.Usuario
{
    public class RecoverPasswordDTO
    {
        public string newPassword { get; set; }
        public int? registrosActualizados { get; set; }
        public string? mensaje { get; set; }
    }
}
