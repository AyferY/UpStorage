using UpStorage.Domain.Enum;

namespace UpStorage.Domain.Dtos
{
    public class UpStorageAddLogDto
    {
        public string Message { get; set; }
        public DateTimeOffset SentOn { get; set; }

        public UpStorageAddLogDto(string message)
        {
            Message = message;
            SentOn = DateTimeOffset.Now;
        }
    }
}
