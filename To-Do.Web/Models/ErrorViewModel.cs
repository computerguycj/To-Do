using System;

namespace To_Do.Models
{
    public class ErrorViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        //TODO: Make this a required field and remove the default message.
        public string ErrorMessage { get; set; } = "There was an error";
    }
}
