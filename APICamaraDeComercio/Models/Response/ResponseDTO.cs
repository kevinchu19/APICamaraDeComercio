namespace APICamaraDeComercio.Models.Response
{
    public class ResponseDTO
    {
        
        public string identificador { get; set; }
        public string status { get; set; }
        public string titulo{ get; set; }
        public string mensaje { get; set; }
        public ComprobanteGenerado comprobantegenerado { get; set; }    

        public ResponseDTO(string? _identificador, string _status, string _titulo, string _mensaje, ComprobanteGenerado _comprobantegenerado)
        {
            this.identificador = _identificador;
            this.status = _status;
            this.titulo = _titulo;
            this.mensaje = _mensaje;
            this.comprobantegenerado = _comprobantegenerado; 
        }
    }
}
