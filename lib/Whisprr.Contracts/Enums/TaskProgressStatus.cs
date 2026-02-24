namespace Whisprr.Contracts.Enums;

public enum TaskProgressStatus
{
  Pending,   // Legacy: mapped to Queued
  Processing, // Legacy: mapped to Queued
  Queued,
  Success,
  Failed
}
