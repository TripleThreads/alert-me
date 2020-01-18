using AlertMe.Models;

namespace AlertMe.ViewModels
{
    public class AlertCommentsViewModel
    {
        public Alert Alert { get; set; }

        public Comment Comment { get; set; }

        public string JsonFormat { get; set; }

    }
}
