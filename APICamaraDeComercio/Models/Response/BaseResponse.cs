namespace APICamaraDeComercio.Models.Response
{
    public class BaseResponse
    {
        public ResponseDTO response { get; set; }

        public BaseResponse(ResponseDTO response)
        {
            this.response = response;
        }
    }
}
