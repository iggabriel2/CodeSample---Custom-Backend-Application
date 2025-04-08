

namespace AdTool.Entities.Logging
{
    public class UIMessage
    {
        public Boolean IsError

        {
            get
            {
                return ErrorMessages.Count > 0;
            }
        }
        public Boolean IsInfo
        {
            get
            {
                return InfoMessages.Count > 0;
            }
        }
        public List<string> ErrorMessages { get; set; }
        public List<string> InfoMessages { get; set; }

        public UIMessage()
        {
            ErrorMessages = new List<string>();
            InfoMessages = new List<string>();
        }
    }
}
