namespace LessPaper.Shared.Interfaces.General
{
    public interface IAccessKey
    {
        /// <summary>
        /// File-Key encrypted by the symmetric key
        /// </summary>
        string SymmetricEncryptedFileKey { get; }

        /// <summary>
        /// User Id who issued the encrypted symmetric key
        /// </summary>
        string IssuerId { get; }
    }
}