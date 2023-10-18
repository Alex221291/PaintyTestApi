namespace PaintyTestApi.ViewModels.MediaViewModels;

public class MediaDownloadViewModel
{
    public string FileName { get; set; }
    public string contentType { get; set; }
    public byte[] Memory { get; set; }
}