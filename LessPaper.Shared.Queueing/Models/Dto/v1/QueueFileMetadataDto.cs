

using LessPaper.Shared.Enums;

namespace LessPaper.Shared.Queueing.Models.Dto.v1
{
    public class QueueFileMetadataDto
    {
        public string FileId { get; set; }
        
        public string FileName { get; set; }

        public string PlaintextKey { get; set; }

        public string EncryptedKey { get; set; }
        
        public DocumentLanguage DocumentLanguage { get; set; }

        public string DirectoryId { get; set; }
    }
}
