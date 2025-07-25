namespace CloneEbay.Models;

public class EmailSettings
{
    public string SmtpServer { get; set; } = string.Empty;
    public int SmtpPort { get; set; }
    public string SmtpUsername { get; set; } = string.Empty;
    public string SmtpPassword { get; set; } = string.Empty;
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string? SenderEmail { get; internal set; }
    public string? SenderName { get; internal set; }
    public string? SmtpHost { get; internal set; }
} 