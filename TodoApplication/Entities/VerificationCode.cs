namespace TodoApplication.Entities;

public class VerificationCode
{
    public Guid code_id { get; set; }
    public Guid user_id { get; set; }
    public byte[] codehash { get; set; }
    public DateTime expires_at { get; set; }
    public DateTime? used_at { get; set; }
    public int attempt_count { get; set; }
    public DateTime CreatedAt { get; set; }
}